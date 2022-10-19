import OptimoveSDK
import Foundation

enum InAppConsentStrategy: String {
    case autoEnroll = "auto-enroll"
    case explicitByUser = "explicit-by-user"
    case disabled = "in-app-disabled"
}

typealias InboxSummaryResultHandler = ([AnyHashable : Any]) -> Void

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
            "optimoveCredentials": "",
            "optimoveMobileCredentials": "<yours>",
            "optimoveInAppConsentStrategy": "explicit-by-user"
        ]

        guard let builder = getConfigBuilder(configValues: configValues) else{
            return
        };

        builder.setPushOpenedHandler(pushOpenedHandlerBlock: { notification in
            let parsedPush = getPushNotificationMap(pushNotification: notification)

            OptimoveCallUnityPushOpened(parsedPush);
        })

        if #available(iOS 10, *) {
            builder.setPushReceivedInForegroundHandler(pushReceivedInForegroundHandlerBlock: { notification, completionHanlder in
                let parsedPush = getPushNotificationMap(pushNotification: notification)

                OptimoveCallUnityPushReceived(parsedPush);

                completionHanlder(UNNotificationPresentationOptions.alert)
            })
        }

        switch(configValues[inAppConsentStrategy]){
            case InAppConsentStrategy.autoEnroll.rawValue:
                builder.enableInAppMessaging(inAppConsentStrategy:OptimoveSDK.InAppConsentStrategy.autoEnroll);
                break
            case InAppConsentStrategy.explicitByUser.rawValue:
                builder.enableInAppMessaging(inAppConsentStrategy:OptimoveSDK.InAppConsentStrategy.explicitByUser);
                break
            case InAppConsentStrategy.disabled.rawValue:
                break
            default:
                print("Invalid inApp consent strategy")
                return
        }

        builder.setInAppDeepLinkHandler(inAppDeepLinkHandlerBlock: { data in
            let parsedButtonPress: [String : Any] = getInappButtonPressMap(inAppButtonPress: data)

            OptimoveCallUnityInAppDeepLinkPressed(parsedButtonPress);
        })

        overrideInstallInfo(builder: builder, unityVersion:unityVersion)

        let config = builder.build()

        Optimove.initialize(with: config)

        OptimoveInApp.setOnInboxUpdated(inboxUpdatedHandlerBlock: {
            OptimoveCallUnityInAppInboxUpdated()
        })
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

    // ========================== MESSAGING ==========================

    @objc(pushRegister)
    static func pushRegister() {
        Optimove.shared.pushRequestDeviceToken()
    }

    @objc(pushUnregister)
    static func pushUnregister() {
        Optimove.shared.pushUnregister()
    }

    @objc(inAppUpdateConsent:)
    static func inAppUpdateConsent(consented: Bool) {
        OptimoveInApp.updateConsent(forUser: consented)
    }

    @objc(inAppGetInboxItems)
    static func inAppGetInboxItems() -> [[String: Any]] {

        let inboxItems = OptimoveInApp.getInboxItems()
        var items = [[String : Any]]()

        let formatter = ISO8601DateFormatter()
        formatter.timeZone = TimeZone(secondsFromGMT: 0)

        for item in inboxItems {
            let dict: [String: Any] = [
                "id": item.id,
                "title": item.title,
                "subtitle": item.subtitle,
                "availableFrom": item.availableFrom != nil ? formatter.string(from: item.availableFrom!) : NSNull(),
                "availableTo": item.availableTo != nil ? formatter.string(from: item.availableTo!) : NSNull(),
                "dismissedAt": item.dismissedAt != nil ? formatter.string(from: item.dismissedAt!) : NSNull(),
                "isRead": item.isRead(),
                "sentAt": formatter.string(from: item.sentAt),
                "imageUrl": item.getImageUrl()?.absoluteString ?? NSNull(),
                "data": item.data ?? NSNull()
            ]

            items.append(dict)
        }

        return items
    }

    @objc(inAppPresentInboxMessage:)
    static func inAppPresentInboxMessage(messageId: Int64) -> String {
        let inboxItems = OptimoveInApp.getInboxItems()

        var presentationResult: InAppMessagePresentationResult = .FAILED
        for msg in inboxItems {
            if (msg.id != messageId){
                continue
            }

            presentationResult = OptimoveInApp.presentInboxMessage(item: msg)

            break;
        }

        return presentationResult.rawValue
    }

    @objc(inAppDeleteMessageFromInbox:)
    static func inAppDeleteMessageFromInbox(messageId: Int64) -> Bool {
        let inboxItems = OptimoveInApp.getInboxItems()

        var result = false
        for msg in inboxItems {
            if msg.id != messageId {
                continue
            }

            result = OptimoveInApp.deleteMessageFromInbox(item: msg)

            break
        }

        return result
    }

    @objc(inAppMarkAsRead:)
    static func inAppMarkAsRead(messageId: Int64) -> Bool {
      let inboxItems = OptimoveInApp.getInboxItems()

      var result = false
      for msg in inboxItems {
          if msg.id != messageId {
              continue
          }

          result = OptimoveInApp.markAsRead(item: msg)
          break
      }

      return result
    }

    @objc(inAppMarkAllInboxItemsAsRead)
    static func inAppMarkAllInboxItemsAsRead() -> Bool {
        return OptimoveInApp.markAllInboxItemsAsRead()
    }

    @objc(inAppGetInboxSummary:handler:)
    static func inAppGetInboxSummary(guid: String, handler: @escaping InboxSummaryResultHandler) {
        OptimoveInApp.getInboxSummaryAsync { summary in
            var dict: [String: Any] = [
                "guid": guid,
                "success": false
            ]

            if let summary = summary {
                dict["totalCount"] = summary.totalCount
                dict["unreadCount"] = summary.unreadCount
                dict["success"] = true
            }

            handler(dict)
        }
    }


    private static func getPushNotificationMap(pushNotification: PushNotification) -> [String: Any] {
        let aps: [AnyHashable:Any] = pushNotification.aps
        var alert: [String: String] = [:]
        if let a = aps["alert"] as? Dictionary<String, String> {
            alert = a
        }

        let title: String? = alert["title"] ?? nil
        let message: String? = alert["body"] ?? nil

        let dict: [String: Any] = [
            "id": pushNotification.id,
            "title": title ?? NSNull(),
            "message": message ?? NSNull(),
            "data": pushNotification.data,
            "url": pushNotification.url?.absoluteString ?? NSNull(),
            "actionId": pushNotification.actionIdentifier ?? NSNull()
        ]

        return dict
    }

    private static func getInappButtonPressMap(inAppButtonPress: InAppButtonPress) -> [String: Any] {
        let dict: [String: Any] = [
            "deepLinkData": inAppButtonPress.deepLinkData,
            "messageData": inAppButtonPress.messageData ?? NSNull(),
            "messageId": inAppButtonPress.messageId
        ]

        return dict
    }
}