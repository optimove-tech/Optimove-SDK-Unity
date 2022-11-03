# Integration summaries

For detailed instructions refer to docs.

## For a new project

1. create a new project
2. switch platform: Android
3. set bundle id (Player Settings -> Other Settings)
4. import OptimoveSDKUnity package
5. add OptimoveInit to MainCamera
6. in Player Settings verify (it should be set automatically):
    - custom gradle main template set to `Plugins/Android/mainTemplate.gradle`,
    - custom gradle launcher template set to `Plugins/Android/launcherTemplate.gradle`
    - custom gradle base template set to `Plugins/Android/baseProjectTemplate.gradle`
    - custom main manifest set to `Plugins/Android/AndroidManifest.xml`.
    - for 2020.3+ make sure custom gradle properties template set to `Plugins/Android/gradleTemplate.properties`
7. If using push uncomment relevant lines in gradle templates

Then follow instructions below

## For the ExampleApp project

8. Create `Assets/Plugins/optimove.json`, set credentials
9. If using push push add `google-services.json` to `Assets/google-services.json`
10. (Optional) Export project

# Internal

This is only relevant for Optimove developers

- The `AndroidPlugin` folder should be opened in Android Studio to work on the Android proxy classes. This project can be run on device and can receive push (need to add google-services.json to the app/ folder)
- The proxy is shipped in the SDK as an AAR, built with `./gradlew OptimoveUnityPlugin:assembleRelease`. Created file is `AndroidPlugin/OptimoveUnityPlugin/build/outputs/aar/OptimoveUnityPlugin.aar`
- Copy the proxy AAR into `Assets/Plugins/Android`
- We also ship gradle templates and AndroidManifest template with the Android plugin, see files in `Assets/Plugins/Android`
- The manifest is necessary to keep the Application class registered
- When testing, choose the Android build platform, and tick `Export Project` to generate an Android Studio project