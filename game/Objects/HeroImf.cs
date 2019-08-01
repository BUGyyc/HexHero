/*
 * @Author: delevin.ying 
 * @Date: 2019-05-29 16:03:49 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-19 16:27:32
 */
using System;
using System.Collections;
// using UnityEngine;
[Serializable]
public class HeroImf {
    public HeroItem[] heros;
    /// <summary>
    /// 获取英雄的行动力
    /// </summary>
    // public static int getHeroMoveStep (HeroItem hero) {
    //     int step = Math.Floor (hero.ts + hero.wl + hero.zl) / 10;
    //     step = Math.Max (5, Math.Min (step, 15));
    //     return step;
    // }

    /// <summary>
    /// 获取英雄特殊技能
    /// </summary>
    /// <param name="hero"></param>
    public static void getHeroSpecial (HeroItem hero) {

    }

    /// <summary>
    /// 英雄攻击伤害值
    /// </summary>
    /// <param name="hero"></param>
    /// <returns></returns>
    public static int getHeroAttackValue (HeroItem hero) {
        return 0;
    }

    /// <summary>
    /// 获取武将带领的军队兵种
    /// </summary>
    /// <param name="hero"></param>
    /// <returns></returns>
    public static Solider getHeroSoliderType (HeroItem hero) {
        return Solider.Spearman;
    }

    /// <summary>
    /// 每个兵种的伤害值
    /// </summary>
    /// <param name="type"></param>
    // /// <returns></returns>
    // public static int getAttackTypeBySolider (Solider type) {

    // }   
}

public enum HeroSpecail {
    MOVE_FAST, //快速行军
    KEEP_CLAM, //保持冷静，不受扰乱影响，免疫
    HARASS_ENEMY, //扰乱敌军，降低敌方攻击力
    CRIT_ATTACK, //暴击，有一定几率暴击，武力越高，暴击概率越高
    BEST_DEFENSE, //最佳防御，降低对方的所造成的伤害
    NORMAL, //无技能
    WATER_MOVE, //无视河流，可以在水中行走
}

/**
        "name": "阿会喃",
        "xb": "男",
        "ts": "65",
        "wl": "75",
        "zl": "31",
        "zz": "34",
        "ml": "45",
        "sm": "35",
        "gj": "13",
        "sf": "1"
 */
 