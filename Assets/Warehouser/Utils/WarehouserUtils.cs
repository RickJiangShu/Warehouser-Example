/*
 * Author:  Rick
 * Create:  2017/8/4 11:12:00
 * Email:   rickjiangshu@gmail.com
 * Follow:  https://github.com/RickJiangShu
 */

/// <summary>
/// WarehouserUtils
/// </summary>
public class WarehouserUtils
{
    /// <summary>
    /// 是否在Resources目录下
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool InResources(string path)
    {
        return path.IndexOf("Resources") != -1;
    }
}
