using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

[Serializable]
public class OptimoveConfig
{
    public string optimoveCredentials;
    public string optimobileCredentials;
    public string inAppConsentStrategy;
    public bool enableDeferredDeepLinking;

    public static OptimoveConfig CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<OptimoveConfig>(jsonString);
    }
}

public class InjectOptimoveConfig : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        OptimoveConfig config = this.ReadConfig();
        this.ValidateConfig(config);

        if (config.inAppConsentStrategy == null){
            config.inAppConsentStrategy = "in-app-disabled";
        }

        File.WriteAllText(
        Application.dataPath + "/Resources/version.txt",
        string.Format("saywhaat. optimoveCreds: {0}, optimobileCreds: {1}, inAppStrategy: {2}, ddl: {3}", config.optimoveCredentials, config.optimobileCredentials,  config.inAppConsentStrategy, config.enableDeferredDeepLinking));
    }

    private OptimoveConfig ReadConfig()
    {
        string jsonPath = Application.dataPath + "/Plugins/optimove.json";
        if (!File.Exists(jsonPath)){
            throw new BuildFailedException("Optimove.json not found. Please, add missing configuration file");
        }

        StreamReader reader = new StreamReader(jsonPath);
        string json = reader.ReadToEnd();
        try{
            return OptimoveConfig.CreateFromJSON(json);
        }
        catch(ArgumentException){
            throw new BuildFailedException("Optimove.json is not a valid json");
        }
    }

    private void ValidateConfig(OptimoveConfig config)
    {
        if (String.IsNullOrEmpty(config.optimoveCredentials) && String.IsNullOrEmpty(config.optimobileCredentials) ){
            throw new BuildFailedException("Invalid optimove.json: must set at least one of optimove or optimobile credentials");
        }

        string[] validInAppStrategies = { "auto-enroll", "explicit-by-user", "in-app-disabled"};
        if (config.inAppConsentStrategy != null){
            if(!validInAppStrategies.Any(config.inAppConsentStrategy.Contains)){
                throw new BuildFailedException("Invalid optimove.json: invalid in-app consent strategy: " + config.inAppConsentStrategy);
            }
        }
    }
}