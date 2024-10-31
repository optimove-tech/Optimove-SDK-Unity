# Optimove-SDK-Unity
![GitHub](https://img.shields.io/github/license/optimove-tech/Optimove-SDK-Unity?style=flat-square)

## Unity Version And Platform Support

Optimove Unity SDK supports Unity >= 2019.3. Supported platforms are iOS and Android.

## Integration Guide

[Developer Docs](https://developer.optimove.com/docs/optimove-sdk-unity)

## Repository structure

This repository contains 3 folders:
1. `Artifacts` has everything necessary to integrate with Optimove Unity SDK. This includes `Optimove.unitypackage` and `OptimoveNativeAssets~` required if integrating with iOS.
2. `unity-sdk` contains a Unity project used to create `Optimove.unitypackage`.
3. `ExampleApp` is an example project with imported `Optimove.unitypackage`. Check `ExampleApp/README.md` if you want to run it.


#### ================= Android =================

1. create a new project
2. switch platform: Android
3. set package id (Player Settings -> Other Settings)
4. import OptimoveSDKUnity package
5. add OptimoveInit to MainCamera
6. in Player Settings verify (it should be set automatically):
    - custom gradle main template set to `Plugins/Android/mainTemplate.gradle`,
    - custom gradle launcher template set to `Plugins/Android/launcherTemplate.gradle`
    - custom gradle base template set to `Plugins/Android/baseProjectTemplate.gradle`
    - custom main manifest set to `Plugins/Android/AndroidManifest.xml`.
    - for 2020.3+ make sure custom gradle properties template set to `Plugins/Android/gradleTemplate.properties`
7. If using push/ddl uncomment relevant lines in gradle templates and `AndroidManifest.xml`
8. Create `Assets/OptimoveConfigFiles/optimove.json`, set credentials
9. If using push push add `google-services.json` to `Assets/OptimoveConfigFiles/google-services.json`
10. (Optional) Export project


#### ================= iOS =================

1. create a new project
2. switch platform to ios
3. set bundle id (Player Settings -> Other Settings)
4. import OptimoveSDKUnity package
5. add OptimoveInit to MainCamera
6. Create `Assets/OptimoveConfigFiles/optimove.json`, set credentials
7. Move `Artifacts/OptimoveNativeAssets~` to `Assets` folder
8. build project. Depending on values in `optimove.json` this automatically adds capabilities, NotificationServiceExtension and sets up Xcode project.

##### In Xcode project

9. signing (if was not set in Unity Player Settings)
10. Switch to New Build System in File -> Project Settings -> Build System (likely already on). This is necessary to use xcframeworks.

# License

Optimove SDK for Unity is available under the [MIT license](LICENSE.md).
