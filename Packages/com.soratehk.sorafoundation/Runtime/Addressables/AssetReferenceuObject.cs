#if USE_ADDRESSABLE
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.AddressablesAddons {
    [Serializable]
    public class AssetReferenceuObject<T> : AssetReference where T : uObject {
        public AssetReferenceuObject() { }
        public AssetReferenceuObject(string guid) : base(guid) { }

        public new T? Asset => base.Asset as T;
#if UNITY_EDITOR
        public new T? editorAsset => base.editorAsset as T;
#endif
        
        public AsyncOperationHandle<T> LoadAssetAsync() {
            return base.LoadAssetAsync<T>();
        }

        public override bool ValidateAsset(uObject obj) {
#if UNITY_EDITOR
            return obj is T;
#else
            return base.ValidateAsset(obj);
#endif
        }
        public override bool ValidateAsset(string path) {
#if UNITY_EDITOR
            var asset = AssetDatabase.GetMainAssetTypeAtPath(path);
            return typeof(T).IsAssignableFrom(asset);
#else
            return base.ValidateAsset(path);
#endif
        }
    }
}
#endif