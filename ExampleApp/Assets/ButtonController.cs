using UnityEngine;
using UnityEngine.UI;
using OptimoveSdk;
using OptimoveSdk.MiniJSON;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour
{
    public InputField m_userIdInput, m_userEmailInput, m_screenName, m_screenCategory, m_eventType, m_eventProps;
    public Button m_reportScreenVisit, m_reportEvent, m_setUserIdButton, m_setUserEmailButton, m_registerUserButton, m_getVisitorId, m_signOutUser;
    public Button m_clearOutput;
    public Text m_output;

    void Start()
    {
        //events
        m_reportScreenVisit.onClick.AddListener(ReportScreenVisit);
        m_reportEvent.onClick.AddListener(ReportEvent);

        //user association
        m_setUserIdButton.onClick.AddListener(SetUserId);
        m_setUserEmailButton.onClick.AddListener(SetUserEmail);
        m_registerUserButton.onClick.AddListener(RegisterUser);
        m_getVisitorId.onClick.AddListener(GetVisitorId);
        m_signOutUser.onClick.AddListener(SignOutUser);

        //clear output
        m_clearOutput.onClick.AddListener(ClearOutput);
    }

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

    void ClearOutput()
    {
       m_output.text = "";
    }




}