/*
 * Author:  Rick
 * Create:  2017/8/1 10:15:06
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源管理器 
/// </summary>
public class ResourceManager
{
    /// <summary>
    /// 获取PoolKey通过InstanceID
    /// </summary>
    private static Dictionary<int, string> poolKeysOfInstances = new Dictionary<int,string>();

    /// <summary>
    /// 所有缓存的Resource
    /// </summary>
    private static Dictionary<string, Object> resources = new Dictionary<string,Object>();


    public static GameObject GetInstance(string resName)
    {
        return GetInstance<GameObject>(resName);
    }
    /// <summary>
    /// 获取资源的实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static T GetInstance<T>(string resName) where T : Object
    {
        return GetInstance<T>(resName, resName);
    }
    public static T GetInstance<T>(string resName, string poolKey, bool cacheResource = true,bool supportRecycle = true, params object[] initArgs) where T : Object
    {
        T instance;

        //从对象池取
        if (ObjectPool.TryPull<T>(poolKey, out instance))
        {
            IRecycler recycler;
            if (TryGetComponent<IRecycler>(instance, out recycler))
            {
                recycler.OnPullFromPool();
            }
            return instance;
        }

        //实例化
        T resource = GetResource<T>(resName, cacheResource);
        instance = UnityEngine.Object.Instantiate<T>(resource);
        
        //如果有初始化组件，则初始化
        IInitializer initializer;
        if (TryGetComponent<IInitializer>(instance, out initializer))
        {
            initializer.Initlize(initArgs);
        }

        //建立索引
        int id = instance.GetInstanceID();
        poolKeysOfInstances.Add(id, poolKey);
        
        return instance;
    }

    /// <summary>
    /// 回收实例
    /// </summary>
    /// <param name="instance"></param>
    public static void RecycleInstance(UnityEngine.Object instance)
    {
        int id = instance.GetInstanceID();
        string poolKey;
        if (!poolKeysOfInstances.TryGetValue(id,out poolKey))
        {
            Debug.LogError("找不到实例：" + instance.name + " 的PoolKey，将直接销毁！");
            UnityEngine.Object.Destroy(instance);
            return;
        }

        poolKeysOfInstances.Remove(id);
        ObjectPool.Push(poolKey, instance);

        //回收处理
        IRecycler recycler;
        if (TryGetComponent<IRecycler>(instance, out recycler))
        {
            recycler.OnPushToPool();
        }
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static T GetResource<T>(string resName, bool cacheResource = true) where T : Object
    {
        Object resource;
        if (resources.TryGetValue(resName, out resource))
        {
            return (T)resource;
        }

        //加载
        resource = Resources.Load<T>(resName);
        
        //缓存
        if (cacheResource)
        {
            resources.Add(resName, resource);
        }

        return (T)resource;
    }

    /// <summary>
    /// 资源是否已缓存
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static bool ContainsResource(string resName)
    {
        return resources.ContainsKey(resName);
    }

    /// <summary>
    /// 判断实例是否是 GameObject 并且 有对应的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private static bool TryGetComponent<T>(Object instance, out T result) where T : class
    {
        GameObject go = instance as GameObject;
        if (go != null)
        {
            result = go.GetComponent<T>();
            return result != null;
        }
        result = null;
        return false;
    }


}

/// <summary>
/// 资源来源
/// </summary>
internal enum ResourceOrinal
{
    Resouces,
    AssetBundle,
}