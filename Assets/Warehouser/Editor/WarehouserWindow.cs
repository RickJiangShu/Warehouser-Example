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

namespace Plugins.Warehouser
{
    /// <summary>
    /// 映射器编辑器
    /// </summary>
    public class WarehouserWindow : EditorWindow
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        private static WarehouserSetting setting;

        /// <summary>
        /// 忽略的拓展名
        /// </summary>
        private static readonly string[] IGNORE_EXTENSIONS = new string[1] { ".meta" };

        [MenuItem("Window/Warehouser")]
        public static WarehouserWindow Get()
        {
            return EditorWindow.GetWindow<WarehouserWindow>("Warehouser");
        }
        public void OnEnable()
        {
            LoadSetting();  
        }


        public void OnGUI()
        {
            //Base Settings
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            setting.pathPairsDirectory = EditorGUILayout.TextField("PathPairs Directory", setting.pathPairsDirectory);
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
                    if (IsIgnore(file.Name))
                        continue;

                    string name = file.Name.Replace(file.Extension, "");
                    string path = WarehouserUtils.Convert2ResourcesPath(file.FullName);
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
            if (File.Exists(setting.pathPairsPath))
            {
                UnityEngine.Object old = AssetDatabase.LoadMainAssetAtPath(setting.pathPairsPath);
                EditorUtility.CopySerialized(pathMap, old);
            }
            else
                AssetDatabase.CreateAsset(pathMap, setting.pathPairsPath);
        }

        /// <summary>
        /// 是否忽略
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool IsIgnore(string fileName)
        {
            for (int i = 0, j = IGNORE_EXTENSIONS.Length; i < j; i++)
            {
                if (IGNORE_EXTENSIONS[i] == Path.GetExtension(fileName))
                    return true;
            }

            if (fileName == Path.GetFileName(WarehouserSetting.PATH))
                return true;

            if (fileName == WarehouserSetting.PATH_PAIRS_NAME)
                return true;

            return false;
        }


        /// <summary>
        /// 加载Setting
        /// </summary>
        private static void LoadSetting()
        {
            if (File.Exists(WarehouserSetting.PATH))
            {
                string content = File.ReadAllText(WarehouserSetting.PATH);
                setting = JsonUtility.FromJson<WarehouserSetting>(content);
            }
            else
            {
                setting = new WarehouserSetting();
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
            if (!File.Exists(WarehouserSetting.PATH))
            {
                fileStream = File.Create(WarehouserSetting.PATH);
                fileStream.Close();
            }
            File.WriteAllText(WarehouserSetting.PATH, json);
            AssetDatabase.Refresh();
        }

    }
}



