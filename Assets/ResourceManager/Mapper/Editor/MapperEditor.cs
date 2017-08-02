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
        /// 重命时的分隔符
        /// </summary>
        private const char SAME_NAME_SEPARATOR = '/';

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

            //过滤重名
            List<string> sameNames;
            Filter(pairs, out sameNames);

            //如果有重名
            if (sameNames.Count > 0)
            {
                //将同名的放入字典
                Dictionary<string, List<PathPair>> allPairs = new Dictionary<string, List<PathPair>>();

             
                foreach (string sameName in sameNames)
                {
                    List<PathPair> pairsOfSameName = new List<PathPair>();//所有同名的Pair
                    foreach (PathPair pair in pairs)
                    {
                        if (pair.name == sameName)
                        {
                            //替换重名的分隔符
                            pair.id = pair.id.Replace(Path.DirectorySeparatorChar, SAME_NAME_SEPARATOR);
                            pairsOfSameName.Add(pair);
                        }
                    }
                    //装入字典
                    allPairs.Add(sameName, pairsOfSameName);

                    //定义错误字符
                    pairs.Add(new PathPair(sameName, PathPair.ERROR_SAMENAME));
                }

                //警告重名
                for(int i = 0,j = sameNames.Count;i<j;i++)
                {
                    List<PathPair> pairsOfSameName = allPairs[sameNames[i]];
                    string ids = "";
                    foreach (PathPair pair in pairsOfSameName)
                    {
                        ids += pair.id + "、";
                    }
                    ids = ids.Remove(ids.Length - 1, 1);
                    Debug.Log(string.Format(Tips.SAME_NAME_EDITOR, sameNames[i], pairsOfSameName.Count) + ids);
                }
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


        /// <summary>
        /// 过滤（通过不断递归，直至没有重复ID）
        /// </summary>
        private static void Filter(List<PathPair> pairs, out List<string> sameNames)
        {
            //记录次数
            Dictionary<string, int> recordCount = new Dictionary<string, int>();
            foreach (PathPair pair in pairs)
            {
                if (recordCount.ContainsKey(pair.id))
                    recordCount[pair.id]++;
                else
                    recordCount.Add(pair.id, 1);
            }

            //找出同名的Id
            sameNames = new List<string>();
            foreach (string key in recordCount.Keys)
            {
                if (recordCount[key] > 1)
                    sameNames.Add(key);
            }

            //退出递归
            if (sameNames.Count == 0)
                return;

            //将同名的加上父目录名
            foreach (string replaceId in sameNames)
            {
                foreach (PathPair pair in pairs)
                {
                    if (pair.id == replaceId)
                    {
                        string newId = InsertParentPath(pair.id, pair.path);
                        pair.id = newId;
                    }
                }
            }

            //进入递归
            List<string> ids;
            Filter(pairs, out ids);
        }

        /// <summary>
        /// 将id前插入父目录的路径
        /// </summary>
        /// <returns></returns>
        private static string InsertParentPath(string id, string path)
        {
            int searchIndex = path.LastIndexOf(Path.DirectorySeparatorChar + id);
            int parentIndex = path.LastIndexOf(Path.DirectorySeparatorChar, searchIndex - 1);
            return path.Substring(parentIndex + 1, searchIndex - parentIndex) + id;
        }

        
    }
}



