using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

    // public static int currMaxPageIndex = 0;
    public static UIManager instance = null;
    //???UI??????????
    public static Dictionary<string, GameObject> pageDic;
    public GameObject cityNodeUI;

    private void Awake () {
        pageDic = new Dictionary<string, GameObject> ();
        instance = this;
        var children = GameObject.FindGameObjectsWithTag ("Page");
        foreach (var item in children) {
            pageDic.Add (item.name, item);
        }
    }


    void Start () {
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.OPEN_TOWN_UI, this.onOpenTownUI);
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.CLOSE_TOWN_UI, this.onCloseTownUI);
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.OPEN_COMMON_PAGE, this.onOpenCommonPage);
    }

    private void onOpenCommonPage (CEvent evt) {
        PageEventObject pObj = (PageEventObject) evt.eventParams;
        string pageName = pObj.pageName;
        GameObject obj = pageDic[pageName];
        if (obj == null) {
            throw new Exception ();
        } else {
            obj.transform.position = new Vector3 (0, 0, 0);
            obj.SetActive (true);
        }
    }

    public static GameObject onOpenCommonPage (string pageName, OpenType type = OpenType.TOP) {
        GameObject obj = pageDic[pageName];
        if (obj == null) {
            throw new Exception ();
        } else {
            obj.transform.localPosition = new Vector3 (0, 0, 0);
            obj.SetActive (true);
            if (type == OpenType.TOP) {
                // obj.layer = currMaxPageIndex;
                // currMaxPageIndex++;
            }
        }
        return obj;
    }

    private void onOpenTownUI (CEvent evt) {
        if (cityNodeUI) cityNodeUI.SetActive (true);
    }

    private void onCloseTownUI (CEvent evt) {
        if (cityNodeUI) cityNodeUI.SetActive (false);
    }
}

/// <summary>
/// ???????
/// </summary>
public enum OpenType {
    NORMAL, //??
    TOP //??
}