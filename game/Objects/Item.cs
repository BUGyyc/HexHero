/*
 * @Author: delevin.ying 
 * @Date: 2019-07-19 09:41:45 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-19 16:28:32
 */

using System;
using System.Collections;
/// <summary>
/// 物品类的信息
/// </summary>
[Serializable]
public class Item {
    /// <summary>
    /// 物品ID
    /// </summary>
    public int id;
    /// <summary>
    /// 物品名称
    /// </summary>
    public string name;
    /// <summary>
    /// 物品等级
    /// </summary>
    public int level;
    /// <summary>
    /// 物品图片ID
    /// </summary>
    public int picId;
    /// <summary>
    /// 物品是否可以升级
    /// </summary>
    public bool isCanUpgrade;
    /// <summary>
    /// 物品是否受损，0-1取值
    /// </summary>
    public float health;
    /// <summary>
    /// 拥有者ID
    /// </summary>
    public int whoHave;
}