using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static class PackageManagerX {
        public static async Awaitable WaitForCompletion(this Request req, CancellationToken ct = default) {
            while (!req.IsCompleted && !ct.IsCancellationRequested) {
                await Awaitable.NextFrameAsync(ct);
            }
            ct.ThrowIfCancellationRequested();
        }
    }
}