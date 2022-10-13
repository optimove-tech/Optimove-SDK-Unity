using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Runtime.InteropServices;

    namespace OptimoveSdk
{
    public class Optimove : MonoBehaviour
    {
        private const string GameObjectName = "OptimoveSdkGameObject";

        public const string Version = "1.0.0";

        // public delegate void PushReceivedDelegate(PushMessage message);

        // public event PushReceivedDelegate OnPushReceived;

        // public delegate void InAppDeepLinkDelegate(Dictionary<string, object> message);

        // public event InAppDeepLinkDelegate OnInAppDeepLinkPressed;

        // public delegate void InAppInboxUpdatedDelegate();

        // public event InAppInboxUpdatedDelegate OnInAppInboxUpdated;

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

        //helper functions
        private bool isInvalidString(string value)
        {
            return (value == null) || value.Equals("");
        }

        // #region User Association

         public void SetUserId(string userId)
         {
            if (isInvalidString(userId))
            {
                Debug.LogError("invalid user id");
                return;
            }

            AndroidProxy.CallStatic("setUserId", userId);
         }

         public void SetUserEmail(string userEmail)
         {
            if (isInvalidString(userEmail))
            {
                Debug.LogError("invalid user email");
                return;
            }

            AndroidProxy.CallStatic("setUserEmail", userEmail);
         }

         public void RegisterUser(string userId, string userEmail)
         {
            if (isInvalidString(userId) || isInvalidString(userEmail))
            {
                Debug.LogError("invalid user id or user email");
            }
            AndroidProxy.CallStatic("registerUser", userId, userEmail);
         }

        // public void SignOutUser()
        // {
        //     //TODO
        // }

        // public string GetVisitorId()
        // {
        //     //TODO
        // }

        // #endregion


//         #region Event Tracking

//         public void ReportScreenVisit(string screenName, string screenCategory)
//         {
//             //TODO
//         }

//         public void ReportEvent(string eventType, Dictionary<string, object> properties)
//         {
//             string propsJson = null;

//             if (properties != null)
//             {
//                 propsJson = MiniJSON.Json.Serialize(properties);
//             }

// #if UNITY_IOS
//             KStrackEvent(eventType, propsJson);
// #elif UNITY_ANDROID
//             AndroidProxy.CallStatic("reportEvent", new object[] { eventType, propsJson});
// #else
// 			Debug.Log(String.Format("Optimove tracking event of type {0} (skipping on unsupported platform)", eventType));
// #endif
//         }

        // #region DDL

        // //TODO: set ddl handler

        // #endregion


//         #endregion

//         #region Push

//         public void PushRegister()
//         {
// #if UNITY_IOS
//             Optimove.KSPushRequestDeviceToken();
// #elif UNITY_ANDROID
//             AndroidProxy.CallStatic("pushRegUnreg", new object[] { true });
// #endif
//         }

//         public void PushUnregister()
//         {
//             //TODO:
//         }

//         public void PushReceived(string message)
//         {
//             if (OnPushReceived == null)
//             {
//                 return;
//             }

//             var push = PushMessage.CreateFromJson(message);

//             OnPushReceived(push);
//         }

