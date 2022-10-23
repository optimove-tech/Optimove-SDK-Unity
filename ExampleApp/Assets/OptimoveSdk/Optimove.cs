using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;

namespace OptimoveSdk
{
    public class Optimove : MonoBehaviour
    {
        private const string GameObjectName = "OptimoveSdkGameObject";

        public const string Version = "1.0.0";

        public delegate void PushReceivedDelegate(PushMessage message);
        public event PushReceivedDelegate OnPushReceived;

        public delegate void PushOpenedDelegate(PushMessage message);
        public event PushOpenedDelegate OnPushOpened;

        public delegate void InAppDeepLinkDelegate(InAppButtonPress press);
        public event InAppDeepLinkDelegate OnInAppDeepLinkPressed;

        public delegate void InAppInboxUpdatedDelegate();
        public event InAppInboxUpdatedDelegate OnInAppInboxUpdated;

        public delegate void DeepLinkResolvedDelegate(DeepLink ddl);
        public event DeepLinkResolvedDelegate OnDeepLinkResolved;

        #region Statics

        public static Optimove Shared
        {
            get;
            private set;
        }

#if UNITY_ANDROID
        private static AndroidJavaClass AndroidProxy;
#endif

        public static void Initialize()
        {
#if UNITY_ANDROID
        AndroidProxy = new AndroidJavaClass("com.optimove.unity.plugin.UnityProxy");
#endif

            var optimoveGameObject = new GameObject(GameObjectName);
            optimoveGameObject.AddComponent<Optimove>();
            DontDestroyOnLoad(optimoveGameObject);
        }

        #endregion

        #region MonoBehaviour events

        void Awake()
        {
                Optimove.Shared = this;

// #if UNITY_ANDROID
//             PollPendingPush();
// #endif

        }

        #endregion

        #region Helper Functions
        private bool isValidString(string value)
        {
            return value != null && !value.Equals("");
        }
        #endregion

        #region User Association

        public void SetUserId(string userId)
        {
                if (!isValidString(userId)){
                        Debug.LogError("Invalid user id");
                        return;
                }

                #if UNITY_IOS
                        OptimoveSetUserId(userId);
                #elif UNITY_ANDROID
                        //AndroidProxy.CallStatic("setUserId", userId);

                #endif
        }

        public void SetUserEmail(string userEmail)
        {
                if (!isValidString(userEmail)){
                        Debug.LogError("Invalid user email");
                        return;
                }

                #if UNITY_IOS
                        OptimoveSetUserEmail(userEmail);
                #elif UNITY_ANDROID
                       //AndroidProxy.CallStatic("setUserEmail", userEmail);

                #endif
        }

        public void RegisterUser(string userId, string userEmail)
        {
                if (!isValidString(userId) || !isValidString(userEmail)){
                        Debug.LogError("Invalid user id or user email");
                        return;
                }

                #if UNITY_IOS
                        OptimoveRegisterUser(userId, userEmail);
                #elif UNITY_ANDROID
                       //AndroidProxy.CallStatic("registerUser", userId, userEmail);
                #endif
        }

        public string GetVisitorId()
        {
                #if UNITY_IOS
                        return OptimoveGetVisitorId();
                #elif UNITY_ANDROID
                        //TODO:
                #endif
        }


        public void SignOutUser()
        {
                #if UNITY_IOS
                        OptimoveSignOutUser();
                #elif UNITY_ANDROID
                        //TODO:
                #endif
        }

        #endregion


        #region Event Tracking

        public void ReportScreenVisit(string screenName, string screenCategory)
        {
                if (!isValidString(screenName)){
                        Debug.LogError("Invalid screen name");
                        return;
                }

                if (screenCategory != null && screenCategory.Equals("")){
                        Debug.LogError("Invalid screen category");
                        return;
                }

                #if UNITY_IOS
                        OptimoveReportScreenVisit(screenName, screenCategory);
                #elif UNITY_ANDROID
                       //TODO
                #endif
        }

