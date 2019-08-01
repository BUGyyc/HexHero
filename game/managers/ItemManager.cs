public class ItemManager {

    public static ItemImf itemImf;
    public static void InitItemManager () {
        loadAllItemImf ();
    }
    public static void loadAllItemImf () {
        itemImf = ConfigUtils.loadAllItemConfig ();
        MyLog.Log ("configUtils  加载物品");
    }

}