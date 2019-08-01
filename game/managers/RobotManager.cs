using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 机器人管理类
/// </summary>

public class RobotManager : MonoBehaviour {

    public static RobotManager mInstance;

    /// <summary>
    /// 所有已出场英雄
    /// </summary>
    /// <returns></returns>
    public Dictionary<Faction, List<HexCell>> showedHeroMap = new Dictionary<Faction, List<HexCell>> ();

    public GameObject hexGame;
    private HexGameUI hexGameUI;

    public GameObject hexGridObject;
    private HexGrid hexGrid;

    private int FactionPower = 0;
    private void Awake () {
        mInstance = this;

        showedHeroMap.Add (Faction.RED, new List<HexCell> ());
        showedHeroMap.Add (Faction.BLUE, new List<HexCell> ());
        showedHeroMap.Add (Faction.YELLOW, new List<HexCell> ());
        showedHeroMap.Add (Faction.GREEN, new List<HexCell> ());
        showedHeroMap.Add (Faction.BLACK, new List<HexCell> ());

        showedHeroMap.Add (Faction.WHITE, new List<HexCell> ());
        showedHeroMap.Add (Faction.GARY, new List<HexCell> ());
        showedHeroMap.Add (Faction.ORANGE, new List<HexCell> ());
        showedHeroMap.Add (Faction.PINK, new List<HexCell> ());

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        hexGameUI = hexGame.GetComponent<HexGameUI> ();
        hexGrid = hexGridObject.GetComponent<HexGrid> ();
    }

    public List<HexUnit> robotList = new List<HexUnit> ();

    public void addRobot (HexUnit obj) {
        robotList.Add (obj);
    }

    public void removeRobot (HexUnit obj) {
        robotList.Remove (obj);
    }

    // public void hasRobot(GameObject obj){
    //     // return robotList.contain
    // }

    public void removeAllRobot () {
        robotList.Clear ();
    }
    /// <summary>
    /// 放置英雄的坐标
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    public List<HexCoordinates> getRobotPosition (HexCell[] cells) {
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
        return list;
    }

    /////////////////////////////////////////////////////////////////////////////
    //AI 
    public void onFactionAILogic (Faction faction, HexGrid hexGrid) {
        //设置100行动力
        FactionPower = 100;
        //把Camera朝向城池
        HexCell city = setCameraFollowCity (faction, hexGrid);
        //AI 进行防守或者进攻
        onExcuteWorldValue (faction, city);
        //结束自己回合
    }

    /// <summary>
    /// 让镜头朝向城池
    /// </summary>
    private HexCell setCameraFollowCity (Faction faction, HexGrid hexGrid) {
        int id = FactionManager.getIdByFaction (faction);
        HexCell[] cells = hexGrid.getCells ();
        HexCell cityCell = null;
        for (int i = 0; i < cells.Length; i++) {
            if (id == cells[i].SpecialIndex) {
                cityCell = cells[i];
                break;
            }
        }
        //有城池才跟随
        if (cityCell) {
            CEventDispatcherObj.cEventDispatcher.dispatchEvent (
                new CEvent (CEventName.CAMERA_FOLLOW_TARGET, cityCell),
                this);
        }
        return cityCell;
    }

    /// <summary>
    /// 让镜头朝向英雄
    /// </summary>
    private void setCameraFollowHero () {

    }

    /// <summary>
    /// 对周围场景进行评估，判断是否防御或者进攻
    /// </summary>
    private void onExcuteWorldValue (Faction faction, HexCell city) {
        if (FactionPower <= 0) {
            //结束当前回合,通知下一方操作
            CEventDispatcherObj.cEventDispatcher.dispatchEvent (new CEvent (CEventName.NEXT_FACTION),
                this);
        }
        float value = onCheckWorldValue ();
        if (value > 0.5) {
            //执行攻击
            onAttackLogic (faction, city);
        } else {
            //执行防守
            onDefenceLogic (faction, city);
        }
    }

