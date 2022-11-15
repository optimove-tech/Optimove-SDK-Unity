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

    private static readonly string _optimovePlistSrc = Path.Combine("Assets", "Plugins", "iOS", "optimove.plist");

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

        AddXcframeworksToUnityFrameworkTarget(project, pathToBuiltProject, mainTargetGuid, unityTarget);

        MaybeSetUpOptimoveNotificationServiceExtension(project, mainTargetGuid, pathToBuiltProject, projectPath);

        project.WriteToFile(projectPath);
    }

    private static void SetBuildProperties(PBXProject project, string mainTargetGuid, string unityTarget)
    {
        project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        project.SetBuildProperty(unityTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
    }

    private static void AddOptimoveConfig(PBXProject project, string pathToBuiltProject, string unityTargetGuid)
    {
        var dstLocalPath = Path.Combine("Libraries", "Plugins", "iOS", "optimove.plist");
        var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
        File.Copy(_optimovePlistSrc, dstPath, true);
        project.AddFileToBuild(unityTargetGuid, project.AddFile(dstLocalPath, dstLocalPath));
    }

    private static void SetupMainTargetCapabilities(PBXProject project, string projectPath, string pathToBuiltProject, string mainTargetGuid) {
        var mainTargetName = "Unity-iPhone";

        var entitlementsPath = GetEntitlementsPath(project, mainTargetGuid, mainTargetName, pathToBuiltProject, false);
        var projCapability = new ProjectCapabilityManager(projectPath, entitlementsPath, mainTargetName);

        projCapability.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications | BackgroundModesOptions.BackgroundFetch);
        projCapability.AddPushNotifications(false);
        projCapability.AddAppGroups(new[] { _appGroupName });

        // associated domains
        string host = GetValueFromOptimovePlist("optimoveDeferredDeepLinkingHost");
        if (host != null){
            string[] domains = {"applinks:" + host};
            projCapability.AddAssociatedDomains(domains);
        }

        projCapability.WriteToFile();
    }

    // Get existing entitlements file if exists or creates a new file, adds it to the project, and returns the path
    private static string GetEntitlementsPath(PBXProject project, string targetGuid, string targetName, string pathToBuiltProject, bool isExtension) {
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
        var relativeDestination = Path.Combine(targetName, entitlementFileName);

        // Add the pbx configs to include the entitlements files on the project
        project.AddFile(relativeDestination, isExtension ? relativeDestination : entitlementFileName);
        project.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativeDestination);

        return relativeDestination;
    }

    private static void SetModuleMap(PBXProject project, string unityTarget, string pathToBuiltProject)
    {
        // Modulemap
        project.AddBuildProperty(unityTarget, "DEFINES_MODULE", "YES");

        var moduleFile = Path.Combine(pathToBuiltProject, "UnityFramework", "UnityFramework.modulemap");
        if (!File.Exists(moduleFile))
        {
            FileUtil.CopyFileOrDirectory("Assets/Plugins/iOS/UnityFramework.modulemap", moduleFile.Replace("\\", "/"));
            project.AddFile(moduleFile, Path.Combine("UnityFramework", "UnityFramework.modulemap"));
            project.AddBuildProperty(unityTarget, "MODULEMAP_FILE", "$(SRCROOT)/UnityFramework/UnityFramework.modulemap");
        }

        // Headers
        string pluginObjcInterfaceGuid = project.FindFileGuidByProjectPath(Path.Combine("Libraries", "Plugins", "iOS", "Swift-objc-bridging-header.h"));
        project.AddPublicHeaderToBuild(unityTarget, pluginObjcInterfaceGuid);
    }

    private static void AddUnityVersionToPlist(string pathToBuiltProject)
    {
        string plistPath = Path.Combine(pathToBuiltProject, "UnityFramework", "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PlistElementDict rootDict = plist.root;
        rootDict.SetString("unityEngineVersionForOptimoveReporting", "$(UNITY_RUNTIME_VERSION)");

        plist.WriteToFile(plistPath);
    }

    private static string GetValueFromOptimovePlist(string key)
    {
        PlistDocument optimoveConfig = new PlistDocument();
        optimoveConfig.ReadFromFile(_optimovePlistSrc);
        PlistElementDict rootDict = optimoveConfig.root;
        PlistElement element = rootDict[key];
        if (element == null){
            return null;
        }

        string val = element.AsString();
        if (val.Equals("")){
            return null;
        }

        return val;
    }

    private static void AddXcframeworksToUnityFrameworkTarget(PBXProject project, string pathToBuiltProject, string mainTargetGuid, string unityTarget)
    {
        AddXcframework(project, pathToBuiltProject, mainTargetGuid, unityTarget, "OptimoveSDK.xcframework");
        AddXcframework(project, pathToBuiltProject, mainTargetGuid, unityTarget, "OptimoveSDKCore.xcframework");
    }

    private static void AddXcframework(PBXProject project, string pathToBuiltProject, string mainTargetGuid, string secondaryTargetGuid, string frameworkName)
    {
        string src = Path.Combine(Application.dataPath, ".OptimoveNativeAssets", frameworkName);
        string dest = Path.Combine(pathToBuiltProject, frameworkName);

        if (Directory.Exists(dest)){
            return;
        }

        if (!Directory.Exists(src)){
            throw new BuildFailedException("Please, add .OptimoveNativeAssets to Assets folder");
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
        string uPhaseGuid = project.GetFrameworksBuildPhaseByTarget(secondaryTargetGuid);
        project.AddFileToBuildSection(secondaryTargetGuid, uPhaseGuid, fileGuid);
    }

    private static void CopyDirectory(string sourcePath, string destPath)
    {
        Directory.CreateDirectory(destPath);

        foreach (string file in Directory.GetFiles(sourcePath))
            File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)));

        foreach (string dir in Directory.GetDirectories(sourcePath))
            CopyDirectory(dir, Path.Combine(destPath, Path.GetFileName(dir)));
    }

    private static void MaybeSetUpOptimoveNotificationServiceExtension(PBXProject project, string mainTargetGuid, string pathToBuiltProject, string projectPath)
    {
        string optimobileCredentials = GetValueFromOptimovePlist("optimoveMobileCredentials");
        if (optimobileCredentials == null){
            return;
        }

        string extensionTargetName = "OptimoveUnityNotificationServiceExtension";
        string extensionGuid = project.TargetGuidByName(extensionTargetName);
        if (!string.IsNullOrEmpty(extensionGuid)){
            return;
        }

        string extensionFilesPath = Path.Combine(Application.dataPath, ".OptimoveNativeAssets", "NotificationServiceExtension");

        // add plist
        string extensionPath = Path.Combine(pathToBuiltProject, extensionTargetName);
        Directory.CreateDirectory(extensionPath);
        string plistPath = Path.Combine(extensionPath, "Info.plist");
        PlistDocument notificationServicePlist = new PlistDocument();
        notificationServicePlist.ReadFromFile(Path.Combine(extensionFilesPath, "Info.plist"));
        notificationServicePlist.WriteToFile(plistPath);

        // add target
        extensionGuid = project.AddAppExtension(mainTargetGuid,
            extensionTargetName,
            PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS) + "." + extensionTargetName,
            extensionTargetName + "/Info.plist"
        );

        // add source file
        string notificationSourceFileName = "NotificationService.swift";
        string sourcePath = Path.Combine(extensionFilesPath, notificationSourceFileName);
        string destPathRelative = Path.Combine(extensionTargetName, notificationSourceFileName);

        string destPath = Path.Combine(pathToBuiltProject, destPathRelative);
        if (!File.Exists(destPath)){
            FileUtil.CopyFileOrDirectory(sourcePath.Replace("\\", "/"), destPath.Replace("\\", "/"));
        }

        string sourceFileGuid = project.AddFile(destPathRelative, destPathRelative);

        string buildPhaseId = project.AddSourcesBuildPhase(extensionGuid);
        project.AddFileToBuildSection(extensionGuid, buildPhaseId, sourceFileGuid);

        // settings
        project.SetBuildProperty(extensionGuid, "IPHONEOS_DEPLOYMENT_TARGET", "10.0");
        project.SetBuildProperty(extensionGuid, "SWIFT_VERSION", "5.0");
        project.SetBuildProperty(extensionGuid, "TARGETED_DEVICE_FAMILY", "1,2");
        project.SetBuildProperty(extensionGuid, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);

        project.WriteToFile(projectPath);

        // add capabilities + entitlements
        string entitlementsPath = GetEntitlementsPath(project, extensionGuid, extensionTargetName, pathToBuiltProject, true);
        var projCapability = new ProjectCapabilityManager(projectPath, entitlementsPath, extensionTargetName);

        projCapability.AddAppGroups(new[] { _appGroupName });

        projCapability.WriteToFile();

        //xcframework
        AddXcframework(project, pathToBuiltProject, mainTargetGuid, extensionGuid, "OptimoveNotificationServiceExtension.xcframework");
    }
}