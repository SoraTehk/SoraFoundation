using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Samples {
    public partial class BootstrapEntryPoint : MonoBehaviour {
        private void Start() {
            Async().Forget();
            async UniTask Async() {
                await SceneManager.LoadSceneAsync("Core", LoadSceneMode.Additive);
                await SceneManager.UnloadSceneAsync("Bootstrap");
            }
        }
    }
}