    /// <summary>
    /// AI进攻类的行为
    /// </summary>
    private void onAttackLogic (Faction faction, HexCell city) {
        //1.增加出场英雄
        //2.主动攻击
        List<HeroItem> factionHeroList = HeroManager.getAllShowedHeroListByFaction (faction);
        //HeroManager.getAllHeroItemListByFaction (faction);
        //已出场英雄数量
        int isShowedHeroNum = 0;
        foreach (var item in factionHeroList) {
            if (item.state == 100) {
                isShowedHeroNum++;
            }
        }
        bool isAddHero = (isShowedHeroNum < 3);
        HexCell hexCell = null;
        if (isAddHero) {
            //增加英雄
            hexCell = onPlaceFactionHero (faction, city);
        } else {
            //找到已出场的英雄
            List<HexCell> showedList = getFactionShowedList (faction);
            if (showedList != null && showedList.Count > 0) {
                hexCell = showedList[0];
            }
        }
        //英雄攻击或者移动
        if (hexCell) {
            //镜头跟随英雄
            CEventDispatcherObj.cEventDispatcher.dispatchEvent (
                new CEvent (CEventName.CAMERA_FOLLOW_HERO, hexCell.Unit),
                this);

            List<HexCell> list = hexCell.getEnemyNeighborHexCell (faction);
            bool isAttackState = false;
            HexCell target = null;
            if (list == null || list.Count < 1) {
                isAttackState = false;
            } else {
                isAttackState = true;
                //事实上应该做比较，血量比较、武力比较、收益比较
                target = list[0];
            }

            if (isAttackState) {
                //执行攻击
                hexCell.Unit.Attack (target);
            } else {

                HexCell myTargetHexCell = findMyCity (Global.myFaction);
                //执行移动
                //智能寻路
                if (myTargetHexCell) {
                    //先找到路径
                    hexGrid.FindEnablePath (hexCell, myTargetHexCell, hexCell.Unit);
                }
                if (hexCell.Unit) {
                    //执行移动
                    hexCell.Unit.Travel (hexGrid.GetPath ());
                }

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private HexCell findMyCity (Faction faction) {
        int id = FactionManager.getIdByFaction (faction);
        HexCell[] cells = hexGrid.getCells ();
        HexCell cityCell = null;
        for (int i = 0; i < cells.Length; i++) {
            if (id == cells[i].SpecialIndex) {
                cityCell = cells[i];
                break;
            }
        }
        return cityCell;
    }

    /// <summary>
    /// AI防御类的行为
    /// </summary>
    private void onDefenceLogic (Faction faction, HexCell city) {

    }

    //对英雄执行某一状态的逻辑
    private void onAutoHeroStateLogic () {
        //英雄攻击敌人

        //英雄移动到某一位置

        //英雄防御

        //英雄回城
    }

    /// <summary>
    /// 自动查找周围的目标
    /// </summary>
    private void onAutoFindTargetHexCell (int maxDistance) {

    }

    /// <summary>
    /// 执行英雄出场
    /// </summary>
    private HexCell onPlaceFactionHero (Faction faction, HexCell cityCell) {
        //是否增加场上英雄
        bool canPlaceHero = true;
        //取当前faction 下的所有英雄
        List<HeroItem> heroItemList = HeroManager.getAllHeroItemListByFaction (faction);
        HeroItem item = null;

        if (heroItemList != null && heroItemList.Count > 0) {
            //选择了武力第一
            List<HeroItem> list = HeroManager.getBestHeroListByWl (heroItemList);
            foreach (var heroItem in list) {
                if (heroItem.state != 100) {
                    item = heroItem;
                    break;
                }
            }
        }
        //放置英雄
        if (item != null) {
            if (cityCell) {
                HexCell target = null;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
                    HexCell neighbor = cityCell.GetNeighbor (d);
                    if (neighbor == null) continue;
                    if (neighbor.IsUnderwater == false && neighbor.Walled == false && neighbor.SpecialIndex <= 0) {
                        target = neighbor;
                        break;
                    }
                }
                //目标位置有效
                if (target != null) {
                    hexGameUI.AutoCreateUnit (target, item);
                    List<HexCell> list = showedHeroMap[faction];
                    list.Add (target);
                    showedHeroMap[faction] = list;
                }

                return target;
            }
        }
        return null;
    }

    /// <summary>
    /// 对环境进行评估
    /// </summary>
    /// <returns></returns>
    private float onCheckWorldValue () {
        return 1;
    }

    /// <summary>
    /// 已出场的英雄
    /// </summary>
    /// <returns></returns>
    private List<HexCell> getFactionShowedList (Faction faction) {
        return (showedHeroMap == null || showedHeroMap.Count < 1) ? null : showedHeroMap[faction];
    }
}

class CameraFollowEventObject {
    public HexCell cell;
    public CameraFollowEventObject (HexCell cell) {
        cell = cell;
    }
}