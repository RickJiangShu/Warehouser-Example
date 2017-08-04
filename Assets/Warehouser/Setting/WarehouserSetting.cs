/*
 * Author:  Rick
 * Create:  2017/8/3 16:05:50
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */

namespace Plugins.Warehouser
{
    /// <summary>
    /// RemSettings
    /// </summary>
    [System.Serializable]
    public class WarehouserSetting
    {
        /// <summary>
        /// 输入/输出路径
        /// </summary>
        public const string PATH = "Assets/Resources/WarehouserSetting.json";

        /// <summary>
        /// 路径映射对名
        /// </summary>
        public const string PAIRS_ASSET_NAME = "PathPairs.asset";


        /// <summary>
        /// 路径映射对输出目录
        /// </summary>
        public string pathParisOutput = "Assets/Resources";


        /// <summary>
        /// 路径映射完整路径
        /// </summary>
        public string pathPairsPath
        {
            get { return pathParisOutput + "/" + PAIRS_ASSET_NAME; }
        }
    }
}
