using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClick : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    public void OpenWarehousePage () {
        GameObject obj = UIManager.onOpenCommonPage (PageName.WAREHOUSE_PAGE);
        obj.GetComponent<WarehousePage> ().showWarehouseData ();
    }

    public void OpenHeroPage () {
        GameObject obj = UIManager.onOpenCommonPage (PageName.HERO_PAGE);
        obj.GetComponent<HeroPage> ().showHeroData ();
    }
}