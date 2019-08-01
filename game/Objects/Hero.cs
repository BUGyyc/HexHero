using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 英雄控制类
/// </summary>
public class Hero : MonoBehaviour {
    /// <summary>
    /// 是否是机器人
    /// </summary>
    private bool isRobot;
    /// <summary>
    /// 所属势力是哪一方
    /// </summary>
    private Faction mFaction;
    /// <summary>
    /// 执行顺序的索引
    /// </summary>
    private int executeIndex;
    /// <summary>
    /// 英雄姓名
    /// </summary>
    private string heroName;
    /// <summary>
    /// 英雄属性对象
    /// </summary>
    private object detailObj;
    /// <summary>
    /// 设置英雄信息
    /// </summary>
    /// <param name="isRobot"></param>
    /// <param name="faction"></param>
    /// <param name="exeIndex"></param>
    public void setHeroImf (bool isRobot, Faction faction, int exeIndex, string name, object detailObj) {
        this.isRobot = isRobot;
        this.mFaction = faction;
        this.executeIndex = exeIndex;
        this.heroName = name;
        this.detailObj = detailObj;
    }
    /// <summary>
    /// 刷新英雄的显示
    /// </summary>
    public void updateHeroImf(){
        
    }

}