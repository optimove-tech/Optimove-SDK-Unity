using UnityEngine;
using UnityEngine.UI;
using OptimoveSdk;
using OptimoveSdk.MiniJSON;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour
{
    public InputField m_userIdInput, m_userEmailInput, m_screenName, m_screenCategory, m_eventType, m_eventProps;
    public Button m_ReportScreenVisit, m_ReportEvent, m_SetUserIdButton, m_SetUserEmailButton, m_RegisterUserButton, m_GetVisitorId, m_SignOutUser;

    void Start()
    {
        //events
        m_ReportScreenVisit.onClick.AddListener(ReportScreenVisit);
        m_ReportEvent.onClick.AddListener(ReportEvent);

        //user association
        m_SetUserIdButton.onClick.AddListener(SetUserId);
        m_SetUserEmailButton.onClick.AddListener(SetUserEmail);
        m_RegisterUserButton.onClick.AddListener(RegisterUser);
        m_GetVisitorId.onClick.AddListener(GetVisitorId);
        m_SignOutUser.onClick.AddListener(SignOutUser);
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

        Optimove.Shared.ReportEvent(eventType, Json.Deserialize(eventProps) as Dictionary<string, object>);

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
        Debug.Log("GetVisitorId: " + Optimove.Shared.GetVisitorId());
    }

    void SignOutUser()
    {
        Optimove.Shared.SignOutUser();
    }

}