using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollViewScript : MonoBehaviour {
    public GameObject itemPrefab;
    public ScrollRect scroll;
    public GridLayoutGroup grid;
    public int AllCount = 30;

    public HeroImf allHero;
    public object[] heroList;
    private void Start () {
        //拿所有英雄的数据
        // allHero = DataManager.allHeroImf;
        // heroList = allHero.heros;
        // this.initScrollItem ();
    }

    private void Update () {

    }

    private void initScrollItem () {
        for (int i = 0; i < heroList.Length; i++) {
            GameObject item = Instantiate (itemPrefab, grid.transform);
            // Image img = GameObject.Find("pic").GetComponent<Image>();
            Text t = GameObject.Find("nick").GetComponent<Text>();
            // t.set
        }

        float colums = grid.constraintCount; //分1列显示
        int rows = Mathf.CeilToInt (grid.transform.childCount / colums);
        //设置grid的大小，如果grid太小，则无法滚动，因此grid的大小需要和所有字对象的大小保持一致
        grid.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (grid.cellSize.y + grid.spacing.y) * rows);
        grid.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (grid.cellSize.x + grid.spacing.x) * colums);
        scroll.verticalNormalizedPosition = 1; //初始化scroll的位置
        scroll.horizontalNormalizedPosition = 0;
    }

    // private int test = 0;
    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // private void OnGUI() {
    //     // Debug.Log("OnGUI hello "+(test++));
    // }

    // // OnDraw

    // /// <summary>
    // /// 绘制listView内容
    // /// </summary>
    // private void drawListView(){

    // }

    // /// <summary>
    // /// 给listView初始化数据
    // /// </summary>
    // private void initListViewData(){

    // }
}