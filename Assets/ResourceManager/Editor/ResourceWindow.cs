/*
 * Author:  Rick
 * Create:  2017/8/2 13:45:24
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Plugins.ResourceManager
{
    /// <summary>
    /// 映射器编辑器
    /// </summary>
    public class ResourceWindow : EditorWindow
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        private static ResourceSetting setting;

        /// <summary>
        /// 忽略的拓展名
        /// </summary>
        private static readonly string[] IGNORE_EXTENSIONS = new string[1] { ".meta" };

        [MenuItem("Window/Resource Manager")]
        public static ResourceWindow Get()
        {
            return EditorWindow.GetWindow<ResourceWindow>("Resource");
        }
        public void OnEnable()
        {
            LoadSetting();  
        }


        public void OnGUI()
        {
            //Base Settings
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            setting.pathParisOutput = EditorGUILayout.TextField("PathPairs Output", setting.pathParisOutput);
            //
            //Opertions
            GUILayout.Label("Opertions", EditorStyles.boldLabel);

            if (GUILayout.Button("Map Paths"))
            {
                MapPaths();
            }

            if (GUI.changed)
            {
                SaveSetting();
            }
        }

        private static void MapPaths()
        {
            //所有对
            List<PathPair> pairs = new List<PathPair>();

            //映射Resource
            DirectoryInfo resourceDir = new DirectoryInfo(Application.dataPath + Path.DirectorySeparatorChar + "Resources");
            if (resourceDir.Exists)
            {
                FileInfo[] allFiles = resourceDir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in allFiles)
                {
                    if (IsIgnore(file.Extension))
                        continue;

                    string name = file.Name.Replace(file.Extension, "");
                    string path = ConvertPath(file.FullName);
                    pairs.Add(new PathPair(name, path));
                }
            }

            //检查是否有重名
            Dictionary<string, int> recordCount = new Dictionary<string, int>();
            foreach (PathPair pair in pairs)
            {
                if (recordCount.ContainsKey(pair.name))
                    recordCount[pair.name]++;
                else
                    recordCount.Add(pair.name, 1);
            }

            //找出同名的Id
            List<string> sameNames = new List<string>();
            foreach (string key in recordCount.Keys)
            {
                if (recordCount[key] > 1)
                    sameNames.Add(key);
            }


            //如果有重名
            if (sameNames.Count > 0)
            {
                foreach (string sameName in sameNames)
                {
                    Debug.LogError(string.Format(Tips.SAME_NAME, sameName));
                }
                return;
            }

            //生成PathMap
            PathPairs pathMap = new PathPairs();
            pathMap.pairs = pairs.ToArray();

            //创建PathMap
            AssetDatabase.CreateAsset(pathMap, setting.pathPairsPath);
        }

        /// <summary>
        /// 是否忽略
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static bool IsIgnore(string extension)
        {
            for (int i = 0, j = IGNORE_EXTENSIONS.Length; i < j; i++)
            {
                if (IGNORE_EXTENSIONS[i] == extension)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 将FullName转换成相对于Resources的路径
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private static string ConvertPath(string fullName)
        {
            int start = fullName.IndexOf("Resources" + Path.DirectorySeparatorChar);
            return fullName.Substring(start);
        }


        /// <summary>
        /// 加载Setting
        /// </summary>
        private static void LoadSetting()
        {
            if (File.Exists(ResourceSetting.PATH))
            {
                string content = File.ReadAllText(ResourceSetting.PATH);
                setting = JsonUtility.FromJson<ResourceSetting>(content);
            }
            else
            {
                setting = new ResourceSetting();
                SaveSetting();
            }
        }

        /// <summary>
        /// 保存Setting
        /// </summary>
        private static void SaveSetting()
        {
            string json = JsonUtility.ToJson(setting, true);
            FileStream fileStream;
            if (!File.Exists(ResourceSetting.PATH))
            {
                fileStream = File.Create(ResourceSetting.PATH);
                fileStream.Close();
            }
            File.WriteAllText(ResourceSetting.PATH, json);
            AssetDatabase.Refresh();
        }

    }
}



