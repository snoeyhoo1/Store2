using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    public static void BuildAndroid()
    {
        const string outputDir = "android";
        Directory.CreateDirectory(outputDir);

        PlayerSettings.productName = "Store1";
        PlayerSettings.applicationIdentifier = "com.store1.tycoon";
        PlayerSettings.bundleVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0";
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
        if (int.TryParse(buildNumber, out var code) && code > 0)
            PlayerSettings.Android.bundleVersionCode = code;

        // Use Unity's default debug signing for the test APK workflow.
        PlayerSettings.Android.useCustomKeystore = false;

        var scenePath = "Assets/Scenes/Store1.unity";
        if (!File.Exists(scenePath))
            throw new Exception("Missing scene: " + scenePath);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(scenePath, true)
        };

        var useAab = string.Equals(
            Environment.GetEnvironmentVariable("BUILD_APP_BUNDLE"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        EditorUserBuildSettings.buildAppBundle = useAab;

        var extension = useAab ? ".aab" : ".apk";
        var outputPath = Path.Combine(outputDir, "Store1" + extension);

        var report = BuildPipeline.BuildPlayer(
            new BuildPlayerOptions
            {
                scenes = new[] { scenePath },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"Android build failed: {report.summary.result}\n{report.summary.totalErrors} errors");

        Debug.Log($"STORE1_ANDROID_BUILD_OK: {outputPath}");
    }
}
