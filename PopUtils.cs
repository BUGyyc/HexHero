using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 弹提示工具类
/// </summary>
public class PopUtils : MonoBehaviour {
    public Image popTip;
    public Text text;
    private int deTime = 0;
    void Start () {
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.SHOW_TIP, this.showTip);
    }

    private void OnDestroy () {
        CEventDispatcherObj.cEventDispatcher.removeEventListener (CEventName.SHOW_TIP, this.showTip);
    }

    private void showTip (CEvent evt) {
        EventCell ec = evt.eventParams as EventCell;
        Debug.Log ("展示消息 --- " + evt + " --- " + ec.str + "    == " + ec.delayTime);
        object evtParams = evt.eventParams;
        // popTip.enabled = true;
        popTip.gameObject.SetActive (true);
        popTip.gameObject.SetActive (true);
        text.GetComponent<Text> ().text = "" + ec.str;
        // delTime = ec.delayTime;
        Invoke ("onHideTip", ec.delayTime);
    }

    private void onHideTip () {
        popTip.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {

    }
}