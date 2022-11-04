using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEditor.iOS.Xcode;
using UnityEditor.Android;
using Unity.VisualScripting.FullSerializer;



[Serializable]
public class OptimoveConfig
{
    public string optimoveCredentials;
    public string optimobileCredentials;
    public string inAppConsentStrategy;
    public string deferredDeepLinkingHost;

    public static OptimoveConfig CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<OptimoveConfig>(jsonString);
    }
}

public class PreprocessSetup : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        this.AddGoogleServicesForAndroidOnly(report);

        this.InjectOptimoveConfig();
    }

    private void AddGoogleServicesForAndroidOnly(BuildReport report)
    {
        // No way to add assets per platform
        string src = Path.Combine(Application.dataPath, "google-services.json");
        string dest = Path.Combine(Application.dataPath, "StreamingAssets/google-services.json");

        bool srcExists = File.Exists(src);
        bool destExists = File.Exists(dest);
        bool isAndroid = report.summary.platform == BuildTarget.Android;

        if (!srcExists || !isAndroid){
            if (destExists){
                FileUtil.DeleteFileOrDirectory(dest);
            }

            return;
        }

        if (!destExists){
            FileUtil.CopyFileOrDirectory(src, dest);
        }
    }

    private void InjectOptimoveConfig()
    {
        OptimoveConfig config = this.ReadConfig();
        this.ValidateConfig(config);

        if (config.inAppConsentStrategy == null){
            config.inAppConsentStrategy = "in-app-disabled";
        }

        this.SetUpIos(config);
        this.SetUpAndroid(config);
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
        if (config.deferredDeepLinkingHost != null){
            rootDict.SetString("optimoveDeferredDeepLinkingHost", config.deferredDeepLinkingHost);
        }

        plist.WriteToFile(Application.dataPath + "/Plugins/iOS/optimove.plist");
    }

    private void SetUpAndroid(OptimoveConfig config)
    {
        // No way to add assets per platform
        string src = Path.Combine(Application.dataPath, "optimove.xml");
        string dest = Path.Combine(Application.dataPath, "StreamingAssets/optimove.xml");

        bool srcExists = File.Exists(src);
        bool destExists = File.Exists(dest);

        if (!srcExists)
        {
            if (destExists)
            {
                FileUtil.DeleteFileOrDirectory(dest);
            }

            return;
        }

        if (!destExists)
        {
            string optimoveXmlTemplate = File.ReadAllText(src);
           optimoveXmlTemplate = optimoveXmlTemplate.Replace("{{OPTIMOVE_CREDENTIALS}}", config.optimoveCredentials);
            optimoveXmlTemplate = optimoveXmlTemplate.Replace("{{OPTIMOVE_MOBILE_CREDENTIALS}}", config.optimobileCredentials);
            optimoveXmlTemplate = optimoveXmlTemplate.Replace("{{IN_APP_STRATEGY}}",config.inAppConsentStrategy);
            optimoveXmlTemplate = optimoveXmlTemplate.Replace("{{ENABLE_DEFERRED_DEEP_LINKING}}", config.deferredDeepLinkingHost);

            File.WriteAllText(dest, optimoveXmlTemplate);
        }
    }


}