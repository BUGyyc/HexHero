using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WarehousePage : MonoBehaviour {
    public GameObject content;
    public GameObject itemPrefab;
    public GameObject mainImg;
    private List<WarehouseItem> list;
    List<GameObject> messages = new List<GameObject> ();
    // public GameObject item;
    GameObject myMessage;
    GameObject parent;
    Vector3 itemLocalPos;
    Vector2 contentSize;
    float itemHeight;
    private int chooseId = 0;
    private GameObject lastChooseNode;
    void Start () {
        list = new List<WarehouseItem> ();
        parent = GameObject.Find ("warehouseContent");
        contentSize = parent.GetComponent<RectTransform> ().sizeDelta;
        itemHeight = 105; //itemPrefab.GetComponent<RectTransform> ().rect.height;
        itemLocalPos = itemPrefab.transform.localPosition;
    }

    public void showWarehouseData () {
        InitData ();
    }

    public void InitData () {
        list = WarehouseManager.getMyAllWarehouseItemList ();
        foreach (var item in list) {
            // GameObject wItem = (GameObject) Instantiate (itemPrefab);
            GameObject wItem = Instantiate (itemPrefab) as GameObject;

            wItem.GetComponent<Transform> ().SetParent (parent.GetComponent<Transform> (), false);

            Image img = wItem.GetComponent<Image> () as Image;
            string spritePath = "img/threekingdom/item/item" + item.id;
            img.overrideSprite = Resources.Load (spritePath, typeof (Sprite)) as Sprite;

            wItem.transform.Find ("Text").GetComponent<Text> ().text = "x" + item.num;

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

        int row = (messages.Count / 7) + 1 + 6;
        if (contentSize.y <= row * itemHeight) //增加内容的高度
        {
            parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (contentSize.x, row * itemHeight);
        }

    }

    private void onClickItem (GameObject wItem, WarehouseItem item) {
        //隐藏上次的选中项
        if (lastChooseNode != null) {
            Image img = lastChooseNode.transform.Find ("Image").GetComponent<Image> ();
            img.gameObject.SetActive (false);
        }
        //显示这次的选中项
        wItem.transform.Find ("Image").GetComponent<Image> ().gameObject.SetActive (true);
        lastChooseNode = wItem;

        Image main = mainImg.transform.Find ("Image").GetComponent<Image> () as Image;
        string spritePath = "img/threekingdom/item/item" + item.id;
        main.overrideSprite = Resources.Load (spritePath, typeof (Sprite)) as Sprite;

    }

    public void onClose () {
        lastChooseNode = null;
        this.gameObject.transform.localPosition = new Vector3 (3000, 3000, 0);
    }
}