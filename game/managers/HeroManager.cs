/*
 * @Author: delevin.ying 
 * @Date: 2019-05-28 16:17:00 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-19 16:16:46
 */
using System;
using System.Collections;
using System.Collections.Generic;
// using System.Collections.Specialized;
/// <summary>
/// 英雄管理类
/// </summary>
public class HeroManager {
    /// <summary>
    /// 所有英雄的数据
    /// </summary>
    public static HeroImf allHeroImf;

    public static void InitHeroManager () {
        //加载所有英雄数据
        loadAllHeroImf ();
    }

    public static List<HeroItem> getAllHeroItemList () {
        Faction myFaction = Faction.All;
        List<HeroItem> list = getAllHeroItemListByFaction (myFaction);
        return list;
    }

    public static List<HeroItem> getAllHeroItemListByFaction (Faction f) {
        int factionId = FactionManager.getIdByFaction (f);
        //Common.getIdByFaction (f);
        if (allHeroImf == null) return null;
        List<HeroItem> list = new List<HeroItem> ();
        foreach (var item in allHeroImf.heros) {
            if (f == Faction.All) {
                list.Add (item);
            } else {
                if (item.gj == factionId) {
                    list.Add (item);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// 获取某一方已上场的所有英雄
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static List<HeroItem> getAllShowedHeroListByFaction (Faction f) {
        int factionId = FactionManager.getIdByFaction (f);
        if (allHeroImf == null) return null;
        List<HeroItem> list = new List<HeroItem> ();
        foreach (var item in allHeroImf.heros) {
            if (factionId == item.gj && item.state == 100) {
                list.Add (item);
            }
        }
        return list;
    }

    /// <summary>
    /// 加载全部英雄的信息
    /// </summary>
    public static HeroImf loadAllHeroImf () {
        allHeroImf = ConfigUtils.loadHeroConfig ();
        MyLog.Log ("ConfigUtils  allHeroImf = " + allHeroImf);
        return allHeroImf;
    }
    /// <summary>
    /// 通过ID获取英雄
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static HeroItem getHeroItemById (int id) {
        foreach (var item in allHeroImf.heros) {
            if (item.id == id) {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 通过姓名获取英雄
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static HeroItem getHeroItemByName (string name) {
        foreach (var item in allHeroImf.heros) {
            if (item.name == name) {
                return item;
            }
        }
        return null;
    }
    /// <summary>
    /// 通过所属方获得英雄组合
    /// </summary>
    /// <param name="faction"></param>
    /// <returns></returns>
    public static List<HeroItem> getHeroListByFaction (int faction) {
        List<HeroItem> list = new List<HeroItem> ();
        foreach (var item in allHeroImf.heros) {
            if (item.gj == faction) {
                list.Add (item);
            }
        }
        return list;
    }

    /// <summary>
    /// 通过英雄ID获取英雄技能
    /// </summary>
    /// <param name="heroId"></param>
    public static List<int> getSkillListIdByHeroId (int heroId) {
        HeroItem hero = getHeroItemById (heroId);
        List<int> skillList = new List<int> ();
        if (hero.skill0 > 0) { //被动技能
            skillList.Add (hero.skill0);
        }

        if (hero.skill1 > 0) { //1技能
            skillList.Add (hero.skill1);
        }

        if (hero.skill2 > 0) { //2技能
            skillList.Add (hero.skill2);
        }

        if (hero.skill3 > 0) { //3技能
            skillList.Add (hero.skill3);
        }
        return skillList;
    }

    /// <summary>
    /// 获取英雄被动技能
    /// </summary>
    /// <param name="heroId"></param>
    /// <returns></returns>
    public static int getSkill_0_byHeroId (int heroId, bool isTest = false, int id = -1) {
        if (isTest == true) return id;
        List<int> skillList = getSkillListIdByHeroId (heroId);
        if (skillList == null || skillList.Count == 0) return -1; //没有技能
        return skillList[0];
    }

    public static bool updateHeroItem (string name, HeroItem h) {
        HeroItem item = getHeroItemByName (name);
        if (item == null) return false;
        item = h;
        return true;
    }

    public static bool updateHeroItem (int id, HeroItem h) {
        HeroItem item = getHeroItemById (id);
        if (item == null) return false;
        item = h;
        return true;
    }

    /// <summary>
    /// 通过比较武力获得最适合的英雄
    /// </summary>
    /// <returns></returns>
    public static HeroItem getBestHeroByWl (List<HeroItem> list) {
        // List<HeroItem> mList = list.OrderByDescending (o => o.wl).ToList (); //降序
        List<HeroItem> mList = getBestHeroListByWl (list);
        //list.Sort ((x, y) => -x.wl.CompareTo (y.wl));
        return (mList == null) ? null : mList[0];
    }

    /// <summary>
    /// 通过比较武力获得最适合的英雄
    /// </summary>
    /// <returns></returns>
    public static List<HeroItem> getBestHeroListByWl (List<HeroItem> list) {
        list.Sort ((x, y) => -x.wl.CompareTo (y.wl));
        return list;
    }

    public static List<HeroItem> getBestHeroListByTs (List<HeroItem> list) {
        // List<HeroItem> mList = list.OrderByDescending (o => o.ts).ToList ();
        // return mList;
        list.Sort ((x, y) => -x.ts.CompareTo (y.ts));
        return list;
    }

    public static List<HeroItem> getBestHeroListByZl (List<HeroItem> list) {
        list.Sort ((x, y) => -x.zl.CompareTo (y.zl));
        return list;
    }

    public static void DestroyManager () {

    }
}