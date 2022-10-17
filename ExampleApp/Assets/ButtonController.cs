using UnityEngine;
using UnityEngine.UI;
using OptimoveSdk;
using OptimoveSdk.MiniJSON;
using System.Collections.Generic;
using System;

public class ButtonController : MonoBehaviour
{
    // events
    public InputField m_screenName, m_screenCategory, m_eventType, m_eventProps;
    public Button m_reportScreenVisit, m_reportEvent;

    // association
    public InputField m_userIdInput, m_userEmailInput;
    public Button m_setUserIdButton, m_setUserEmailButton, m_registerUserButton, m_getVisitorId, m_signOutUser;

    // registration
    public Button m_pushRegister, m_pushUnregister, m_inAppConsentTrue, m_inAppConsentFalse;

    // messaging
    public InputField m_inboxItemId;
    public Button m_presentInboxMessage, m_deleteInboxMessage, m_markItemAsRead, m_getInboxItems, m_MarkAllAsRead, m_getInboxSummary;

    // output
    public Button m_clearOutput;
    public Text m_output;

    void Start()
    {
        // events
        m_reportScreenVisit.onClick.AddListener(ReportScreenVisit);
        m_reportEvent.onClick.AddListener(ReportEvent);

        // user association
        m_setUserIdButton.onClick.AddListener(SetUserId);
        m_setUserEmailButton.onClick.AddListener(SetUserEmail);
        m_registerUserButton.onClick.AddListener(RegisterUser);
        m_getVisitorId.onClick.AddListener(GetVisitorId);
        m_signOutUser.onClick.AddListener(SignOutUser);

        // registration
        m_pushRegister.onClick.AddListener(PushRegister);
        m_pushUnregister.onClick.AddListener(PushUnregister);
        m_inAppConsentTrue.onClick.AddListener(InAppConsentTrue);
        m_inAppConsentFalse.onClick.AddListener(InAppConsentFalse);

        // messaging
        m_presentInboxMessage.onClick.AddListener(PresentInboxMessage);
        m_deleteInboxMessage.onClick.AddListener(DeleteInboxMessage);
        m_markItemAsRead.onClick.AddListener(MarkItemAsRead);
        m_getInboxItems.onClick.AddListener(GetInboxItems);
        m_MarkAllAsRead.onClick.AddListener(MarkAllAsRead);
        m_getInboxSummary.onClick.AddListener(GetInboxSummary);

        // clear output
        m_clearOutput.onClick.AddListener(ClearOutput);
    }

    // events
    void ReportScreenVisit()
    {
        string category = m_screenCategory.text;

        Optimove.Shared.ReportScreenVisit(m_screenName.text, category.Equals("") ? null : category);

        m_screenName.text = "";
        m_screenCategory.text = "";
    }

    void ReportEvent()
    {
        string eventType = m_eventType.text;
        string eventProps = m_eventProps.text;

        Dictionary<string, object> props = null;
        if (!eventProps.Equals("")){
            props =  Json.Deserialize(eventProps) as Dictionary<string, object>;
        }
        Optimove.Shared.ReportEvent(eventType, props);

        m_eventType.text = "";
        m_eventProps.text = "";
    }

    // association
    void SetUserId()
    {
        Optimove.Shared.SetUserId(m_userIdInput.text);

        m_userIdInput.text = "";
    }

    void SetUserEmail()
    {
        Optimove.Shared.SetUserEmail( m_userEmailInput.text);

        m_userEmailInput.text = "";
    }

    void RegisterUser()
    {
        Optimove.Shared.RegisterUser(m_userIdInput.text, m_userEmailInput.text);
        m_userIdInput.text = "";
        m_userEmailInput.text = "";
    }

    void GetVisitorId()
    {
        m_output.text = Optimove.Shared.GetVisitorId();
    }

    void SignOutUser()
    {
        Optimove.Shared.SignOutUser();
    }

    // registration
    void PushRegister()
    {
         Optimove.Shared.PushRegister();
    }

    void PushUnregister()
    {
        Optimove.Shared.PushUnregister();
    }

    void InAppConsentTrue()
    {
        Optimove.Shared.InAppUpdateConsent(true);
    }

    void InAppConsentFalse()
    {
        Optimove.Shared.InAppUpdateConsent(false);
    }

    // messaging
    void PresentInboxMessage()
    {
        int targetId = ReadInboxItemId();
        if (targetId == 0){
            return;
        }

        List<InAppInboxItem> items = Optimove.Shared.InAppGetInboxItems();
        foreach (var item in items) {
            if (item.Id == targetId){
                OptimoveInAppPresentationResult result = Optimove.Shared.InAppPresentInboxMessage(item);
                break;
            }
        }
    }

    void DeleteInboxMessage()
    {
        int targetId = ReadInboxItemId();
        if (targetId == 0){
            return;
        }

        List<InAppInboxItem> items = Optimove.Shared.InAppGetInboxItems();
        foreach (var item in items) {
            if (item.Id == targetId){
                bool result = Optimove.Shared.InAppDeleteMessageFromInbox(item);
                break;
            }
        }
    }

    void MarkItemAsRead()
    {
        int targetId = ReadInboxItemId();
        if (targetId == 0){
            return;
        }

        List<InAppInboxItem> items = Optimove.Shared.InAppGetInboxItems();
        foreach (var item in items) {
            if (item.Id == targetId){
                bool result = Optimove.Shared.InAppMarkAsRead(item);
                break;
            }
        }
    }

    void GetInboxItems()
    {
        List<InAppInboxItem> items = Optimove.Shared.InAppGetInboxItems();

        List<string> result = new List<string>();
        foreach (var item in items) {
            result.Add(string.Join(",", new Dictionary<string, string> {
                {"id", item.Id + ""},
                {"isRead", item.IsRead + ""},
                {"sentAt", item.SentAt.ToString()},
                {"availableFrom", item.AvailableFrom.ToString()},
            }));
        }

		m_output.text = string.Join(Environment.NewLine, result);
    }

    void MarkAllAsRead()
    {
        bool result = Optimove.Shared.InAppMarkAllInboxItemsRead();
    }

    void GetInboxSummary()
    {
        Optimove.Shared.GetInboxSummaryAsync((InAppInboxSummary summary) => {
            if (summary != null){
                m_output.text = "InboxSummary. totalCount: " + summary.TotalCount + " unreadCount: "+ summary.UnreadCount;
            }
        });
    }


    // output
    void ClearOutput()
    {
       m_output.text = "";
    }



    // helpers
    int ReadInboxItemId()
    {
        int targetId = 0;
        try {
            targetId = Int32.Parse(m_inboxItemId.text);
        }
        catch (FormatException) {
        }
        catch (OverflowException) {
           Console.WriteLine("Invalid inbox item id: {0}", m_inboxItemId.text);
        }

        if (targetId <= 0){
            Console.WriteLine("Inbox item id must be a positive integer: {0}", m_inboxItemId.text);
        }

        return targetId;
    }






// setOnInboxUpdatedHandler: (inboxUpdatedHandler: InAppInboxUpdatedHandler) void;

// setPushOpenedHandler(pushOpenedHandler: PushNotificationHandler) void;

// setPushReceivedHandler(pushReceivedHandler: PushNotificationHandler) void;

// setInAppDeepLinkHandler(inAppDeepLinkHandler: InAppDeepLinkHandler) void;

// setDeepLinkHandler(deepLinkHandler: DeepLinkHandler) void;
}