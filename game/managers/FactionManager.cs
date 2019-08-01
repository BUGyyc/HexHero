public class FactionManager {

    /// <summary>
    /// 所有玩家信息
    /// </summary>
    public static PlayerImf allPlayerImf;

    public static void InitFactionManager () {
        //加载各方信息
        loadAllFactionImf ();
    }
    /// <summary>
    /// 加载各方势力的信息
    /// </summary>
    public static void loadAllFactionImf () {
        allPlayerImf = ConfigUtils.loadAllPlayerConfig ();
        MyLog.Log ("ConfigUtils  allPlayerImf = " + allPlayerImf);
    }

    /// <summary>
    /// 获取我方玩家信息
    /// </summary>
    public static PlayerItem getMyFactionPlayerImf (int id = 1) {
        PlayerItem item = getPlayerImfById (id);
        return item;
    }

    /// <summary>
    /// 通过Id 获取某一方的信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static PlayerItem getPlayerImfById (int id) {
        foreach (var playerItem in allPlayerImf.allPlayers) {
            if (playerItem.id == id) {
                return playerItem;
            }
        }
        return null;
    }

    // RED = 0,
    // BLUE = 1,
    // YELLOW = 2,
    // GREEN = 3,
    // BLACK = 4,
    // WHITE = 5,
    // GARY = 6,
    // ORANGE = 7,
    // PINK = 8,

    public static int getIdByFaction (Faction faction) {
        switch (faction) {
            case Faction.RED:
                return 1;
            case Faction.BLUE:
                return 2;
            case Faction.YELLOW:
                return 3;
            case Faction.GREEN:
                return 4;
            case Faction.BLACK:
                return 5;
            case Faction.WHITE:
                return 6;
            case Faction.GARY:
                return 7;
            case Faction.ORANGE:
                return 8;
            default:
                return 1;
        }
    }

    public static string getNameByFaction (Faction faction) {
        switch (faction) {
            case Faction.RED:
                return "红方";
            case Faction.BLUE:
                return "蓝方";
            case Faction.YELLOW:
                return "黄方";
            case Faction.GREEN:
                return "绿方";
            case Faction.BLACK:
                return "黑方";
            case Faction.WHITE:
                return "白方";
            case Faction.GARY:
                return "灰方";
            case Faction.ORANGE:
                return "橙方";
            default:
                return "**";
        }
    }

    public static void DestroyManager () { }
}