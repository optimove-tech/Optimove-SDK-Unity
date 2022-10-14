// To use this example, attach this script to an empty GameObject.
// Create three buttons (Create>UI>Button). Next, select your
// empty GameObject in the Hierarchy and click and drag each of your
// Buttons from the Hierarchy to the Your First Button, Your Second Button
// and Your Third Button fields in the Inspector.
// Click each Button in Play Mode to output their message to the console.
// Note that click means press down and then release.

using UnityEngine;
using UnityEngine.UI;
using OptimoveSdk;
using OptimoveSdk.MiniJSON;
using System.Collections.Generic;

public class ButtonController : MonoBehaviour
{
    public InputField m_userIdInput, m_userEmailInput, m_screenName, m_screenCategory, m_eventType, m_eventProps;
    public Button m_ReportScreenVisit, m_ReportEvent, m_SetUserIdButton, m_SetUserEmailButton, m_RegisterUserButton, m_GetVisitorId;

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


}