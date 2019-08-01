/*
 * @Author: delevin.ying 
 * @Date: 2019-07-17 16:53:15 
 * @Last Modified by: delevin.ying
 * @Last Modified time: 2019-07-17 18:56:47
 */
using System;
using System.Collections;
[Serializable]
public class ActImf {
    // public string jsonName;
    // public string version;
    // public bool active;
    // public string app_version;
    public ActItem[] actLists;
}

[Serializable]
public class ActItem {
    public string name;
    public int id;
    public string start_time;
    public string end_time;
    public string isVip;
    public string isClicked;
    public string extra;
}