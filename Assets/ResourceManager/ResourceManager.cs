/*
 * Author:  Rick
 * Create:  2017/8/1 10:15:06
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源管理器 
/// ResourceManager
/// </summary>
public class ResourceManager
{
    private static Dictionary<string, Object> resources;

    /// <summary>
    /// 获取资源的实例（poolKey = resName）
    /// Get Instance of resource(poolKey = resName)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static T GetInstance<T>(string resName) where T : Object
    {
        return GetInstance<T>(resName, resName);
    }
    public static T GetInstance<T>(string resName, string poolKey, bool cacheResouce = true) where T : Object
    {
        if (ObjectPool.Contains(poolKey))
            return ObjectPool.Get<T>(poolKey);

        T resource = GetResource<T>(resName);
        T instance = GameObject.Instantiate<T>(resource);

        if (cacheResouce)
        {

        }
        else
        {
            //unload resource
        }

        return instance;
    }

    public static T GetResource<T>(string resName) where T : Object
    {
        UnityEngine.Object result;
        resources.TryGetValue(resName, out result);
        return (T)result;
    }

    public static bool ContainsResource(string resName)
    {
        return resources.ContainsKey(resName);
    }
}

/// <summary>
/// 资源来源自哪
/// Resouce From where
/// </summary>
internal enum ResourceOrinal
{
    Resouces,
    AssetBundle,
}