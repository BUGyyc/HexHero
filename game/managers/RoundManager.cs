/*
 * @Author: delevin.ying 
 * @Date: 2019-06-04 15:41:26 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-06-04 16:46:32
 */
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 回合管理器
/// </summary>
public class RoundManager : MonoBehaviour {
    /// <summary>
    /// 游戏内的所有玩家
    /// </summary>
    public List<Faction> gameFactions;
    /// <summary>
    /// 当前第几回合
    /// </summary>
    public int currRound = 0;
    /// <summary>
    /// 当前回合轮到哪一方
    /// </summary>
    public Faction currFaction;
    /// <summary>
    /// 当前回合的第几方
    /// </summary>
    public int currFactionIndex = 0;

    public HexGrid hexGrid;

    public static RoundManager mInstance;

    private void Awake () {
        mInstance = this;
        initAllFaction (7);
    }

    /// <summary>
    /// 初始化所有方的信息
    /// </summary>
    /// <param name="num"></param>
    public void initAllFaction (int num) {
        MyLog.Log ("RoundManager -- initAllFaction");
        gameFactions = new List<Faction> ();
        for (int i = 0; i < num; i++) {
            Faction f = getFaction (i);
            gameFactions.Add (f);
        }
    }

    /// <summary>
    /// 开始第几回合
    /// </summary>
    /// <param name="round"></param>
    public void onStartRound (int round, HexGrid hexGrid) {
        hexGrid = hexGrid;
        MyLog.Log ("RoundManager -- onStartRound  开始回合 -- " + round);
        if (round - currRound != 1) {
            MyLog.Log ("回合出错~", LogType.ERR);
            return;
        }
        currRound = round;
        currFactionIndex = 0;
        currFaction = gameFactions[currFactionIndex];
        //不是我操作，机器人操作
        if (currFaction != Global.myFaction) {
            //通知机器人操作
            RobotManager.mInstance.onFactionAILogic (currFaction, hexGrid);
        } else {
            //通知自己操作

        }
    }

    /// <summary>
    /// 轮到下一方操作
    /// </summary>
    public Faction goNextFaction () {
        MyLog.Log ("RoundManager -- goNextFaction  轮到下一方操作");
        int index = gameFactions.IndexOf (currFaction);
        if (index < 0 || index >= gameFactions.Count) {
            MyLog.Log ("回合溢出出错~", LogType.ERR);
            return Faction.DEFAULT;
        }
        index++;
        currFaction = gameFactions[index];
        Common.getInstance ().showPopTip ("轮到" + currFaction + "操作", 1);
        return currFaction;
    }

    /// <summary>
    /// 进入下一回合
    /// </summary>
    public int goNextRound () {
        int round = currRound + 1;
        onStartRound (round, hexGrid);
        return round;
    }

    /// <summary>
    /// 根据编号分配
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private Faction getFaction (int num) {
        switch (num) {
            case 0:
                return Faction.RED;
                break;
            case 1:
                return Faction.BLUE;
                break;
            case 2:
                return Faction.YELLOW;
                break;
            case 3:
                return Faction.GREEN;
                break;
            case 4:
                return Faction.BLACK;
                break;
            case 5:
                return Faction.WHITE;
                break;
            case 6:
                return Faction.GARY;
                break;
            case 7:
                return Faction.ORANGE;
                break;
            default:
                return Faction.DEFAULT;
                break;
        }
    }
}

//////////////////////////////////////////////////////////////////////////////
// public class RoundManager {
//     /// <summary>
//     /// 游戏内的所有玩家
//     /// </summary>
//     public static List<Faction> gameFactions;
//     /// <summary>
//     /// 当前第几回合
//     /// </summary>
//     public static int currRound = 0;
//     /// <summary>
//     /// 当前回合轮到哪一方
//     /// </summary>
//     public static Faction currFaction;
//     /// <summary>
//     /// 当前回合的第几方
//     /// </summary>
//     public static int currFactionIndex = 0;

//     public static RobotManager mRobotManager;

//     public static HexGrid hexGrid;

//     /// <summary>
//     /// 初始化所有方的信息
//     /// </summary>
//     /// <param name="num"></param>
//     public static void initAllFaction (int num) {
//         MyLog.Log ("RoundManager -- initAllFaction");
//         gameFactions = new List<Faction> ();
//         for (int i = 0; i < num; i++) {
//             Faction f = getFaction (num);
//             gameFactions.Add (f);
//         }

//         mRobotManager = new RobotManager ();
//     }

//     /// <summary>
//     /// 开始第几回合
//     /// </summary>
//     /// <param name="round"></param>
//     public static void onStartRound (int round, HexGrid hexGrid) {
//         hexGrid = hexGrid;
//         MyLog.Log ("RoundManager -- onStartRound  开始回合 -- " + round);
//         if (round - currRound != 1) {
//             MyLog.Log ("回合出错~", LogType.ERR);
//             return;
//         }
//         currRound = round;
//         currFactionIndex = 0;
//         currFaction = gameFactions[currFactionIndex];

//         //不是我操作，机器人操作
//         if (currFaction != Global.myFaction) {
//             //通知机器人操作
//             RobotManager.onFactionAILogic (currFaction, hexGrid);
//         } else {
//             //通知自己操作

//         }
//     }

//     /// <summary>
//     /// 轮到下一方操作
//     /// </summary>
//     public static Faction goNextFaction () {
//         MyLog.Log ("RoundManager -- goNextFaction  轮到下一方操作");
//         int index = gameFactions.IndexOf (currFaction);
//         if (index < 0 || index >= gameFactions.Count) {
//             MyLog.Log ("回合溢出出错~", LogType.ERR);
//             return Faction.DEFAULT;
//         }
//         index++;
//         currFaction = gameFactions[index];
//         Common.getInstance ().showPopTip ("轮到" + currFaction + "操作", 1);
//         return currFaction;
//     }

//     /// <summary>
//     /// 进入下一回合
//     /// </summary>
//     public static int goNextRound () {
//         int round = currRound + 1;
//         onStartRound (round, hexGrid);
//         return round;
//     }

//     /// <summary>
//     /// 根据编号分配
//     /// </summary>
//     /// <param name="num"></param>
//     /// <returns></returns>
//     private static Faction getFaction (int num) {
//         switch (num) {
//             case 0:
//                 return Faction.RED;
//                 break;
//             case 1:
//                 return Faction.BLUE;
//                 break;
//             case 2:
//                 return Faction.YELLOW;
//                 break;
//             case 3:
//                 return Faction.GREEN;
//                 break;
//             case 4:
//                 return Faction.BLACK;
//                 break;
//             case 5:
//                 return Faction.WHITE;
//                 break;
//             case 6:
//                 return Faction.GARY;
//                 break;
//             case 7:
//                 return Faction.ORANGE;
//                 break;
//             default:
//                 return Faction.DEFAULT;
//                 break;
//         }
//     }

//     // /// <summary>
//     // /// 传入所有的物件list
//     // /// </summary>
//     // /// <param name="list"></param>
//     // public static void setUnits (List<HexUnit> list) {
//     //     units = list;
//     // }
// }