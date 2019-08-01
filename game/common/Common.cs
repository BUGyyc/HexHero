using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ?????
/// </summary>
public class Common {
    public static Common instance = null;

    public static Common getInstance () {
        if (instance == null) {
            instance = new Common ();
        }
        return instance;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="str"></param>
    /// <param name="showTime"></param>
    public void showPopTip (string str, int showTime) {
        MyLog.Log ("showPopTip ---> " + str + " --- showTime ---> " + showTime);
        CEventDispatcherObj.cEventDispatcher.dispatchEvent (
            new CEvent (CEventName.SHOW_TIP, new EventCell (str, showTime)),
            this);
    }

    private void func () {

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

    /// <summary>
    /// 查找攻击目标
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static HexCell FindHexCellTarget (Dictionary<Faction, List<HexCell>> dic, HexCell sourceCell) {
        //起点
        HexCoordinates sourceCoor = sourceCell.coordinates;
        List<HexCell> targetList = dic[Global.myFaction];

        return null;
    }
}

class EventCell {
    public string str;
    public int delayTime;
    public EventCell (string str, int delayTime) {
        this.str = str;
        this.delayTime = delayTime;
    }
}