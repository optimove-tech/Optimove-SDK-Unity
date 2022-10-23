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

    private static List<string> logMessages = new List<string>();
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

        SetUpHandlers();
    }

    void SetUpHandlers()
    {
        Optimove.Shared.setInAppDeepLinkHandler ( (InAppButtonPress press) =>
        {
            string deepLinkData = OptimoveSdk.MiniJSON.Json.Serialize(press.DeepLinkData);
            string messageData = OptimoveSdk.MiniJSON.Json.Serialize(press.MessageData);

            AddLogMessage(string.Format("InAppDeepLinkPressedHandler: id: {0}, deepLinkData: {1}, messageData: {2}", press.MessageId, deepLinkData, messageData));
        });

        Optimove.Shared.setInAppInboxUpdatedHandler(() =>
        {
            AddLogMessage("InAppInboxUpdatedHandler");
        });

        Optimove.Shared.setPushOpenedHandler( (PushMessage push) =>
        {
            string id = push.Id.ToString();
            string title = push.Title ?? "";
            string message = push.Message ?? "";
            string data = push.Data != null ? OptimoveSdk.MiniJSON.Json.Serialize(push.Data) : "";

            AddLogMessage(string.Format("PushOpenedHandler: id: {0}, title: {1}, message: {2}, data: {3} ", id, title, message, data));
        });

        Optimove.Shared.setPushReceivedHandler ( (PushMessage push) =>
        {
            string id = push.Id.ToString();
            string title = push.Title ?? "";
            string message = push.Message ?? "";
            string data = push.Data != null ? OptimoveSdk.MiniJSON.Json.Serialize(push.Data) : "";

            AddLogMessage(string.Format("PushReceivedHandler: id: {0}, title: {1}, message: {2}, data: {3} ", id, title, message, data));
        });

        Optimove.Shared.OnDeepLinkResolved += (DeepLink ddl) =>
        {
            string resolution = ddl.Resolution.ToString();
            string url = ddl.Url;
            string linkData = ddl.LinkData != null ? OptimoveSdk.MiniJSON.Json.Serialize(ddl.LinkData) : "";
            string content = ddl.Content != null ? OptimoveSdk.MiniJSON.Json.Serialize(ddl.Content) : "";

            AddLogMessage(string.Format("DeepLinkHandler: resolution: {0}, url: {1}, linkData: {2}, content: {3} ", resolution, url, linkData, content));
        };
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
        if (!eventProps.Equals(""))
        {
            props = Json.Deserialize(eventProps) as Dictionary<string, object>;
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
        AddLogMessage(Optimove.Shared.GetVisitorId());
    }

    void SignOutUser()
    {
        Optimove.Shared.SignOutUser();
    }

    // registration
    void PushRegister()
    {
        AddLogMessage("Registering for push");
        Optimove.Shared.PushRegister();
    }

    void PushUnregister()
    {
        AddLogMessage("Unregistering for push");
        Optimove.Shared.PushUnregister();
    }

    void InAppConsentTrue()
    {
        AddLogMessage("Updating in-app consent: true");
        Optimove.Shared.InAppUpdateConsent(true);
    }

    void InAppConsentFalse()
    {
        AddLogMessage("Updating in-app consent: false");
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
                AddLogMessage("Present message result: " + result.ToString());
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
                AddLogMessage("Delete message result: " + result);
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
                AddLogMessage("Mark item read result: " + result);
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

        string toLog = result.Count == 0 ? "[]" : string.Join(Environment.NewLine, result);
        AddLogMessage(toLog);
    }

    void MarkAllAsRead()
    {
        bool result = Optimove.Shared.InAppMarkAllInboxItemsRead();

        AddLogMessage("Mark all items read result: " + result);
    }
    
    void GetInboxSummary()
    {
        Optimove.Shared.GetInboxSummaryAsync((InAppInboxSummary summary) => {
            if (summary != null){
                AddLogMessage("InboxSummary. totalCount: " + summary.TotalCount + " unreadCount: "+ summary.UnreadCount);
            }
        });
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
        m_inboxItemId.text = "";
        return targetId;
    }

    void AddLogMessage(string message)
    {
        string prefix = DateTime.Now.ToString("[MM/dd/yyyy h:mm:ss]: ");

        ButtonController.logMessages.Add(prefix + message);

        m_output.text = string.Join(Environment.NewLine, ButtonController.logMessages);
    }

    void ClearOutput()
    {
       ButtonController.logMessages.Clear();
       m_output.text = "";
    }
}