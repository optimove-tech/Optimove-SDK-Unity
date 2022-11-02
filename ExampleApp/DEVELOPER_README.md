## iOS

- iOS project created by Unity uses xcframeworks built by the Optimive-SDK-iOS (swift). Created xcframeworks ensure Module Stability and swift 5+ provides ABI stability
- The proxy uses extern C functions to communicate with the C# layer (as per Unity plugin documentation). See `OptimovePluginInterface.h`
- Delegate's swizzling and observer addition is in `UnityAppController+Optimove.m`
- User-level config resides in `optimove.json`, turned into `optimove.plist` in `OnPreprocessBuild` and added to the iOS project in `PostProcessBuild`
- In `PostProcessBuild` we override modulemap for UnityFramework exposing objc wrapper functions to swift. See `Swift-objc-bridging-header.h`. You cannot add bridging header to a framework, and we are avoiding modifying umbrella header. All plugin files belong to the Unity framework.

### Creating xcframeworks

- If Swift SDK was updated with new files, make sure these files are added to dynamic framework targets.
- Create `OptimoveSDK.xcframework`, `OptimoveSDKCore.xcframework` and `OptimoveNotificationServiceExtension.xcframework` from Optimive-SDK-iOS project's combined targets.

## Integration summaries

For detailed instructions refer to docs. For development use ExampleApp project (1. and 4. are not needed then)

### iOS

#### For a new project

1. create a new project
2. switch platform to ios
3. set bundle id (Player Settings -> Other Settings)
4. import OptimoveSDKUnity package
5. add OptimoveInit to MainCamera

Then follow instructions below

#### For an existing project (ExampleApp)

1. Create `Assets/Plugins/optimove.json`, set credentials
2. build project

#### In Xcode project

3. signing
4. add extension (target 10, signing)
5. paste NotificationService.m contents (copy from docs)
6. Add App Groups capability to the extension target. Set group to `group.{bundleId}.optimove`
7. Switch to New Build System in File -> Project Settings -> Build System (likely already on). This is necessary to use xcframeworks.
8. Add `OptimoveSDK.xcframework` and `OptimoveSDKCore.xcframework` from UnitySDK to UnityFramework target (drag under frameworks, copy files). Select `Do Not Embed` option. Same way add `OptimoveNotificationServiceExtension.xcframework` from UnitySDK to the extension target. This is equivalent to adding the xcframeworks to `Link Binary With Libraries` section in `Build phases`.
9. Add `OptimoveSDK.xcframework`, `OptimoveSDKCore.xcframework` and `OptimoveNotificationServiceExtension.xcframework` from UnitySDK to the application target. Select `Embed and sign` option. This is equivalent to adding the xcframeworks to `Link Binary With Libraries` and `Embed Frameworks` sections in `Build phases`. Simulator builds (as chosen in unity) don't have general tab, add frameworks in Build phases.
> Note. 8. and 9. are explained in detail [here](https://docs.leanplum.com/changelog/unity-ios-xcode-123-and-new-build-system)

#### iOS Other

> Note. When exporting package, do not include `xcframeworks`. They are shipped separately alongside `.unitypackage`.
