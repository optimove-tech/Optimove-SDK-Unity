using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OptimoveSdk;
using TMPro;
public class ExampleAppScript : MonoBehaviour
{
    public TMP_InputField userIdInput;
    public TMP_InputField userEmailInput;
    
    public void setUserId()
    {
        Optimove.Shared.SetUserId(userIdInput.text);
        userIdInput.text = "";
    }

    public void setUserEmail()
    {
        Optimove.Shared.SetUserEmail(userEmailInput.text);
        userEmailInput.text = "";
    }
}
