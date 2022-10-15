import OptimoveSDK

@objc(Optimove_Unity) class OptimoveSDKPlugin: NSObject {

    private static let optimoveCredentialsKey = "optimoveCredentials"
    private static let optimoveMobileCredentialsKey = "optimoveMobileCredentials"
    private static let inAppConsentStrategy = "optimoveInAppConsentStrategy"
    private static let enableDeferredDeepLinking = "optimoveEnableDeferredDeepLinking"

    private static let sdkVersion = "1.0.0"
    private static let sdkTypeOptimoveUnity = 108
    private static let runtimeTypeUnity = 6

    // ========================== INITIALIZATION ==========================

    @objc(didFinishLaunching:unityVersion:)
    static func didFinishLaunching(notification: Notification, unityVersion: String) {

        let configValues = [
            "optimoveCredentials": "<yours>",
            "optimoveMobileCredentials": "<yours>"
        ]

        guard let builder = getConfigBuilder(configValues: configValues) else{
            return
        };

        overrideInstallInfo(builder: builder, unityVersion:unityVersion)

        let config = builder.build()

        Optimove.initialize(with: config)
    }

    static func overrideInstallInfo(builder: OptimoveConfigBuilder, unityVersion: String) -> Void {
        let runtimeInfo: [String : AnyObject] = [
            "id": runtimeTypeUnity as AnyObject,
            "version": unityVersion as AnyObject,
        ]

        let sdkInfo: [String : AnyObject] = [
            "id": sdkTypeOptimoveUnity as AnyObject,
            "version": sdkVersion as AnyObject,
        ]

        builder.setRuntimeInfo(runtimeInfo: runtimeInfo);
        builder.setSdkInfo(sdkInfo: sdkInfo);

        var isRelease = true
        #if DEBUG
            isRelease = false
        #endif
        builder.setTargetType(isRelease: isRelease);
    }

    static func getConfigBuilder(configValues: [String: String]) -> OptimoveConfigBuilder? {
        var optimoveCredentials: String? = nil
        if let val = configValues[optimoveCredentialsKey] {
            if (!val.isEmpty){
                optimoveCredentials = val;
            }
        }

        var optimobileCredentials: String? = nil
        if let val = configValues[optimoveMobileCredentialsKey] {
            if (!val.isEmpty){
                optimobileCredentials = val;
            }
        }

        let builder = OptimoveConfigBuilder(optimoveCredentials: optimoveCredentials, optimobileCredentials: optimobileCredentials)

        return builder
    }



    // ========================== ASSOCIATION AND EVENTS ==========================

    @objc(reportEvent:parameters:)
    static func reportEvent(type: String, parameters: [String:Any]?) {
        Optimove.shared.reportEvent(name: type, parameters: parameters ?? [:])
    }

    @objc(reportScreenVisit:screenCategory:)
    static func reportScreenVisit(screenTitle: String, screenCategory: String?) {
        Optimove.shared.reportScreenVisit(screenTitle: screenTitle, screenCategory: screenCategory)
    }

    @objc(registerUser:email:)
    static func registerUser(userId: String, email: String) {
        Optimove.shared.registerUser(sdkId: userId, email: email)
    }

    @objc(setUserId:)
    static func setUserId(userId: String) {
        Optimove.shared.setUserId(userId)
    }

    @objc(setUserEmail:)
    static func setUserEmail(email: String) {
        Optimove.shared.setUserEmail(email: email)
    }

    @objc(getVisitorId)
    static func getVisitorId() -> String? {
        Optimove.getVisitorID()
    }

    @objc(signOutUser)
    static func signOutUser() {
        Optimove.signOutUser()
    }
}
