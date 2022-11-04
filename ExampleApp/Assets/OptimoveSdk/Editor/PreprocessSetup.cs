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
        this.ClearGeneratedAssetsBeforeSetup();

        bool isAndroid = report.summary.platform == BuildTarget.Android;
        if (isAndroid){
            this.AddGoogleServices();
        }

        this.InjectOptimoveConfig(isAndroid);
    }

    private void ClearGeneratedAssetsBeforeSetup()
    {
        // to make sure values are updated + to remove irrelevant platforms
        string[] androidSpecificFiles = new string[2]{
            Path.Combine(Application.dataPath, "StreamingAssets/google-services.json"),
            Path.Combine(Application.dataPath, "StreamingAssets/optimove.xml")
        };

        foreach (string path in androidSpecificFiles){
            if (File.Exists(path)){
                FileUtil.DeleteFileOrDirectory(path);
            }
        }
    }

    private void AddGoogleServices()
    {
        string src = Path.Combine(Application.dataPath, "OptimoveConfigFiles/google-services.json");
        string dest = Path.Combine(Application.dataPath, "StreamingAssets/google-services.json");

        if (File.Exists(src)){
            FileUtil.CopyFileOrDirectory(src, dest);
        }
    }

    private void InjectOptimoveConfig(bool isAndroid)
    {
        OptimoveConfig config = this.ReadConfig();
        this.ValidateConfig(config);

        if (config.inAppConsentStrategy == null){
            config.inAppConsentStrategy = "in-app-disabled";
        }

        this.SetUpIos(config);
        if (isAndroid){
            this.SetUpAndroid(config);
        }
    }

    private OptimoveConfig ReadConfig()
    {
        string jsonPath = Path.Combine(Application.dataPath, "OptimoveConfigFiles/optimove.json");

        if (!File.Exists(jsonPath)){
            throw new BuildFailedException("optimove.json not found. Please, add missing configuration file");
        }

        StreamReader reader = new StreamReader(jsonPath);
        string json = reader.ReadToEnd();
        try{
            return OptimoveConfig.CreateFromJSON(json);
        }
        catch(ArgumentException){
            throw new BuildFailedException("optimove.json is not a valid json");
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
        string templateSrc = Path.Combine(Application.dataPath, "Plugins/Android/optimoveConfigTemplate.xml");
        string dest = Path.Combine(Application.dataPath, "StreamingAssets/optimove.xml");

        string optimoveXmlTemplate = File.ReadAllText(templateSrc);
        optimoveXmlTemplate = this.FillTemplateValue(optimoveXmlTemplate, "{{OPTIMOVE_CREDENTIALS}}", config.optimoveCredentials);
        optimoveXmlTemplate = this.FillTemplateValue(optimoveXmlTemplate, "{{OPTIMOVE_MOBILE_CREDENTIALS}}", config.optimobileCredentials);
        optimoveXmlTemplate = this.FillTemplateValue(optimoveXmlTemplate, "{{OPTIMOVE_IN_APP_STRATEGY}}", config.inAppConsentStrategy);
        optimoveXmlTemplate = this.FillTemplateValue(optimoveXmlTemplate, "{{OPTIMOVE_DDL_HOST}}", config.deferredDeepLinkingHost);

        File.WriteAllText(dest, optimoveXmlTemplate);
    }

    private string FillTemplateValue(string target, string search, string value)
    {
        return target.Replace(search, value == null ? "" : value);
    }
}