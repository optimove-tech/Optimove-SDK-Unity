This is only relevant for Optimove developers

# ================= Android =================

- The `AndroidPlugin` folder should be opened in Android Studio to work on the Android proxy classes. This project can be run on device and can receive push (need to add google-services.json to the app/ folder). Project has 2 libs. `classes.jar` are unity classes missing UnityPlayerActivity. So, to extend the activity (for DDL) we add `UnityActivity.aar` with the copied UnityPlayerActivity.
- The proxy is shipped in the SDK as an AAR, built with `./gradlew OptimoveUnityPlugin:assembleRelease`. Created file is `AndroidPlugin/OptimoveUnityPlugin/build/outputs/aar/OptimoveUnityPlugin.aar`
- Copy the proxy AAR into `Assets/Plugins/Android`
- We also ship gradle templates and AndroidManifest template with the Android plugin, see files in `Assets/Plugins/Android`
- The manifest is necessary to keep the Application class registered
- When testing, choose the Android build platform, and tick `Export Project` to generate an Android Studio project

# =================== iOS ===================

## Overview

- iOS project created by Unity uses xcframeworks built by the Optimive-SDK-iOS (swift). Created xcframeworks ensure Module Stability and swift 5+ provides ABI stability
- The proxy uses extern C functions to communicate with the C# layer (as per Unity plugin documentation). See `OptimovePluginInterface.h`
- Delegate's swizzling and observer addition is in `UnityAppController+Optimove.m`
- User-level config resides in `optimove.json`, turned into `optimove.plist` in `OnPreprocessBuild` and added to the iOS project in `PostProcessBuild`
- In `PostProcessBuild` we override modulemap for UnityFramework exposing objc wrapper functions to swift. See `Swift-objc-bridging-header.h`. You cannot add bridging header to a framework, and we are avoiding modifying umbrella header. All plugin files belong to the Unity framework.

## Creating xcframeworks

- If Swift SDK was updated with new files, make sure these files are added to dynamic framework targets.
- Create `OptimoveSDK.xcframework`, `OptimoveSDKCore.xcframework` and `OptimoveNotificationServiceExtension.xcframework` from Optimive-SDK-iOS project's combined targets.

## ================= Workflow ==================

- Make changes in ExampleApp
- When ready port changes to `unity-sdk`
- To create `.unitypackage` open `unity-sdk` project and export `Optimove-*.*.*.unitypackage`
- `.OptimoveNativeAssets` folder is shipped separately alongside `.unitypackage`