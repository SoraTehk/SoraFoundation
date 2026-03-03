using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace SoraTehk.Samples {
    public partial class CoreEntryPoint : MonoBehaviour, IAsyncStartable {
        public async UniTask StartAsync(CancellationToken cancellation = default) {
            await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
        }
    }
}