/*
 * @Author: delevin.ying 
 * @Date: 2019-07-16 10:35:02 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-16 17:44:22
 */
using UnityEngine;
/// <summary>
/// 输出log类
/// </summary>
public class MyLog {
    public static void Log (string str, LogType type = LogType.Normal) {
        switch (type) {
            case LogType.Normal:
                Debug.Log ("Hex------ " + str);
                break;
            case LogType.Warn:
                Debug.LogWarning ("Hex------ " + str);
                break;
            case LogType.ERR:
                Debug.LogError ("Hex------ " + str);
                break;
            default:
                Debug.Log ("Hex------ " + str);
                break;
        }
    }

    // public static void Log (string str, string[] strArr, LogType type = LogType.Normal) {
    //     string line = " ";
    //     if (strArr && strArr.Length > 0) {
    //         foreach (var item in strArr) {
    //             line += item + " ";
    //         }
    //     }
    //     switch (type) {
    //         case LogType.Normal:
    //             Debug.Log ("Hex------ " + str + line);
    //             break;
    //         case LogType.Warn:
    //             Debug.LogWarning ("Hex------ " + str + line);
    //             break;
    //         case LogType.ERR:
    //             Debug.LogError ("Hex------ " + str + line);
    //             break;
    //         default:
    //             Debug.Log ("Hex------ " + str + line);
    //             break;
    //     }
    // }
}

public enum LogType {
    Normal,
    Warn,
    ERR
}