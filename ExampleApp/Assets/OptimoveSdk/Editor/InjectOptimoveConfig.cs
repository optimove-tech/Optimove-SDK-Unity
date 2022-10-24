using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEditor.iOS.Xcode;
[Serializable]
public class OptimoveConfig
{
    public string optimoveCredentials;
    public string optimobileCredentials;
    public string inAppConsentStrategy;
    public bool enableDeferredDeepLinking;
    public string cname;

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

        this.SetUpIos(config);
        //this.SetUpAndroid(config);
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

    private void SetUpIos(OptimoveConfig config)
    {
        PlistDocument plist = new PlistDocument();
        PlistElementDict rootDict = plist.root;
        rootDict.SetString("optimoveCredentials", config.optimoveCredentials);
        rootDict.SetString("optimoveMobileCredentials", config.optimobileCredentials);
        rootDict.SetString("optimoveInAppConsentStrategy", config.inAppConsentStrategy);
        if (config.enableDeferredDeepLinking){
            rootDict.SetString("optimoveEnableDeferredDeepLinking", "true");
        }

        //TODO: problem of the world: enableDeferredDeepLinking is bool or a cname (a in Flutter)
        // if (config.cname != null){
        //     rootDict.SetString("optimoveDdlCname", config.cname);
        // }

        plist.WriteToFile(Application.dataPath + "/Plugins/iOS/optimove.plist");
    }

    //private void SetUpAndroid(OptimoveConfig config)
    //{
      //TODO
    //}

    
}