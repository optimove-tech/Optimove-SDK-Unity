# Optimove-SDK-Unity
![GitHub](https://img.shields.io/github/license/optimove-tech/Optimove-SDK-Unity?style=flat-square)

# Integration Guide

In this guide we will discuss the steps to integrate the Optimove Unity SDK with your application.

### Setup

1. [Initializing the SDK](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/Initializing-the-sdk)
2. [Tracking customers](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/Tracking-customers)
3. [Tracking events](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/Tracking-events)

### Mobile Messaging

1. [Push Setup](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/push-setup)
2. [In-App Setup](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/in-app)
3. [Deferred Deep Linking](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/deferred-deep-linking)
4. [Testing](https://github.com/optimove-tech/Optimove-SDK-Unity/wiki/testing-troubleshooting)

### Integration summaries

This is a quick start integration guide. For detailed integration instructions refer to the above.

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