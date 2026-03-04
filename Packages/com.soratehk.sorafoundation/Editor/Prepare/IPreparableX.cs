using System;
using SoraTehk.Diagnostic;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SoraTehk.Prepare {
    public static partial class IPreparableX {
        [MenuItem("Tools/SoraTehk/Prepare/PrepareProjectAssets")]
        public static void PrepareProjectAssets() {
            // Prompt user to save any dirty scenes before we begin
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                SorLog.LogInfo($"PrepareProjectAssets cancelled by user.");
                return;
            }
            // Save currently opened scenes to restore later
            var originalSetup = EditorSceneManager.GetSceneManagerSetup();

            GUID[] prepareGuids = AssetDatabase.FindAssetGUIDs("t:SceneAsset t:GameObject t:ScriptableObject", new[] {
                "Assets/_Project",
                "Assets/Resources",
                "Assets/Settings"
            }); // All possible IPrepare assets
            int changeCount = 0;
            int skipCount = 0;

            try {
                for (int i = 0; i < prepareGuids.Length; i++) {
                    var guid = prepareGuids[i];
                    var asset = AssetDatabase.LoadAssetByGUID<uObject>(guid);
                    if (asset == null) continue;
                    // Progress bar
                    if (i % 100 == 0) {
                        float percent = (float)i / prepareGuids.Length;
                        EditorUtility.DisplayProgressBar(
                            $"Preparing {percent:P1}",
                            $"Processing {i}/{prepareGuids.Length}: {asset.name}",
                            percent
                        );
                    }
                    // Immutable asset
                    if (!AssetDatabase.IsOpenForEdit(asset)) {
                        skipCount++;
                        continue;
                    }
                    // Args
                    Type mainType = AssetDatabase.GetMainAssetTypeFromGUID(guid);
                    // Process
                    try {
                        if (typeof(ScriptableObject).IsAssignableFrom(mainType)) {
                            // ReSharper disable once SuspiciousTypeConversion.Global
                            if (asset is not IPreparable prepare) continue;

                            if (prepare.Prepare()) {
                                EditorUtility.SetDirty(asset);
                                SorLog.LogInfo($"Prepared: {asset.name} (ScriptableObject)", asset);
                                changeCount++;
                            }
                        }
                        else if (typeof(GameObject).IsAssignableFrom(mainType)) {
                            var prefab = asset as GameObject;
                            if (prefab == null || !prefab.IsValidOrWarn()) continue;

                            bool changed = false;
                            foreach (var prepare in prefab.GetComponentsInChildren<IPreparable>(true)) {
                                if (prepare.Prepare()) changed = true;
                            }

                            if (changed) {
                                EditorUtility.SetDirty(prefab);
                                if (PrefabUtility.IsPartOfPrefabAsset(prefab)) {
                                    try {
                                        PrefabUtility.SavePrefabAsset(prefab);
                                        SorLog.LogInfo($"Prepared: {prefab.name} (Prefab)", asset);
                                        changeCount++;
                                    }
                                    catch (Exception ex) {
                                        SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}': {ex}");
                                        skipCount++;
                                    }
                                }
                            }
                        }
                        else if (typeof(SceneAsset).IsAssignableFrom(mainType)) {
                            try {
                                string path = AssetDatabase.GetAssetPath(asset);
                                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                                bool changed = false;

                                foreach (var root in scene.GetRootGameObjects()) {
                                    foreach (var prepare in root.GetComponentsInChildren<IPreparable>(true)) {
                                        if (prepare.Prepare()) changed = true;
                                    }
                                }

                                if (changed) {
                                    EditorSceneManager.MarkSceneDirty(scene);
                                    EditorSceneManager.SaveScene(scene);
                                    SorLog.LogInfo($"Prepared: {scene.name} (Prefab)", asset);
                                    changeCount++;
                                }
                            }
                            catch (Exception ex) {
                                SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}' (SceneAsset): {ex}");
                                skipCount++;
                            }
                        }
                    }
                    catch (Exception ex) {
                        SorLog.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}': {ex}");
                        skipCount++;
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();

                // Restore the originally opened scenes (or default scene)
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
    }
}