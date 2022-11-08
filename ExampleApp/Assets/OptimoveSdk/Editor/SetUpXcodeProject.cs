using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.Build;

public class SetUpXcodeProject
{
    private static readonly string _appGroupName = $"group.{PlayerSettings.applicationIdentifier}.optimove";

    private static readonly string _optimovePlistSrc = "Assets/Plugins/iOS/optimove.plist";

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        #if UNITY_2019_3_OR_NEWER
            DoProjectSetup(buildTarget, pathToBuiltProject);
        #endif
    }

    private static void DoProjectSetup(BuildTarget buildTarget, string pathToBuiltProject)
    {
        var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        var project = new PBXProject();

        project.ReadFromFile(projectPath);

        //2019.3+ only
        string mainTargetGuid = project.GetUnityMainTargetGuid();
        string unityTarget = project.GetUnityFrameworkTargetGuid();

        // copy optimove.plist as *.plist files are not copied automatically
        AddOptimoveConfig(project, pathToBuiltProject, unityTarget);

        // Push Notifications, Background Modes, App Groups for the main target
        SetupMainTargetCapabilities(project, projectPath, pathToBuiltProject, mainTargetGuid);

        // enables calling objc functions from swift
        SetModuleMap(project, unityTarget, pathToBuiltProject);

        // add UNITY_RUNTIME_VERSION to Info.plist of the framework target
        AddUnityVersionToPlist(pathToBuiltProject);

        SetBuildProperties(project, mainTargetGuid, unityTarget);

        AddXcframeworks(project, pathToBuiltProject, mainTargetGuid, unityTarget);

        project.WriteToFile(projectPath);
    }

    private static void SetBuildProperties(PBXProject project, string mainTargetGuid, string unityTarget)
    {
        project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        project.SetBuildProperty(unityTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
    }

    private static void AddOptimoveConfig(PBXProject project, string pathToBuiltProject, string unityTargetGuid)
    {
        var dstLocalPath = "Libraries/Plugins/iOS/optimove.plist";
        var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
        File.Copy(_optimovePlistSrc, dstPath, true);
        project.AddFileToBuild(unityTargetGuid, project.AddFile(dstLocalPath, dstLocalPath));
    }

    private static void SetupMainTargetCapabilities(PBXProject project, string projectPath, string pathToBuiltProject, string mainTargetGuid) {
        var mainTargetName = "Unity-iPhone";

        var entitlementsPath = GetEntitlementsPath(project, mainTargetGuid, mainTargetName, pathToBuiltProject);
        var projCapability = new ProjectCapabilityManager(projectPath, entitlementsPath, mainTargetName);

        projCapability.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications | BackgroundModesOptions.BackgroundFetch);
        projCapability.AddPushNotifications(false);
        projCapability.AddAppGroups(new[] { _appGroupName });

        // associated domains
        string host = getValueFromOptimovePlist("optimoveDeferredDeepLinkingHost");
        if (host != null){
            string[] domains = {"applinks:" + host};
            projCapability.AddAssociatedDomains(domains);
        }

        projCapability.WriteToFile();
    }

    // Get existing entitlements file if exists or creates a new file, adds it to the project, and returns the path
    private static string GetEntitlementsPath(PBXProject project, string targetGuid, string targetName, string pathToBuiltProject) {
        var relativePath = project.GetBuildPropertyForAnyConfig(targetGuid, "CODE_SIGN_ENTITLEMENTS");

        if (relativePath != null) {
            var fullPath = Path.Combine(pathToBuiltProject, relativePath);

            if (File.Exists(fullPath))
                return fullPath;
        }

        var entitlementsPath = Path.Combine(pathToBuiltProject, targetName, $"{targetName}.entitlements");

        // make new file
        var entitlementsPlist = new PlistDocument();
        entitlementsPlist.WriteToFile(entitlementsPath);

        // Copy the entitlement file to the xcode project
        var entitlementFileName = Path.GetFileName(entitlementsPath);
        var relativeDestination = targetName + "/" + entitlementFileName;

        // Add the pbx configs to include the entitlements files on the project
        project.AddFile(relativeDestination, entitlementFileName);
        project.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativeDestination);

        return relativeDestination;
    }

    private static void SetModuleMap(PBXProject project, string unityTarget, string buildPath)
    {
        // Modulemap
        project.AddBuildProperty(unityTarget, "DEFINES_MODULE", "YES");

        var moduleFile = buildPath + "/UnityFramework/UnityFramework.modulemap";
        if (!File.Exists(moduleFile))
        {
            FileUtil.CopyFileOrDirectory("Assets/Plugins/iOS/UnityFramework.modulemap", moduleFile);
            project.AddFile(moduleFile, "UnityFramework/UnityFramework.modulemap");
            project.AddBuildProperty(unityTarget, "MODULEMAP_FILE", "$(SRCROOT)/UnityFramework/UnityFramework.modulemap");
        }

        // Headers
        string pluginObjcInterfaceGuid = project.FindFileGuidByProjectPath("Libraries/Plugins/iOS/Swift-objc-bridging-header.h");
        project.AddPublicHeaderToBuild(unityTarget, pluginObjcInterfaceGuid);
    }

    private static void AddUnityVersionToPlist(string buildPath)
    {
        var plistPath = buildPath + "/UnityFramework/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;
        rootDict.SetString("unityEngineVersionForOptimoveReporting", "$(UNITY_RUNTIME_VERSION)");

        plist.WriteToFile(plistPath);
    }

    private static string getValueFromOptimovePlist(string key)
    {
        PlistDocument optimoveConfig = new PlistDocument();
        optimoveConfig.ReadFromFile(_optimovePlistSrc);
        PlistElementDict rootDict = optimoveConfig.root;
        PlistElement host = rootDict[key];
        if (host == null){
            return null;
        }

        return host.AsString();
    }

    private static void AddXcframeworks(PBXProject project, string pathToBuiltProject, string mainTargetGuid, string unityTarget)
    {
        AddXcframework(project, pathToBuiltProject, mainTargetGuid, unityTarget, "OptimoveSDK.xcframework");
        AddXcframework(project, pathToBuiltProject, mainTargetGuid, unityTarget, "OptimoveSDKCore.xcframework");
    }

    private static void AddXcframework(PBXProject project, string pathToBuiltProject, string mainTargetGuid, string unityTargetGuid, string frameworkName)
    {
        string src = Path.Combine(Application.dataPath, ".OptimoveNativeAssets/" + frameworkName);
        string dest = Path.Combine(pathToBuiltProject, frameworkName);

        if (Directory.Exists(dest)){
            return;
        }

        if (!Directory.Exists(src)){
            throw new BuildFailedException("Please, add " + frameworkName + " to .OptimoveNativeAssets folder");
        }

        CopyDirectory(src, dest);

        string fileGuid = project.AddFile(dest, Path.Combine("Frameworks", frameworkName));

        //MAIN TARGET
        //embed and sign
        project.AddFileToEmbedFrameworks(mainTargetGuid, fileGuid);

        //link binary
        string mPhaseGuid = project.GetFrameworksBuildPhaseByTarget(mainTargetGuid);
        project.AddFileToBuildSection(mainTargetGuid, mPhaseGuid, fileGuid);

        //UNITY FRAMEWORK TARGET

        //link binary
        string uPhaseGuid = project.GetFrameworksBuildPhaseByTarget(unityTargetGuid);
        project.AddFileToBuildSection(unityTargetGuid, uPhaseGuid, fileGuid);
    }

    private static void CopyDirectory(string sourcePath, string destPath)
    {
        Directory.CreateDirectory(destPath);

        foreach (string file in Directory.GetFiles(sourcePath))
            File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)));

        foreach (string dir in Directory.GetDirectories(sourcePath))
            CopyDirectory(dir, Path.Combine(destPath, Path.GetFileName(dir)));
    }
}
