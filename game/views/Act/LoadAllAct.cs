using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class LoadAllAct : MonoBehaviour {

    public ActImf allActImf;
    public ActImf allEnableActImf;

    public Sprite[] mSprite;

    public GameObject actBtnItem;
    /// <summary>
    /// 展示所有可见活动
    /// </summary>
    /// <returns></returns>
    public ActImf getAllEnableActImf () {
        MyLog.Log ("执行 ----getAllEnableActImf");
        //读取所有活动信息
        allActImf = ConfigUtils.loadAllActConfig ();
        allEnableActImf = allActImf;
        return allActImf;
        ///
        // if (allActImf == null) return null;
        // //可见活动
        // ActImf enableActImf = new ActImf ();
        // enableActImf.jsonName = allActImf.jsonName;
        // enableActImf.active = allActImf.active;
        // enableActImf.app_version = allActImf.app_version;
        // enableActImf.version = allActImf.version;
        // List<ActItem> actList = new ArrayList<> ();
        // //根据时间比较
        // foreach (var item in allActImf.allList) {
        //     int type = Util.compareTime (item.end_time);
        //     if (type < 1) {
        //         //时间满足
        //         actList.Add (item);
        //     }
        // }
        // enableActImf.allList = actList;
        // allEnableActImf = enableActImf;
        // return enableActImf;
    }

    /// <summary>
    /// 展示可见的活动
    /// </summary>
    public void onShowEnableAct () {
        getAllEnableActImf ();
        // int index = 0;
        MyLog.Log ("执行 ----onShowEnableAct   allEnableActImf.allList = " + allEnableActImf.actLists);
        /// <summary>
        /// 展示可见活动图标
        /// </summary>
        /// <value></value>
        foreach (var item in allEnableActImf.actLists) {
            GameObject g = (GameObject) Instantiate (actBtnItem);
            GameObject imgObj = GameObject.Find ("img");
            Image img = imgObj.GetComponent<Image> ();
            img.sprite = mSprite[item.id];
            // index++;
            // img.s
            Button btn = g.GetComponent<Button> ();
            btn.onClick.AddListener (delegate () {
                OnClickActItem (item.id, item.name);
            });
            // btn.
            g.transform.SetParent (transform, false);
        }
    }

    private void OnClickActItem (int actId, string actName) {
        MyLog.Log ("点击活动 ----- actId = " + actId + "   actName=  " + actName);
    }
}