using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using System.IO;

class SetUpGradleProject : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }
    public void OnPostGenerateGradleAndroidProject(string pathToUnityLibrary)
    {
        string src = Path.Combine(pathToUnityLibrary, "src/main/assets/google-services.json");
        string dest = Path.Combine(pathToUnityLibrary, "../launcher/google-services.json");
        if (!File.Exists(src)){
            return;
        }

        if (File.Exists(dest)){
            FileUtil.DeleteFileOrDirectory(dest);
        }

        FileUtil.MoveFileOrDirectory(src, dest);
        setUpAndroidConfig(pathToUnityLibrary);
    }



    private void setUpAndroidConfig(string pathToUnityLibrary) {
        string src = Path.Combine(pathToUnityLibrary, "src/main/assets/optimove.xml");
        string dest = Path.Combine(pathToUnityLibrary, "../launcher/src/main/res/optimove.xml");
        if (!File.Exists(src))
        {
            return;
        }

        if (File.Exists(dest))
        {
            FileUtil.DeleteFileOrDirectory(dest);
        }

        FileUtil.MoveFileOrDirectory(src, dest);
    }
}