        public void ReportEvent(string eventType, Dictionary<string, object> properties)
        {
                if (!isValidString(eventType)){
                        Debug.LogError("Invalid event type");
                        return;
                }

                string propsJson = null;

                if (properties != null) {
                        propsJson = MiniJSON.Json.Serialize(properties);
                }

                #if UNITY_IOS
                        OptimoveReportEvent(eventType, propsJson);
                #elif UNITY_ANDROID
                        //TODO

                #endif
        }

        #endregion

        #region DDL

        public void DeepLinkResolved(string message)
        {
                if (OnDeepLinkResolved == null){
                        return;
                }

                var ddl = DeepLink.CreateFromJson(message);

                OnDeepLinkResolved(ddl);
        }

        #endregion

        #region Push

        public void PushRegister()
        {
                #if UNITY_IOS
                        OptimoveUpdatePushRegistration(1);
                #elif UNITY_ANDROID
                        //AndroidProxy.CallStatic("pushRegUnreg", new object[] { true });
                #endif
        }

        public void PushUnregister()
        {
                #if UNITY_IOS
                        OptimoveUpdatePushRegistration(0);
                #elif UNITY_ANDROID
                        //TODO
                #endif
        }


        public void PushReceived(string message)
        {
            if (OnPushReceived == null)
            {
                return;
            }

            var push = PushMessage.CreateFromJson(message);

            OnPushReceived(push);
        }

        public void PushOpened(string message)
        {
                if (OnPushOpened == null){
                        return;
                }

                var push = PushMessage.CreateFromJson(message);

                OnPushOpened(push);
        }


        #endregion

        #region InApp

        public void InAppUpdateConsent(bool consented)
        {
                #if UNITY_IOS
                        if (consented) {
                                OptimoveInAppUpdateConsentForUser(1);
                        } else {
                                OptimoveInAppUpdateConsentForUser(0);
                        }
                #elif UNITY_ANDROID
                        //TODO
                #endif
        }

        public List<InAppInboxItem> InAppGetInboxItems()
        {
                string json = "[]";
                #if UNITY_IOS
                        json = OptimoveInAppGetInboxItems();
                #elif UNITY_ANDROID
                        //json = AndroidProxy.CallStatic<string>("inAppGetInboxItems", new object[] { });
                #endif

                return InAppInboxItem.ListFromJson(json);
        }

        public OptimoveInAppPresentationResult InAppPresentInboxMessage(InAppInboxItem item)
        {
                #if UNITY_IOS
                        string result = OptimoveInAppPresentInboxMessage(item.Id);

                #elif UNITY_ANDROID
                        //return AndroidProxy.CallStatic<bool>("inAppPresentInboxMessage", new object[] { item.Id });
                #else
                        return OptimoveInAppPresentationResult.Failed;
                #endif

                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                result = info.ToTitleCase(result);

                return (OptimoveInAppPresentationResult) Enum.Parse(typeof(OptimoveInAppPresentationResult), result, true);
        }

        public bool InAppDeleteMessageFromInbox(InAppInboxItem item)
        {
                #if UNITY_IOS
                        return OptimoveInAppDeleteMessageFromInbox(item.Id);
                #elif UNITY_ANDROID
                        //return AndroidProxy.CallStatic<bool>("inAppDeleteMessageFromInbox", new object[] { item.Id });
                #else
                        return false;
                #endif
        }

        public bool InAppMarkAsRead(InAppInboxItem item)
        {
                #if UNITY_IOS
                        return OptimoveInAppMarkAsRead(item.Id);
                #elif UNITY_ANDROID
                        //return AndroidProxy.CallStatic<bool>("inAppMarkInboxItemRead", new object[] { item.Id });
                #else
                        return false;
                #endif
        }


        public bool InAppMarkAllInboxItemsRead()
        {
                #if UNITY_IOS
                        return OptimoveMarkAllInboxItemsAsRead();
                #elif UNITY_ANDROID
                        //return AndroidProxy.CallStatic<bool>("inAppMarkAllInboxItemsRead", new object[] { });
                #else
                        return false;
                #endif
        }

