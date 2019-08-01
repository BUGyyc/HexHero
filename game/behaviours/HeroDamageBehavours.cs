using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 英雄受到伤害的行为类
/// </summary>
public class HeroDamageBehavours : MonoBehaviour {
    private Vector3 mTarget;
    private Vector3 mScreen;
    public int value;
    public float ContentHeight = 100f;
    public float ContentWidth = 3f;
    public float ContentSpeed = 10f;
    private Vector2 mPoint;
    public float FreeTime = 5f;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        mTarget = transform.position;
        mScreen = Camera.main.WorldToScreenPoint (mTarget);
        mPoint = new Vector2 (mScreen.x, Screen.height - mScreen.y);
        StartCoroutine (DestroyDamage ());
    }

    IEnumerator DestroyDamage () {
        yield return new WaitForSeconds (1f);
        Destroy (this.gameObject);
    }

    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI () {
        if (mScreen.z > 0) {
            GUI.color = Color.red;
            GUI.skin.label.fontSize = 25;
            GUI.Label (new Rect (mPoint.x, mPoint.y, ContentWidth, ContentHeight), value.ToString ());
        }
    }

    private void Update () {
        transform.Translate (Vector3.up * 0.5F * Time.deltaTime);
        //重新计算坐标  
        mTarget = transform.position;
        //获取屏幕坐标  
        mScreen = Camera.main.WorldToScreenPoint (mTarget);
        //将屏幕坐标转化为GUI坐标  
        mPoint = new Vector2 (mScreen.x, Screen.height - mScreen.y);
    }
}