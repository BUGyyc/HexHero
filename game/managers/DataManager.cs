public class DataManager {

    /// <summary>
    /// 初始化所有数据
    /// </summary>
    public static void InitAllData () {
        ClearAllData ();
        //加载英雄
        HeroManager.InitHeroManager ();
        //加载物品
        ItemManager.InitItemManager ();
        //加载各方数据
        FactionManager.InitFactionManager ();
        //加载仓库数据
        WarehouseManager.InitWarehouseManager ();
    }

    public static void ClearAllData () {
        HeroManager.DestroyManager ();
        FactionManager.DestroyManager ();
        WarehouseManager.DestroyManager ();
    }
}