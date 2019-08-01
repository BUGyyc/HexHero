using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUI : MonoBehaviour {

    // [SerializeField] TextPooler TextPool;
    // Start is called before the first frame update
    public GameObject popDamageText;
    private Camera camera;
    private string name = "Hero--------->";

    Transform m_Camera;
    public GameObject HeroCanvas;
    public GameObject text;
    public GameObject hp;
    private GameObject HexCamera;
    Transform swivel, stick;
    private HeroItem heroItem;
    void Start () {
        camera = Camera.main;

        m_Camera = Camera.main.transform;
        HexCamera = GameObject.Find ("Hex Map Camera");
        swivel = HexCamera.transform.GetChild (0);
        stick = swivel.GetChild (0);

        // this.showHeroImf ("");w
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate () {
        if (m_Camera == null) return;
        HeroCanvas.transform.rotation = Quaternion.LookRotation (HeroCanvas.transform.position - m_Camera.position);
    }

    public void setHeroItem (HeroItem item) {
        heroItem = item;
    }

    public HeroItem GetHeroItem () {
        return heroItem;
    }

    public void updateHeroImf () {
        text.GetComponent<Text> ().text = "" + heroItem.name;
    }

    public void showHeroImf (string str) {
        text.GetComponent<Text> ().text = "" + str;
    }

    /// <summary>
    /// 展示伤害
    /// </summary>
    /// <param name="amount"></param>
    public void PopDamageText (float amount) {
        popDamageText.GetComponent<Text> ().text = "" + amount;
        StartCoroutine (DelayDisposeText (popDamageText));
    }

    IEnumerator DelayDisposeText (GameObject textObj) {
        yield return new WaitForSeconds (2f);
        textObj.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        Vector2 position = Camera.main.WorldToScreenPoint (this.gameObject.transform.position);
        // MyLog.Log ("Billboard    ---------     Update  " + stick.position + "   " + swivel.position + "   " + swivel.rotation);

        float offset = stick.position.y / 3 + 150;
        // transform.position = position;
        text.transform.position = new Vector2 (position.x, position.y + offset);
        hp.transform.position = new Vector2 (position.x, position.y + offset - 30);
    }
}