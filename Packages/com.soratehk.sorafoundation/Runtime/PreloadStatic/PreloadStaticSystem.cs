using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SoraTehk.Extensions;
using UnityEngine;

#if USE_ZLINQ
using ZLinq;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.Attributes {
    public static class PreloadStaticSystem {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        // PREF: Move this to loading scene if it took too long, source gen
        private static void HandleAfterAssembliesLoaded() {
#if UNITY_EDITOR
            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
#else
            bool isPlayMode = true;
#endif
            PreloadStaticModes currMode = isPlayMode ? PreloadStaticModes.PLAY_MODE : PreloadStaticModes.EDIT_MODE;
            // All types that had our attribute & matched current player mode
            var typesWithAttr = AppDomain.CurrentDomain.GetAssemblies()
#if USE_ZLINQ
                .AsValueEnumerable()
#endif
                .SelectMany(a => a.GetTypes())
                .Select(t => (
                        Type: t,
                        Attribute: t.GetCustomAttribute<PreloadStaticAttribute>()
                    )
                )
                .Where(tp =>
                    tp.Attribute != null &&
                    (tp.Attribute.Modes & currMode) != 0
                )
                .ToList();

            // After(ZLogger) -> This(SoraLog) -> Before(...)
            // https://www.geeksforgeeks.org/dsa/introduction-to-directed-acyclic-graph/
            var graph = new Dictionary<Type, HashSet<Type>>(typesWithAttr.Count);
            foreach (var (type, attr) in typesWithAttr) {
                // Contain all types that depend on this type
                // Ex: graph[SoraLog] = {ZLogger}
                var dependantSet = graph.GetOrAdd(type);

                if (attr.AfterTypes != null) {
                    foreach (var afterType in attr.AfterTypes) {
                        dependantSet.Add(afterType);
                    }
                }
                if (attr.BeforeTypes != null) {
                    foreach (var beforeType in attr.BeforeTypes) {
                        var beforeDepSet = graph.GetOrAdd(beforeType);
                        beforeDepSet.Add(type);
                    }
                }
            }
            // https://www.geeksforgeeks.org/dsa/topological-sort-using-dfs/
            var visited = new HashSet<Type>();
            var orderedTypeList = new List<Type>();
            var currentEnumerator = new Dictionary<Type, IEnumerator<Type>>();
            // For each type
            foreach (var startNode in graph.Keys) {
                if (visited.Contains(startNode)) continue;

                var stack = new Stack<Type>();
                stack.Push(startNode);
                currentEnumerator[startNode] = graph[startNode].GetEnumerator();
                // Traverse it dependants (deep first search)
                while (stack.TryPeek(out var node)) {
                    var enumerator = currentEnumerator[node];
                    bool hasUnvisited = false;

                    // Process neighbor
                    while (enumerator.MoveNext()) {
                        var neighbor = enumerator.Current!;

                        if (stack.Contains(neighbor)) {
                            // Convert stack to list from bottom -> top
                            var stackList = stack.Reverse().ToList();
                            // First occurrence of a repeated node
                            var startIndex = stackList.IndexOf(neighbor);
                            // Build the cycle
                            var cycleTypeList = stackList.Skip(startIndex)
                                .Concat(
                                    new[] {
                                        neighbor
                                    }
                                )
                                .ToList();

                            throw new InvalidOperationException($"Cycle detected: {string.Join(" -> ", cycleTypeList.Select(t => t.Name))}");
                        }

                        if (!visited.Contains(neighbor)) {
                            stack.Push(neighbor);
                            currentEnumerator[neighbor] = graph.GetValueOrDefault(neighbor, new HashSet<Type>()).GetEnumerator();
                            hasUnvisited = true;
                            break;
                        }
                    }
                    // Finished this branch (with startNode as root)
                    if (!hasUnvisited) {
                        stack.Pop();
                        visited.Add(node);
                        orderedTypeList.Add(node);
                        currentEnumerator.Remove(node);
                    }
                }
            }

            foreach (var type in orderedTypeList) {
                Debug.Log($"[PreloadStatic] RunClassConstructor: Type={type.GetFriendlyTypeName()}");
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
    }
}