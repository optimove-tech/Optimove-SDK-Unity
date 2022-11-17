### Description of Changes

(briefly outline the reason for changes, and describe what's been done)

### Breaking Changes

- None

### Release Checklist

Prepare:

- [ ] Detail any breaking changes. Breaking changes require a new major version number, and a migration guide in wiki / README.md
- [ ] After update android proxy classes, rebuild `OptimoveUnityPlugin.aar`
- [ ] Make sure changes are present in both `ExampleApp` and `unity-sdk` projects
- [ ] When exporting `.unitypackage` from `unity-sdk` don't include dependencies, don't export scene

Bump versions in:

- [ ] `OptimoveSDKPlugin.swift`
- [ ] `OptimoveInitProvider.java`
- [ ] `Optimove-x.x.x.unitypackage` should have a name matching release version

### Release Procedure

- [ ] Squash and merge `dev` to `master`
- [ ] Create a tag matching release version
- [ ] Delete branch once merged