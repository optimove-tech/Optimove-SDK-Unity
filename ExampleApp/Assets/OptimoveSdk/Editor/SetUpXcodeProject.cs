using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

/*
 * This class sets up the Xcode native project for iOS with the necessary capabilities & dependencies for analytics, crash reporting, and push notifications.
 *
 */
public class SetUpXcodeProject
{

    private const string kCoreDataFramework = "CoreData.framework";
    private const string kUserNotificationsFramework = "UserNotifications.framework";

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        // if (buildTarget == BuildTarget.iOS)
        // {
        //     var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        //     var project = new PBXProject();

        //     project.ReadFromFile(projectPath);

        //     #if UNITY_2019_3_OR_NEWER
        //         var unityTarget = project.GetUnityFrameworkTargetGuid();
        //     #else
        //         var unityTarget = project.TargetGuidByName(PBXProject.GetUnityTargetName());
        //     #endif

        //     SetBuildProperties(project, unityTarget);
        //     LinkCoreData(project, unityTarget, pathToBuiltProject);
        //     SetupPushCapabilities(project, unityTarget, pathToBuiltProject);

        //     project.WriteToFile(projectPath);
        // }
    }

    // private static void SetupPushCapabilities(PBXProject project, string unityTarget, string pathToBuiltProject)
    // {
    //     if (!project.ContainsFramework(unityTarget, kUserNotificationsFramework))
    //     {
    //         project.AddFrameworkToProject(unityTarget, kUserNotificationsFramework, true);
    //     }

    //     project.AddCapability(unityTarget, PBXCapabilityType.PushNotifications);

    //     string plistPath = pathToBuiltProject + "/Info.plist";
    //     PlistDocument plist = new PlistDocument();
    //     plist.ReadFromFile(plistPath);

    //     PlistElementDict rootDict = plist.root;

    //     // Add our background mode
    //     var buildKey = "UIBackgroundModes";
    //     var backgroundModes = rootDict.CreateArray(buildKey);
    //     backgroundModes.AddString("fetch");
    //     backgroundModes.AddString("remote-notification");

    //     plist.WriteToFile(plistPath);
    // }

	// private static void LinkCoreData(PBXProject project, string unityTarget, string pathToBuiltProject)
    // {
    //     if (!project.ContainsFramework(unityTarget, kCoreDataFramework))
    //     {
    //         project.AddFrameworkToProject(unityTarget, kCoreDataFramework, false);
    //     }
    // }

    // private static void SetBuildProperties(PBXProject project, string unityTarget)
    // {
    //     project.AddBuildProperty(unityTarget, "OTHER_LDFLAGS", "-ObjC");
    //     project.AddBuildPropertyForConfig(project.BuildConfigByName(unityTarget, "Debug"), "GCC_PREPROCESSOR_DEFINITIONS[arch=*]", "DEBUG=1");
    // }
}
