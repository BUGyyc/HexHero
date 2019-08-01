public class Util {
    /// <summary>
    /// 比较系统时间
    /// </summary>
    /// <param name="time"></param>
    /// <returns>-1 小于系统时间 0 等于系统时间 1大于系统时间</returns>
    public static int compareTime (string time) {
        System.DateTime dateTime = System.DateTime.Now;
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;
        string[] str = time.Split ('-');
        if (str.Length < 3) { //异常情况
            return -1;
        }
        int l_y = int.Parse (str[0]);
        int l_m = int.Parse (str[1]);
        int l_d = int.Parse (str[2]);
        //等于
        if (l_y == year && l_m == month && l_d == day) {
            return 0;
        }
        if (l_y > year) {
            return 1;
        } else {
            if (l_m > month) {
                return 1;
            } else {
                if (l_d > day) {
                    return 1;
                }
            }
        }
        return -1;
    }
}