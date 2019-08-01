/*
 * @Author: delevin.ying 
 * @Date: 2019-05-28 16:16:43 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-19 10:59:55
 */
// using GameFramework.Event;
/// <summary>
/// 游戏管理类
/// </summary>
public class GameManager {
    /// <summary>
    /// 初始化游戏
    /// </summary>
    public static void InitGame () {
        MyLog.Log ("GameManager -- InitGame");
        //初始化所有数据
        DataManager.InitAllData ();
        //预加载要用上的module
        ModuleManager.InitAllModule ();
        //初始化回合管理器，设定7个势力方
        // RoundManager.initAllFaction(7);
        
    }

    private void loadComplete () {

    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public static void StartGame () {

    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public static void endGame () {

    }
    /// <summary>
    /// 获取执行力
    /// </summary>
    /// <returns></returns>
    public static int getMovePower (Faction faction) {
        return 0; //
    }
}