/*
 * @Author: delevin.ying 
 * @Date: 2019-07-19 09:35:13 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-19 16:29:15
 */
using System;
using System.Collections;
using System.Collections.Generic;
public class WarehouseManager {
    // //
    // public static Dictionary<string, List<WarehouseItem>> allItem;

    public static WarehouseImf warehouseImf;

    public static void InitWarehouseManager () {
        // allItem = new Dictionary<string, List<WarehouseItem>> ();
        loadAllWarehouseImf ();
    }

    public static void loadAllWarehouseImf () {
        warehouseImf = ConfigUtils.loadAllWarehouseConfig ();
        MyLog.Log (" ConfigUtils   加载仓库  ");
    }

    public static List<WarehouseItem> getMyAllWarehouseItemList () {
        Faction myFaction = Global.myFaction;
        List<WarehouseItem> list = getAllWarehouseItemListByFaction (myFaction);
        return list;
    }

    public static List<WarehouseItem> getAllWarehouseItemListByFaction (Faction f) {
        int factionId = getIdByFaction (f);
        if (warehouseImf == null) return null;
        List<WarehouseItem> list = new List<WarehouseItem> ();
        foreach (var item in warehouseImf.allItems) {
            if (item.faction == factionId) {
                list.Add (item);
            }
        }
        return list;
    }

    public static int getIdByFaction (Faction f) {
        switch (f) {
            case Faction.RED:
                return 0;
            case Faction.BLUE:
                return 1;
            case Faction.GREEN:
                return 2;
            case Faction.BLACK:
                return 3;
            case Faction.GARY:
                return 4;
            default:
                return 0;
        }
    }

    // /// <summary>
    // /// 加入物品
    // /// </summary>
    // /// <param name="itemName"></param>
    // /// <param name="list"></param>
    // public static void AddWarehouseItemList (string itemName, List<WarehouseItem> list) {
    //     if (allItem.ContainsKey (itemName)) {
    //         //已包含
    //         List<WarehouseItem> temp = allItem[itemName];
    //         foreach (var item in list) {
    //             temp.Add (item);
    //         }
    //         allItem[itemName] = temp;
    //     } else {
    //         //不包含
    //         allItem.Add (itemName, list);
    //     }
    // }
    // /// <summary>
    // /// 加入物品
    // /// </summary>
    // /// <param name="itemName"></param>
    // /// <param name="list"></param>
    // public static void AddWarehouseItem (string itemName, WarehouseItem wItem) {
    //     if (allItem.ContainsKey (itemName)) {
    //         //已包含
    //         List<WarehouseItem> temp = allItem[itemName];
    //         temp.Add (wItem);
    //         allItem[itemName] = temp;
    //     } else {
    //         List<WarehouseItem> temp = new List<WarehouseItem> ();
    //         temp.Add (wItem);
    //         allItem.Add (itemName, temp);
    //     }
    // }
    // /// <summary>
    // /// 删除物品
    // /// </summary>
    // /// <param name="itemName"></param>
    // /// <param name="list"></param>
    // public static void removeWarehouseItemList (string itemName, List<WarehouseItem> list) {

    // }
    // /// <summary>
    // /// 删除物品
    // /// </summary>
    // /// <param name="itemName"></param>
    // /// <param name="item"></param>
    // public static void removeWarehouseItem (string itemName, WarehouseItem item) {
    //     if (allItem.ContainsKey (itemName)) {
    //         //已包含
    //         List<WarehouseItem> temp = allItem[itemName];
    //         foreach (var w in temp) {
    //             if (w.id == item.id) {
    //                 temp.Remove (w);
    //                 break;
    //             }
    //         }
    //         allItem[itemName] = temp;
    //     } else {
    //         MyLog.Log ("移除错误  仓库中不存在 " + itemName, LogType.ERR);
    //     }
    // }

    // /// <summary>
    // /// 修改商品的信息
    // /// </summary>
    // /// <param name="itemName"></param>
    // /// <param name="item"></param>
    // public static void setWarehouseItemData (string itemName, WarehouseItem item) {
    //     if (allItem.ContainsKey (itemName)) {
    //         //已包含
    //         List<WarehouseItem> temp = allItem[itemName];
    //         foreach (var w in temp) {
    //             if (w.id == item.id) {
    //                 temp.Remove (w);
    //                 temp.Add (item);
    //                 break;
    //             }
    //         }
    //         allItem[itemName] = temp;
    //     } else {
    //         MyLog.Log ("修改错误  仓库中不存在 " + itemName, LogType.ERR);
    //     }
    // }

    public static void DestroyManager () {

    }
}