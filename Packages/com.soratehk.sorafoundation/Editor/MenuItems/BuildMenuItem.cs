using System;
using System.IO;
using System.Linq;
using SoraTehk.BuildTool;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Tools {
    public static partial class SoraTehkMenuItem {
        [MenuItem("Tools/SoraTehk/DryBuild")]
        public static void DryBuild() {
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            BuildTarget target = BuildTarget.WebGL;

            string platformFolder = target.ToString();
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string buildPath = Path.Combine(projectRoot, "Builds", ".Temp", platformFolder);
            Directory.CreateDirectory(buildPath);

            BuildPlayerOptions options = new BuildPlayerOptions {
                scenes = scenes,
                locationPathName = buildPath,
                target = target,
                options = BuildOptions.BuildScriptsOnly |
                          BuildOptions.Development |
                          BuildOptions.DetailedBuildReport |
                          BuildOptions.StrictMode
            };

            var snapshot = MementoPlayerSettings.CreateSnapshot();
            
                
            var originWebGlPlayerSettings = new {
                memorySize = PlayerSettings.WebGL.memorySize,
                exceptionSupport = PlayerSettings.WebGL.exceptionSupport,
                nameFilesAsHashes = PlayerSettings.WebGL.nameFilesAsHashes,
                showDiagnostics = PlayerSettings.WebGL.showDiagnostics,
                dataCaching = PlayerSettings.WebGL.dataCaching,
                debugSymbols = PlayerSettings.WebGL.debugSymbolMode,
                emscriptenArgs = PlayerSettings.WebGL.emscriptenArgs,
                modulesDirectory = PlayerSettings.WebGL.modulesDirectory,
                template = PlayerSettings.WebGL.template,
                analyzeBuildSize = PlayerSettings.WebGL.analyzeBuildSize,
                useEmbeddedResources = PlayerSettings.WebGL.useEmbeddedResources,
                compressionFormat = PlayerSettings.WebGL.compressionFormat,
                wasmArithmeticExceptions = PlayerSettings.WebGL.wasmArithmeticExceptions,
                linkerTarget = PlayerSettings.WebGL.linkerTarget,
                threadsSupport = PlayerSettings.WebGL.threadsSupport,
                decompressionFallback = PlayerSettings.WebGL.decompressionFallback,
                initialMemorySize = PlayerSettings.WebGL.initialMemorySize,
                maximumMemorySize = PlayerSettings.WebGL.maximumMemorySize,
                memoryGrowthMode = PlayerSettings.WebGL.memoryGrowthMode,
                memoryLinearGrowthStep = PlayerSettings.WebGL.linearMemoryGrowthStep,
                memoryGeometricGrowthStep = PlayerSettings.WebGL.geometricMemoryGrowthStep,
                memoryGeometricGrowthCap = PlayerSettings.WebGL.memoryGeometricGrowthCap,
                powerPreference = PlayerSettings.WebGL.powerPreference,
                webAssemblyTable = PlayerSettings.WebGL.webAssemblyTable,
                webAssemblyBigInt = PlayerSettings.WebGL.webAssemblyBigInt
            };

            try {
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
                PlayerSettings.WebGL.nameFilesAsHashes = false;
                PlayerSettings.WebGL.showDiagnostics = false;
                PlayerSettings.WebGL.dataCaching = false;
                PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                PlayerSettings.WebGL.threadsSupport = false;
                PlayerSettings.WebGL.decompressionFallback = false;

                var report = BuildPipeline.BuildPlayer(options);
                Debug.Log($"Finished: Result={report.summary.result}, Time={report.summary.totalTime}");
            }
            finally {
                PlayerSettings.WebGL.exceptionSupport = originWebGlPlayerSettings.exceptionSupport;
                PlayerSettings.WebGL.debugSymbolMode = originWebGlPlayerSettings.debugSymbols;
                PlayerSettings.WebGL.compressionFormat = originWebGlPlayerSettings.compressionFormat;
                PlayerSettings.WebGL.threadsSupport = originWebGlPlayerSettings.threadsSupport;
                PlayerSettings.WebGL.decompressionFallback = originWebGlPlayerSettings.decompressionFallback;
                PlayerSettings.WebGL.nameFilesAsHashes = originWebGlPlayerSettings.nameFilesAsHashes;
                PlayerSettings.WebGL.dataCaching = originWebGlPlayerSettings.dataCaching;
                PlayerSettings.WebGL.showDiagnostics = originWebGlPlayerSettings.showDiagnostics;
            }
        }
    }
}