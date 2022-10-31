using System.Collections.Generic;
using UnityEngine;
using OptimoveSdk;

public class OptimoveInit : MonoBehaviour
{

    void Awake()
    {
        Optimove.Initialize();

        // Uncomment the following to register for push notifications
        //Optimove.Shared.PushRegister();

        // Uncomment to handle push message receipt
        //Optimove.Shared.OnPushReceived += (PushMessage message) =>
        //{
        //    Debug.Log("Push message received");
        //};

        // Uncomment to handle in-app message deep-link button actions
        //Optimove.Shared.OnInAppDeepLinkPressed += (Dictionary<string, object> data) =>
        //{
        //    Debug.Log("In-app deep link pressed");
        //};

        // Uncomment to handle in-app inbox updates
        //Optimove.Shared.OnInAppInboxUpdated += OnInboxUpdated;
    }

    // public static void OnInboxUpdated() {
    //     Debug.Log("In-app inbox updated");
    // }
}