        public void InAppDeepLinkPressed(string dataJson)
        {
                if (OnInAppDeepLinkPressed == null)
                {
                        return;
                }

                var press = InAppButtonPress.CreateFromJson(dataJson);

                OnInAppDeepLinkPressed(press);
        }

        public void InAppInboxUpdated()
        {
            if (OnInAppInboxUpdated == null)
            {
                return;
            }

            OnInAppInboxUpdated();
        }


        //**************************************** SUMMARY ************************************************
        private static Dictionary<string, Action<InAppInboxSummary>> inboxSummaryHandlers = new Dictionary<string, Action<InAppInboxSummary>>();

        public void GetInboxSummaryAsync(Action<InAppInboxSummary> inboxSummaryHandler) {
                string guid = this.CacheInboxSummaryHandler(inboxSummaryHandler);
                #if UNITY_IOS
                        OptimoveInAppGetInboxSummary(guid);
                #elif UNITY_ANDROID
                        //AndroidProxy.CallStatic("inAppGetInboxSummary", new object[] { guid });
                #endif
        }

        private string CacheInboxSummaryHandler(Action<InAppInboxSummary> inboxSummaryHandler)
        {
            string guid = Guid.NewGuid().ToString();
            while(inboxSummaryHandlers.ContainsKey(guid)){
                guid = Guid.NewGuid().ToString();
            }

            inboxSummaryHandlers.Add(guid, inboxSummaryHandler);

            return guid;
        }

        private void InvokeInboxSummaryHandler(string resultJson)
        {
            var parsed = MiniJSON.Json.Deserialize(resultJson) as Dictionary<string, object>;
            if (parsed == null){
                return;
            }

            string guid = parsed["guid"] as string;
            if (!inboxSummaryHandlers.ContainsKey(guid)){
                return;
            }

            bool success = (bool)parsed.GetValueOrDefault("success");
            InAppInboxSummary summary = null;
            if (success){
                summary = InAppInboxSummary.CreateFromDictionary(parsed);
            }

            inboxSummaryHandlers[guid](summary);
            inboxSummaryHandlers.Remove(guid);
        }

        //************************************************************************************************


        #endregion

        #region Native

#if UNITY_IOS
        private const string nativeLib = "__Internal";

        [DllImport(nativeLib)]
        private static extern void OptimoveReportEvent(string type, string jsonData);

        [DllImport(nativeLib)]
        private static extern void OptimoveReportScreenVisit(string screenName, string screenCategory);

        [DllImport(nativeLib)]
        private static extern void OptimoveRegisterUser(string userId, string email);

        [DllImport(nativeLib)]
        private static extern void OptimoveSetUserId(string userId);

        [DllImport(nativeLib)]
        private static extern void OptimoveSetUserEmail(string email);

        [DllImport(nativeLib)]
        private static extern string OptimoveGetVisitorId();

        [DllImport(nativeLib)]
        private static extern void OptimoveSignOutUser();

        [DllImport(nativeLib)]
        private static extern void OptimoveUpdatePushRegistration(int state);

        [DllImport(nativeLib)]
        private static extern void OptimoveInAppUpdateConsentForUser(int consented);

        [DllImport(nativeLib)]
        private static extern string OptimoveInAppGetInboxItems();

        [DllImport(nativeLib)]
        private static extern string OptimoveInAppPresentInboxMessage(long messageId);

        [DllImport(nativeLib)]
        private static extern bool OptimoveInAppDeleteMessageFromInbox(long messageId);

        [DllImport(nativeLib)]
        private static extern bool OptimoveInAppMarkAsRead(long messageId);

        [DllImport(nativeLib)]
        private static extern bool OptimoveMarkAllInboxItemsAsRead();

        [DllImport(nativeLib)]
        private static extern void OptimoveInAppGetInboxSummary(string guid);

#endif

//         private static void PollPendingPush()
//         {
// #if UNITY_ANDROID
//             AndroidProxy.CallStatic("pollPendingPush", new object[] { });
// #endif
//         }

        #endregion

    }

}
