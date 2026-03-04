using System;
using SoraTehk.Diagnostic;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SoraTehk.Prepare {
    public static partial class PreparableSystem {
        public static readonly string[] gSearchFolders = new[] {
            "Assets/_Project",
            "Assets/Editor Default Resources",
            "Assets/Gizmos",
            "Assets/Resources",
            "Assets/Settings"
        };

        [MenuItem("Tools/SoraTehk/Prepare/PrepareProjectAssets")]
        public static void PrepareProjectAssets() {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                SorLog.LogInfo($"PrepareProjectAssets cancelled by user.");
                return;
            }
            var originalSetup = EditorSceneManager.GetSceneManagerSetup();

            GUID[] prepareGuids = AssetDatabase.FindAssetGUIDs("t:SceneAsset t:GameObject t:ScriptableObject", gSearchFolders);
            int changeCount = 0;
            int skipCount = 0;

            try {
                for (int i = 0; i < prepareGuids.Length; i++) {
                    var guid = prepareGuids[i];
                    var asset = AssetDatabase.LoadAssetByGUID<uObject>(guid);
                    if (asset == null) continue;

                    if (i % 32 == 0) {
                        float percent = (float)i / prepareGuids.Length;
                        EditorUtility.DisplayProgressBar(
                            $"Preparing {percent:P1}",
                            $"Processing {i}/{prepareGuids.Length}: {asset.name}",
                            percent
                        );
                    }

                    if (!AssetDatabase.IsOpenForEdit(asset)) {
                        skipCount++;
                        continue;
                    }

                    Type mainType = AssetDatabase.GetMainAssetTypeFromGUID(guid);
                    PrepareResult result = default;

                    if (typeof(SceneAsset).IsAssignableFrom(mainType) && asset is SceneAsset sceneAsset) {
                        result = Prepare(sceneAsset);
                    }
                    else if (typeof(GameObject).IsAssignableFrom(mainType) && asset is GameObject go) {
                        result = Prepare(go);
                    }
                    else if (typeof(ScriptableObject).IsAssignableFrom(mainType) && asset is ScriptableObject so) {
                        result = Prepare(so);
                    }

                    switch (result) {
                        case PrepareResult.Invalid:
                            skipCount++;
                            break;
                        case PrepareResult.Prepared:
                            SorLog.LogInfo($"Prepared '{asset.name}' ({mainType.Name})", asset);
                            changeCount++;
                            break;
                        case PrepareResult.None:
                        case PrepareResult.Unchanged:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"PrepareResult '{result}' is not supported.");
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();

                if (originalSetup is { Length: > 0 }) {
                    EditorSceneManager.RestoreSceneManagerSetup(originalSetup);
                }
                else {
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SorLog.LogInfo($"Finished Preparing: Total={prepareGuids.Length}, Changed={changeCount}, Skipped={skipCount}");
        }

        public static PrepareResult Prepare<T>(T asset) where T : uObject {
            return asset switch {
                SceneAsset sceneAsset => PrepareScene(sceneAsset),
                Component cpn when cpn.gameObject.scene.IsValid() => PrepareComponent(cpn),
                Component cpn when PrefabUtility.IsPartOfPrefabAsset(cpn.gameObject) => PreparePrefab(cpn.gameObject),
                GameObject gObj => PreparePrefab(gObj),
                ScriptableObject sObj => PrepareScriptableObject(sObj),
                _ => PrepareResult.Invalid
            };
        }

        internal static PrepareResult PrepareScene(SceneAsset sceneAsset) {
            try {
                string path = AssetDatabase.GetAssetPath(sceneAsset);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

                bool changed = false;
                var rootGObjs = scene.GetRootGameObjects();
                foreach (var gObj in rootGObjs) {
                    var prepares = gObj.GetComponentsInChildren<IPreparable>(true);
                    foreach (var prepare in prepares) {
                        if (prepare.Prepare()) {
                            changed = true;
                        }
                    }
                }
                if (!changed) return PrepareResult.Unchanged;

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                return PrepareResult.Prepared;
            }
            catch (Exception ex) {
                SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(sceneAsset)}' ({sceneAsset.GetType().Name}): {ex}");
                return PrepareResult.Invalid;
            }
        }

        internal static PrepareResult PreparePrefab(GameObject gObj) {
            if (!gObj.IsValidOrWarn()) return PrepareResult.Invalid;
            if (!PrefabUtility.IsPartOfPrefabAsset(gObj)) return PrepareResult.Unchanged;

            try {
                bool changed = false;
                var prepares = gObj.GetComponentsInChildren<IPreparable>(true);
                foreach (var prepare in prepares) {
                    if (prepare.Prepare()) {
                        changed = true;
                    }
                }
                if (!changed) return PrepareResult.Unchanged;

                PrefabUtility.SavePrefabAsset(gObj);
                return PrepareResult.Prepared;
            }
            catch (Exception ex) {
                SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(gObj)} (Prefab)': {ex}");
                return PrepareResult.Invalid;
            }
        }

        internal static PrepareResult PrepareComponent(Component cpn) {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (cpn is not IPreparable prepare) return PrepareResult.Invalid;

            var scene = cpn.gameObject.scene;
            if (!scene.IsValid()) return PrepareResult.Invalid;

            try {
                if (!prepare.Prepare()) return PrepareResult.Unchanged;

                EditorSceneManager.MarkSceneDirty(scene);
                return PrepareResult.Prepared;
            }
            catch (Exception ex) {
                SorLog.LogWarning($"Skipping '{cpn.name}' ({cpn.GetType().Name}): {ex}");
                return PrepareResult.Invalid;
            }
        }

        internal static PrepareResult PrepareScriptableObject(ScriptableObject sObj) {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (sObj is not IPreparable prepare) return PrepareResult.Invalid;

            try {
                if (!prepare.Prepare()) return PrepareResult.Unchanged;

                EditorUtility.SetDirty(sObj);
                return PrepareResult.Prepared;
            }
            catch (Exception ex) {
                SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(sObj)}' ({sObj.GetType().Name}): {ex}");
                return PrepareResult.Invalid;
            }
        }
    }
}