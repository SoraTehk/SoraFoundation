using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace SoraTehk.Samples {
    public partial class MainEntryPoint : MonoBehaviour, IAsyncStartable {
        public async UniTask StartAsync(CancellationToken cancellation = default) {
            Debug.Log("Scene flow successfully!");
        }
    }
}