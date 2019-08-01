using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillBroadcastPanel : MonoBehaviour {

    GameObject parent;
    Vector3 itemLocalPos;
    Vector2 contentSize;
    float itemHeight;
    public GameObject itemPrefab;
    List<GameObject> messages = new List<GameObject> ();
    private List<SkillListAdapter> dataList;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        dataList = new List<SkillListAdapter> ();
        parent = GameObject.Find ("skillBroadcastContent");
        contentSize = parent.GetComponent<RectTransform> ().sizeDelta;
        itemHeight = 80;
        itemLocalPos = itemPrefab.transform.localPosition;

        CEventDispatcherObj.cEventDispatcher.addEventListener (CEventName.UPDATE_SKILL_BROADCAST_PANEL, this.OnUpdateListView);
    }

    private void OnUpdateListView (CEvent evt) {
        SkillListAdapter adapter = new SkillListAdapter (1, "水淹七军", "关羽", "曹仁"); //evt.eventParams as SkillListAdapter;
        dataList.Add (adapter);
        // setListDataByAdapter (adapter);
        updateListView ();
    }

    public void setListData (int id, string skillName, string fromHeroName, string toHeroName) {
        SkillListAdapter adapter = null;
        if (string.IsNullOrEmpty (toHeroName)) {
            adapter = new SkillListAdapter (id, skillName, fromHeroName);
        } else {
            adapter = new SkillListAdapter (id, skillName, fromHeroName, toHeroName);
        }
        dataList.Add (adapter);
    }

    // public void setListDataByAdapter (SkillListAdapter adapter) {
    //     list.Add (adapter);
    // }

    public void updateListView () {
        foreach (var adapter in dataList) {
            // GameObject wItem = (GameObject) Instantiate (itemPrefab);
            GameObject wItem = Instantiate (itemPrefab) as GameObject;

            wItem.GetComponent<Transform> ().SetParent (parent.GetComponent<Transform> (), false);
            //技能图片ID 
            int imgId = adapter.id;
            //设置技能图片
            Image img = wItem.transform.Find ("Image").GetComponent<Image> () as Image;
            string spritePath = "img/threekingdom/skills" + imgId;
            img.overrideSprite = Resources.Load (spritePath, typeof (Sprite)) as Sprite;

            string skillName = adapter.skillName;
            string fromHeroName = adapter.fromHeroName;
            string toHeroName = adapter.toHeroName;

            wItem.transform.Find ("line1").GetComponent<Text> ().text = fromHeroName + " 发动技能 " + skillName;
            wItem.transform.Find ("line2").GetComponent<Text> ().text = "对 " + toHeroName + " 造成伤害";

            wItem.gameObject.transform.localPosition = new Vector3 (0, 0, 0);
            wItem.transform.localPosition = new Vector3 (itemLocalPos.x, itemLocalPos.y - messages.Count * itemHeight, 0);
            messages.Add (wItem);
        }

        int row = (messages.Count / 5) + 1 + 3;
        if (contentSize.y <= row * itemHeight) //增加内容的高度
        {
            parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (contentSize.x, row * itemHeight);
        }
    }
}

class SkillListAdapter {
    public int id;
    public string skillName;
    public string fromHeroName;
    public string toHeroName;

    public SkillListAdapter (int id, string skillName, string fromHeroName) {
        this.id = id;
        this.skillName = skillName;
        this.fromHeroName = fromHeroName;
    }

    public SkillListAdapter (int id, string skillName, string fromHeroName, string toHeroName) {
        this.id = id;
        this.skillName = skillName;
        this.fromHeroName = fromHeroName;
        this.toHeroName = toHeroName;
    }
}