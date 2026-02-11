#if USE_ADDRESSABLE
using System;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.AddressablesAddons {
    [Serializable]
    public class AssetReferenceScene : AssetReference {
        public AssetReferenceScene() { }
        public AssetReferenceScene(string guid) : base(guid) { }

#if UNITY_EDITOR
        public new SceneAsset? editorAsset => base.editorAsset as SceneAsset;
#endif

        public override bool ValidateAsset(uObject obj) {
#if UNITY_EDITOR
            return typeof(SceneAsset).IsAssignableFrom(obj.GetType());
#else
            return base.ValidateAsset(obj);
#endif
        }
        public override bool ValidateAsset(string path) {
#if UNITY_EDITOR
            return typeof(SceneAsset).IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(path));
#else
            return base.ValidateAsset(path);
#endif
        }
    }
}
#endif