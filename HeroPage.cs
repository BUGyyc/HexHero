using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeroPage : MonoBehaviour {

    public GameObject content;
    public GameObject itemPrefab;
    public GameObject mainImg;
    private List<HeroItem> list;
    List<GameObject> messages = new List<GameObject> ();
    // public GameObject item;
    GameObject myMessage;
    GameObject parent;
    Vector3 itemLocalPos;
    Vector2 contentSize;
    float itemHeight;
    private int chooseId = 0;
    private GameObject lastChooseNode;

    private HeroItem chooseItem;
    void Start () {
        list = new List<HeroItem> ();
        parent = GameObject.Find ("heroContent");
        contentSize = parent.GetComponent<RectTransform> ().sizeDelta;
        itemHeight = 170;
        itemLocalPos = itemPrefab.transform.localPosition;
        chooseItem = null;
    }

    public void showHeroData () {
        InitData ();
        this.transform.SetAsLastSibling();
    }

    public void InitData () {
        list = HeroManager.getAllHeroItemList ();
        //WarehouseManager.getMyAllWarehouseItemList ();
        foreach (var item in list) {
            // GameObject wItem = (GameObject) Instantiate (itemPrefab);
            GameObject wItem = Instantiate (itemPrefab) as GameObject;

            wItem.GetComponent<Transform> ().SetParent (parent.GetComponent<Transform> (), false);

            Image img = wItem.GetComponent<Image> () as Image;
            string spritePath = "img/heroBmp/" + item.bmp;
            img.overrideSprite = Resources.Load (spritePath, typeof (Sprite)) as Sprite;

            wItem.transform.Find ("Text").GetComponent<Text> ().text = item.name;

            wItem.GetComponent<Button> ().onClick.AddListener (
                delegate () {
                    onClickItem (wItem, item);
                }
            );

            // wItem.gameObject.transform.SetParent (this.GetComponent<Transform> (), false);
            wItem.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
            wItem.transform.localPosition = new Vector3 (itemLocalPos.x, itemLocalPos.y - messages.Count * itemHeight, 0);
            messages.Add (wItem);

        }

        int row = (messages.Count / 5) + 1 + 6;
        if (contentSize.y <= row * itemHeight) //增加内容的高度
        {
            parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (contentSize.x, row * itemHeight);
        }
    }

    private void onClickItem (GameObject wItem, HeroItem item) {
        //隐藏上次的选中项
        if (lastChooseNode != null) {
            Image img = lastChooseNode.transform.Find ("Image").GetComponent<Image> ();
            img.gameObject.SetActive (false);
        }
        //显示这次的选中项
        wItem.transform.Find ("Image").GetComponent<Image> ().gameObject.SetActive (true);
        lastChooseNode = wItem;

        Image main = mainImg.transform.Find ("Image").GetComponent<Image> () as Image;
        string spritePath = "img/heroBmp/" + item.bmp;
        main.overrideSprite = Resources.Load (spritePath, typeof (Sprite)) as Sprite;
        //当前选中英雄
        chooseItem = item;
    }

    public void onClose () {
        lastChooseNode = null;
        this.gameObject.transform.localPosition = new Vector3 (3000, 3000, 0);
    }

    public void onClickGoBattle () {
        goBattle (chooseItem);
    }

    /// <summary>
    /// 选中英雄出征
    /// </summary>
    public void goBattle (HeroItem item) {
        this.onClose ();
        Common.getInstance ().showPopTip ("选中英雄位置", 1);
        Global.isEditMode = true;
        Global.heroItem = item;
    }
}