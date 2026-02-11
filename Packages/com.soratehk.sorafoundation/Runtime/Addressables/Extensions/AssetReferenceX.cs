#if USE_ADDRESSABLE
using System.Diagnostics.CodeAnalysis;
using SoraTehk.AddressablesAddons;

namespace SoraTehk.Extensions {
    public static partial class AssetReferenceX {
#if UNITY_EDITOR
        public static bool E_TryConvert<T, TAs>(
            this AssetReferenceuObject<T> assetRef,
            [NotNullWhen(true)] out AssetReferenceuObject<TAs>? asAssetRef
        ) where T : TAs
          where TAs : uObject {
            //
            asAssetRef = null;

            if (assetRef.editorAsset == null) {
                return false;
            }

            if (assetRef.editorAsset is not TAs type) {
                return false;
            }

            asAssetRef = new AssetReferenceuObject<TAs>();
            asAssetRef.SetEditorAsset(type);

            return true;
        }
#endif
    }
}

#endif