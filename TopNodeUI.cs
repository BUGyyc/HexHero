using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TopNodeUI : MonoBehaviour {

    public Text coin;
    public Text zs;
    public Text tl;
    public Text hp;
    public Text time;
    public Text weapon;
    public Text power;
    // Start is called before the first frame update
    void Start () {
        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.UPDATE_TOP_UI, this.onUpdateTopUI);
        initView ();
    }

    private void initView () {
        PlayerItem item = FactionManager.getMyFactionPlayerImf ();
        if (item == null) return;
        TopUIEventObject obj = new TopUIEventObject (item.coin, item.zs, item.tl, item.hp, item.time, item.weapon, item.power);
        updateData (obj);
    }

    private void onUpdateTopUI (CEvent et) {

    }

    /// <summary>
    /// 更新顶部UI
    /// </summary>
    /// <param name="obj"></param>
    private void updateData (TopUIEventObject obj) {
        // text.string
        coin.GetComponent<Text> ().text = "x" + obj.coin;
        zs.GetComponent<Text> ().text = "x" + obj.zs;
        tl.GetComponent<Text> ().text = "x" + obj.tl;
        hp.GetComponent<Text> ().text = "x" + obj.hp;
        time.GetComponent<Text> ().text = "x" + obj.time;
        weapon.GetComponent<Text> ().text = "x" + obj.weapon;
        power.GetComponent<Text> ().text = "x" + obj.power;
    }

    // Update is called once per frame
    void Update () {

    }
}