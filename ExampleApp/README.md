### ExampleApp project

This project
- Serves as an example of a unity project mostly integrated with Optimove SDK (except credentials)
- Provides a UI to test various bits of the SDK functionality.

To finish the integration follow the steps below.

#### ================= Android =================

1. Create `Assets/OptimoveConfigFiles/optimove.json`, set credentials
2. If using push push add `google-services.json` to `Assets/OptimoveConfigFiles/google-services.json`
3. (Optional) Export project


#### ================= iOS =================

1. Create `Assets/OptimoveConfigFiles/optimove.json`, set credentials
2. Move `Artifacts/OptimoveNativeAssets~` to `Assets` folder
3. build project. Depending on values in `optimove.json` this automatically adds capabilities, NotificationServiceExtension and sets up Xcode project.

##### In Xcode project

4. signing (if was not set in Unity Player Settings)
5. Switch to New Build System in File -> Project Settings -> Build System (likely already on). This is necessary to use xcframeworks.