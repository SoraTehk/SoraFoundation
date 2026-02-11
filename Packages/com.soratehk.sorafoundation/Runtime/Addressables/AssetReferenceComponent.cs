#if USE_ADDRESSABLE
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.AddressablesAddons {
    [Serializable]
    public class AssetReferenceComponent<T> : AssetReference where T : Component {
        public AssetReferenceComponent() { }
        public AssetReferenceComponent(string guid) : base(guid) { }

        public new T? Asset {
            get {
                if (m_Asset != null) return m_Asset;

                return m_Asset = (base.Asset as GameObject)?.GetComponent<T>();
            }
        }
        private T? m_Asset;
#if UNITY_EDITOR
        public new GameObject? editorAsset => base.editorAsset as GameObject;
#endif

        public AsyncOperationHandle<T> LoadAssetAsync() {
            return base.LoadAssetAsync<T>();
        }

        public override bool ValidateAsset(uObject obj) {
#if UNITY_EDITOR
            return obj is GameObject gObj && gObj.TryGetComponent<T>(out _);
#else
            return base.ValidateAsset(obj);
#endif
        }

        public override bool ValidateAsset(string path) {
#if UNITY_EDITOR
            var gObj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return gObj != null && gObj.TryGetComponent<T>(out _);
#else
            return base.ValidateAsset(path);
#endif
        }
    }
}
#endif