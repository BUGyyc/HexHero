using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// using GameFramework;
// using LitJson;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
/// <summary>
/// 配置操作工具类
/// </summary>
public class ConfigUtils {
    // private static HeroData obj = null;
    /// <summary>
    /// 加载所有英雄配置
    /// </summary>
    /// <returns></returns>
    public static HeroImf loadHeroConfig () {
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/heros.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/heros.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            HeroImf gs = JsonUtility.FromJson<HeroImf> (json);
            return gs;
        }
        return null;
    }

    /// <summary>
    /// 加载所有活动配置
    /// </summary>
    /// <returns></returns>
    public static ActImf loadAllActConfig () {
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/MainAct.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/MainAct.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            ActImf actImf = JsonUtility.FromJson<ActImf> (json);
            return actImf;
        }
        return null;
    }

    /// <summary>
    /// 加载各个玩家信息
    /// </summary>
    /// <returns></returns>
    public static PlayerImf loadAllPlayerConfig () {
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/player.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/player.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            PlayerImf imf = JsonUtility.FromJson<PlayerImf> (json);
            return imf;
        }
        return null;
    }

    /// <summary>
    /// 加载仓库资源
    /// </summary>
    /// <returns></returns>
    public static WarehouseImf loadAllWarehouseConfig(){
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/warehouse.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/warehouse.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            WarehouseImf imf = JsonUtility.FromJson<WarehouseImf> (json);
            return imf;
        }
        return null;
    }

     /// <summary>
    /// 加载物品数据
    /// </summary>
    /// <returns></returns>
    public static ItemImf loadAllItemConfig(){
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/item.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/item.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            ItemImf imf = JsonUtility.FromJson<ItemImf> (json);
            return imf;
        }
        return null;
    }

    /// <summary>
    /// 读取所有技能配置
    /// </summary>
    /// <returns></returns>
    public static SkillImf loadAllSkillConfig(){
        BinaryFormatter bf = new BinaryFormatter ();
        if (!File.Exists (Application.dataPath + "/Resources/skills.json")) {
            return null;
        }
        StreamReader sr = new StreamReader (Application.dataPath + "/Resources/skills.json");
        if (sr == null) {
            return null;
        }
        string json = sr.ReadToEnd ();
        if (json.Length > 0) {
            SkillImf imf = JsonUtility.FromJson<SkillImf> (json);
            return imf;
        }
        return null;
    }

    /// <summary>
    /// 保存所有英雄配置
    /// </summary>
    /// <returns></returns>
    public static bool saveHeroConfig () {
        return false;
    }
}