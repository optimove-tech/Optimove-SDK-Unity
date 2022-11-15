using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;
using UnityEditor.Build;

class SetUpGradleProject : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }
    public void OnPostGenerateGradleAndroidProject(string pathToUnityLibrary)
    {
        MoveGoogleServices(pathToUnityLibrary);

        MoveOptimoveConfig(pathToUnityLibrary);
    }

    private void MoveGoogleServices(string pathToUnityLibrary)
    {
        string src = Path.Combine(pathToUnityLibrary, "src", "main", "assets", "google-services.json");
        string dest = Path.Combine(pathToUnityLibrary, "..", "launcher", "google-services.json");
        if (!File.Exists(src)){
            return;
        }

        if (File.Exists(dest)){
            FileUtil.DeleteFileOrDirectory(dest);
        }

        FileUtil.MoveFileOrDirectory(src, dest);
    }

    private void MoveOptimoveConfig(string pathToUnityLibrary) {
        string src = Path.Combine(pathToUnityLibrary, "src", "main", "assets", "optimove.xml");
        string dest = Path.Combine(pathToUnityLibrary, "..", "launcher", "src", "main", "res", "values", "optimove.xml");
        if (!File.Exists(src))
        {
            throw new BuildFailedException("optimove.xml is missing in StreamingAssets");
        }

        if (File.Exists(dest))
        {
            FileUtil.DeleteFileOrDirectory(dest);
        }

        FileUtil.MoveFileOrDirectory(src, dest);
    }
}