﻿/*
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

namespace Plugins.ConfigManager
{
    /// <summary>
    /// 映射器编辑器
    /// </summary>
    public class MapperEditor : ScriptableObject
    {
        /// <summary>
        /// 忽略的拓展名
        /// </summary>
        private static readonly string[] IGNORE_EXTENSIONS = new string[1] { ".meta" };


        /// <summary>
        /// 路径映射队列输出路径
        /// </summary>
        private const string PAIRS_OUTPUT = "Assets/PathPairs.asset";

        [MenuItem("Resource/Map Path")]
        private static void MapPath()
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
                    Debug.LogError(string.Format(Tips.SAME_NAME_EDITOR, sameName));
                }
                return;
            }

            //生成PathMap
            PathPairs pathMap = new PathPairs();
            pathMap.pairs = pairs.ToArray();

            //创建PathMap
            AssetDatabase.CreateAsset(pathMap, PAIRS_OUTPUT);
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

        
    }
}



