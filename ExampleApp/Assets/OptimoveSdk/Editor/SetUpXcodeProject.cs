using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class SetUpXcodeProject
{
    private static readonly string _appGroupName = $"group.{PlayerSettings.applicationIdentifier}.optimove";

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
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

        var unityTarget = project.GetUnityFrameworkTargetGuid();

        //copy optimove.plist as *.plist files are not copied automatically
        AddOptimoveConfig(project, pathToBuiltProject, unityTarget);

        // Push Notifications, Background Modes, App Groups for the main target
        SetupMainTargetCapabilities(project, projectPath, pathToBuiltProject);

        // enables calling objc functions from swift
        SetModuleMap(project, unityTarget, pathToBuiltProject);

        project.WriteToFile(projectPath);
    }

    private static void AddOptimoveConfig(PBXProject project, string pathToBuiltProject, string unityTargetGuid)
    {
        var srcPath = "Assets/Plugins/iOS/optimove.plist";
        var dstLocalPath = "Libraries/Plugins/iOS/optimove.plist";
        var dstPath = Path.Combine(pathToBuiltProject, dstLocalPath);
        File.Copy(srcPath, dstPath, true);
        project.AddFileToBuild(unityTargetGuid, project.AddFile(dstLocalPath, dstLocalPath));
    }

    private static void SetupMainTargetCapabilities(PBXProject project, string projectPath, string pathToBuiltProject) {
        //2019.3+ only
        var mainTargetGuid = project.GetUnityMainTargetGuid();
        var mainTargetName = "Unity-iPhone";

        var entitlementsPath = GetEntitlementsPath(project, mainTargetGuid, mainTargetName, pathToBuiltProject);
        var projCapability = new ProjectCapabilityManager(projectPath, entitlementsPath, mainTargetName);

        projCapability.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications | BackgroundModesOptions.BackgroundFetch);

        projCapability.AddPushNotifications(false);
        projCapability.AddAppGroups(new[] { _appGroupName });

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
}
