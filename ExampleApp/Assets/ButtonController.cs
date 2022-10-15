using UnityEngine;
using UnityEngine.UI;
using OptimoveSdk;
using OptimoveSdk.MiniJSON;
using System.Collections.Generic;

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

    }

    void PushUnregister()
    {

    }

    void InAppConsentTrue()
    {

    }

    void InAppConsentFalse()
    {

    }

    // messaging
    void PresentInboxMessage()
    {

    }

    void DeleteInboxMessage()
    {

    }

    void MarkItemAsRead()
    {

    }

    void GetInboxItems()
    {

    }

    void MarkAllAsRead()
    {

    }

    void GetInboxSummary()
    {

    }


    // output
    void ClearOutput()
    {
       m_output.text = "";
    }




}