    //TODO: PushOpened?

//         #endregion

//         #region InApp

//         public void InAppDeepLinkPressed(string dataJson)
//         {
//             if (OnInAppDeepLinkPressed == null)
//             {
//                 return;
//             }

//             var data = MiniJSON.Json.Deserialize(dataJson) as Dictionary<string, object>;

//             OnInAppDeepLinkPressed(data);
//         }

//         public void InAppInboxUpdated()
//         {
//             if (OnInAppInboxUpdated == null)
//             {
//                 return;
//             }

//             OnInAppInboxUpdated();
//         }

//         public void InAppUpdateConsent(bool consented)
//         {
// #if UNITY_IOS
//             if (consented) {
//                 KSInAppUpdateConsentForUser(1);
//             } else {
//                 KSInAppUpdateConsentForUser(0);
//             }
// #elif UNITY_ANDROID
//             AndroidProxy.CallStatic("inAppUpdateConsentForUser", new object[] { consented });
// #endif
//         }

//         public List<InAppInboxItem> InAppGetInboxItems()
//         {
//             string json = "[]";
// #if UNITY_IOS
//             json = KSInAppGetInboxItems();
// #elif UNITY_ANDROID
//             json = AndroidProxy.CallStatic<string>("inAppGetInboxItems", new object[] { });
// #endif

//             return InAppInboxItem.ListFromJson(json);
//         }

//         public bool InAppPresentInboxMessage(InAppInboxItem item)
//         {
// #if UNITY_IOS
//             return KSInAppPresentInboxMessage(item.Id);
// #elif UNITY_ANDROID
//             return AndroidProxy.CallStatic<bool>("inAppPresentInboxMessage", new object[] { item.Id });
// #else
// 			return false;
// #endif
//         }

//         public bool InAppDeleteMessageFromInbox(InAppInboxItem item)
//         {
// #if UNITY_IOS
//             return KSInAppDeleteMessageFromInbox(item.Id);
// #elif UNITY_ANDROID
//             return AndroidProxy.CallStatic<bool>("inAppDeleteMessageFromInbox", new object[] { item.Id });
// #else
// 			return false;
// #endif
//         }

//         public bool InAppMarkAsRead(InAppInboxItem item)
//         {
// #if UNITY_IOS
//             return KSInAppMarkInboxItemRead(item.Id);
// #elif UNITY_ANDROID
//             return AndroidProxy.CallStatic<bool>("inAppMarkInboxItemRead", new object[] { item.Id });
// #else
// 			return false;
// #endif
//         }


//         public bool InAppMarkAllInboxItemsRead()
//         {
// #if UNITY_IOS
//             return KSInAppMarkAllInboxItemsRead();
// #elif UNITY_ANDROID
//             return AndroidProxy.CallStatic<bool>("inAppMarkAllInboxItemsRead", new object[] { });
// #else
// 			return false;
// #endif
//         }


//         //**************************************** SUMMARY ************************************************
//         private static Dictionary<string, Action<InAppInboxSummary>> inboxSummaryHandlers = new Dictionary<string, Action<InAppInboxSummary>>();

//         private string CacheInboxSummaryHandler(Action<InAppInboxSummary> inboxSummaryHandler)
//         {
//             string guid = Guid.NewGuid().ToString();
//             while(inboxSummaryHandlers.ContainsKey(guid)){
//                 guid = Guid.NewGuid().ToString();
//             }

//             inboxSummaryHandlers.Add(guid, inboxSummaryHandler);

//             return guid;
//         }

//         private void InvokeInboxSummaryHandler(string resultJson)
//         {
//             var parsed = MiniJSON.Json.Deserialize(resultJson) as Dictionary<string, object>;
//             if (parsed == null){
//                 return;
//             }

//             string guid = parsed["guid"] as string;
//             if (!inboxSummaryHandlers.ContainsKey(guid)){
//                 return;
//             }

//             bool success = (bool)parsed.GetValueOrDefault("success");
//             InAppInboxSummary summary = null;
//             if (success){
//                 summary = InAppInboxSummary.CreateFromDictionary(parsed);
//             }

//             inboxSummaryHandlers[guid](summary);
//             inboxSummaryHandlers.Remove(guid);
//         }

//         public void GetInboxSummaryAsync(Action<InAppInboxSummary> inboxSummaryHandler) {
//             string guid = this.CacheInboxSummaryHandler(inboxSummaryHandler);
// #if UNITY_IOS
//             KSInAppGetInboxSummary(guid);
// #elif UNITY_ANDROID
//             AndroidProxy.CallStatic("inAppGetInboxSummary", new object[] { guid });
// #endif
//         }


//          //************************************************************************************************


//         #endregion

        #region Native

#if UNITY_IOS
        private const string nativeLib = "__Internal";

        // [DllImport(nativeLib)]
        // private static extern void KStrackEvent(string type, string jsonData, int immediateFlush);

        // [DllImport(nativeLib)]
        // private static extern void KSPushRequestDeviceToken();

        // [DllImport(nativeLib)]
        // private static extern void KSInAppUpdateConsentForUser(int consented);

        // [DllImport(nativeLib)]
        // private static extern string KSInAppGetInboxItems();

        // [DllImport(nativeLib)]
        // private static extern bool KSInAppPresentInboxMessage(long messageId);

        // [DllImport(nativeLib)]
        // private static extern bool KSInAppDeleteMessageFromInbox(long messageId);

        // [DllImport(nativeLib)]
        // private static extern bool KSInAppMarkInboxItemRead(long messageId);

        // [DllImport(nativeLib)]
        // private static extern bool KSInAppMarkAllInboxItemsRead();

        // [DllImport(nativeLib)]
        // private static extern void KSInAppGetInboxSummary(string guid);

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
