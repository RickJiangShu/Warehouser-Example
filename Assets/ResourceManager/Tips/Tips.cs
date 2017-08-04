﻿/*
 * Author:  Rick
 * Create:  2017/8/2 15:54:13
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */
using System.Collections;
using System.Collections.Generic;

namespace Plugins.ResourceManager
{
    /// <summary>
    /// Tips
    /// </summary>
    public class Tips
    {
        public const string SAME_NAME = "重复文件名： {0}";
        public const string NO_SETTING = "找不到" + ResourceSetting.PATH;
        public const string NO_PAIRS = "找不到路径映射文件";
        public const string NO_START = "ResourceManager 未启动，使用前调用Start()";
        public const string NO_GET_PATH = "没有找到路径";

        //public const string SAME_NAME_RUNTIME = "该文件名重复，请使用正确的Id或完整路径！";
    }
}
