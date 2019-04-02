using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class OriginalAssetCreator : EditorWindow
{
    string class_Name = "";
    string config_Name = "";
    string file_floder = "";

    [MenuItem("CreateOrignalAsset/CreateAsset")]
    static void Init()
    {
        OriginalAssetCreator OriginalAssetCreator = (OriginalAssetCreator)EditorWindow.GetWindow(typeof(OriginalAssetCreator), false, "MyWindow", true);//创建窗口
        OriginalAssetCreator.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Class Name", EditorStyles.boldLabel);
        class_Name = EditorGUILayout.TextField("Config类名", class_Name);
        GUILayout.Label("Config Name", EditorStyles.boldLabel);
        config_Name = EditorGUILayout.TextField("Asset文件名", config_Name);
        GUILayout.Label("File Path", EditorStyles.boldLabel);
        file_floder = EditorGUILayout.TextField("保存路径", file_floder);
        GUILayout.Space(20);
        if (GUILayout.Button("Save AssetFile"))
        {
            CreateAsset();
        }
        GUILayout.EndVertical();
    }

    void CreateAsset()
    {
        if (class_Name.Length == 0 || config_Name.Length == 0 || file_floder.Length == 0)
        {
            Debug.LogError(string.Format("[UConfig]: 创建失败 , 信息不完整!"));
            return;
        }
        ScriptableObject config = ScriptableObject.CreateInstance(class_Name);

        if (config == null)
        {
            Debug.LogError(string.Format("[UConfig]: 创建失败 , 类名无法识别! --> {0}", class_Name));
            return;
        }
        string path = file_floder;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        config_Name = config_Name.Replace(".asset", "");
        path = string.Format("{0}/{1}.asset", file_floder, config_Name);
        string defFilePath = "Assets/" + config_Name + ".asset";
        AssetDatabase.CreateAsset(config, defFilePath);
        File.Move(Application.dataPath + string.Format("/{0}.asset", config_Name), path);
        AssetDatabase.Refresh();
        Debug.Log(string.Format("<color=yellow>[UConfig]: 创建成功 ! --> {0}</color>", path));
    }
}
