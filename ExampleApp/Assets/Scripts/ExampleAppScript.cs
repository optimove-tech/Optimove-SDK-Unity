using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OptimoveSdk;
using TMPro;
public class ExampleAppScript : MonoBehaviour
{
    public TMP_InputField userIdInput;
    // Start is called before the first frame update
    void Start()
    {
        Optimove.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setUserId()
    {
        Optimove.Shared.SetUserId(userIdInput.text);
        userIdInput.text = "";
    }
}
