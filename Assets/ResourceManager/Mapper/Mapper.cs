/*
 * Author:  Rick
 * Create:  2017/8/1 10:26:21
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 映射器 Mapper
/// </summary>
public class Mapper
{
    //private static PathPairs pairs;
    private static Dictionary<string, string> pairs;


    public static string GetPath(string resName)
    {
        return "";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">路径</param>
    /// <param name="path">完整路径</param>
    /// <returns></returns>
    public static bool TryGetPath(string name, out string path)
    {
        if (pairs == null)
        {
            path = name;
            return false;
        }
        path = name;
        return true;
    }
}