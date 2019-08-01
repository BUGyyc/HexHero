using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

    public GameObject gameObject;
    private HexGrid hexGrid;

    public GameObject hexGame;
    private HexGameUI hexGameUI;
    void Start () {
        //地图创建完成
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.INIT_COMPLETE, this.gameCreateComplete);
        hexGrid = gameObject.GetComponent<HexGrid> ();
        hexGameUI = hexGame.GetComponent<HexGameUI> ();
    }

    /// <summary>
    /// 地图创建完成，开始创建城池
    /// </summary>
    private void gameCreateComplete (CEvent evt) {
        //先创建城池
        onCreateAllFactionCity ();
        // onAutoCreateRobotHero ();

        //所有英雄、城池创建完成
        //发起回合开始
        // RoundManager.onStartRound (1, hexGrid);
        RoundManager.mInstance.onStartRound (1, hexGrid);
    }

    /// <summary>
    /// 创建每个势力方的城池
    /// </summary>
    private void onCreateAllFactionCity () {
        List<Faction> gameFactions = RoundManager.mInstance.gameFactions;
        int num = 1;
        HexCell[] cells = hexGrid.getCells ();
        while (num <= 7) {
            int index = Random.Range (0, cells.Length);
            HexCell cell = cells[index];
            if (cell && cell.IsUnderwater == false && cell.Walled == false && cell.SpecialIndex == 0) {
                cell.SpecialIndex = num;
                num++;
            }
        }
    }

    /// <summary>
    /// 创建机器人
    /// </summary>
    private void onAutoCreateRobotHero () {
        List<HeroItem> list = HeroManager.getAllHeroItemList ();
        List<HexCoordinates> coorList = this.getAutoAllPostion (hexGrid.getCells ());
        int n = 0;
        int max = coorList.Count;
        foreach (HeroItem item in list) {
            if (n >= max - 1) {
                break;
            }
            HexCell cell = hexGrid.GetCell (coorList[n]);

            if (cell == null) return;
            if (item.wl < 90) {
                continue;
            }
            //已上场
            if (item.state >= 100) {
                continue;
            }
            n++;
            item.state = 100;
            hexGameUI.AutoCreateUnit (cell, item);
        }
    }

    private List<HexCoordinates> getAutoAllPostion (HexCell[] cells) {
        List<HexCoordinates> list = new List<HexCoordinates> ();
        //在城堡附近分配每一方的英雄
        for (int i = 0; i < cells.Length; i++) {
            if (cells[i].SpecialIndex > 0) {
                HexCell temp = null;
                bool canPlace = false;
                //遍历六个方向
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    temp = cells[i].GetNeighbor (d);
                    //不能被墙包围，不能在水下
                    if (temp && temp.Walled == false && temp.IsUnderwater == false) {
                        //在这个单元上创建英雄
                        canPlace = true;
                        break;
                    }
                }
                if (canPlace == true) {
                    //可以放置英雄
                    list.Add (temp.coordinates);
                }
            }
        }

        //野外放置英雄
        for (int i = 0; i < cells.Length; i++) {
            HexCell cell = cells[i];
            if (cell.Walled == false && cell.IsUnderwater == false && cell.TerrainTypeIndex >= 2) {
                int v = Random.Range (1, 100);
                if (v >= 95) {
                    if (list.Contains (cell.coordinates) == false) {
                        list.Add (cell.coordinates);
                    }
                }
            }
        }

        return list;
    }
}