## Android

- The `AndroidPlugin` folder should be opened in Android Studio to work on the Android proxy classes. This project can be run on device and can receive push (need to add google-services.json to the app/ folder)
- The proxy is shipped in the SDK as an AAR, built with `./gradlew OptimoveUnityPlugin:assembleRelease`. Created file is `AndroidPlugin/OptimoveUnityPlugin/build/outputs/aar/OptimoveUnityPlugin.aar`
- Copy the proxy AAR into `Assets/Plugins/Android`
- We also ship a gradle template and AndroidManifest template with the Android plugin, see files in `Assets/Plugins/Android`
- The gradle template adds our SDK dependency and allows suppressing dependency conflicts
- The manifest is necessary to keep the Application class registered
- When testing, choose the Android build platform, and tick `Export Project` to generate an Android Studio project


## Integration summaries

For detailed instructions refer to docs.

### Android

1. create a new project
2. switch platform: Android
3. set bundle id
4. import KumulosSDKUnity package
5. add KumulosInit to MainCamera, set credentials, register for push
6. in Player Settings verify (it's set automatically) custom gradle mainTemplate set to `plugins/android/mainTemplate.gradle`. For unity 2019.3+ make sure launcherTemplate is set to `plugins/android/launcherTemplate.gradle`. Main Manifest must be set to `plugins/android/AndroidManifest.xml`. For 2020.3+ make sure custom gradle properties template set to `plugins/android/gradleTemplate.properties`.
7. uncomment lines in `plugins/android/mainTemplate.gradle`
8. import FirebaseMessaging package. You only need it for android, so, when importing untick iOS related plugin files located in `Plugins/iOS/Firebase` (including Firebase static libraries may result in iOS build errors if switch to iOS). Additionally, if Android Studio suggests to enable Android Auto-resolution, choose 'Disable'. Otherwise it may load Firebase dependencies as jars, and classes from jars may conflict with Kumulos gradle dependencies. Sync project. If auto-resolution was enabled by default, can delete conflicting jars manually.
9. Importing FirebaseMessaging package overrides AndroidManifest. Delete the file and rename FirebaseAndroidManifest -> AndroidManifest. FirebaseAndroidManifest contains correctly merged kumulos and Firebase plugin manifests. Make sure this file is set as Main Manifest.
10. add google-services.json to the Assets folder, ensure proper bundle id
11. export project
12. Add MainApplication, check set in manifest. For unity 2019.4 MainApplication has to be added to unityLibrary module
13. ~~fix minSdk errors in Firebase/AndroidManifest.xml(remove uses-sdk), build.gradle for Firebase module, e.g. `defaultConfig {minSdkVersion 16, targetSdkVersion 29}`~~
