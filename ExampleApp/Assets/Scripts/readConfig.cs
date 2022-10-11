using TMPro;
using UnityEngine;

public class readConfig : MonoBehaviour
{
    [System.Serializable]
    public class OptimoveConfig
    {
        public string optimoveCredentials;
        public string optimoveMobileCredentials;
        public string inAppConsentStrategy;
        public bool enableDeferredDeepLinking;
    }


    // Start is called before the first frame update

    void Awake()
    {
        string optimoveConfigText = getOptimoveConfigText();
        OptimoveConfig optimoveConfig = JsonUtility.FromJson<OptimoveConfig>(optimoveConfigText);
        if ((optimoveConfigText == null) || optimoveConfig == null)
        {
            Debug.LogError("Invalid Optimove.txt file");
            return;
        }

        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject optimovePlugin = new AndroidJavaObject("com.example.optimoveunityplugin.OptimovePlugin"))
        {
            if (optimovePlugin != null)
            {
                if (optimoveConfig.optimoveCredentials == null && optimoveConfig.optimoveMobileCredentials == null)
                {
                    Debug.LogError("Invalid credentials");
                }
                else
                {

                    optimovePlugin.Call("initialize", optimoveConfig.optimoveCredentials, optimoveConfig.optimoveMobileCredentials, currentActivity);
                }

            }
            else
            {
                Debug.LogError("initialization failed: optimovePlugin is null");
            }
        }

    }

    public string getOptimoveConfigText()
    {
        TextAsset optimoveConfig = (TextAsset)Resources.Load("Optimove");
        if (optimoveConfig == null)
        {
            Debug.LogError("Optimove.txt doesn't exist");
            return null;
        }
        return optimoveConfig.text;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
