using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    // Start is called before the first frame update

    Transform m_Camera;
    public GameObject hero;
    public GameObject text;
    public GameObject hp;

    private GameObject HexCamera;
    Transform swivel, stick;
    void Start () {
        m_Camera = Camera.main.transform;
        HexCamera = GameObject.Find ("Hex Map Camera");
        swivel = HexCamera.transform.GetChild (0);
        stick = swivel.GetChild (0);
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate () {
        if (m_Camera == null) return;
        transform.rotation = Quaternion.LookRotation (transform.position - m_Camera.position);

    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update () {
        Vector2 position = Camera.main.WorldToScreenPoint (hero.transform.position);
        // MyLog.Log ("Billboard    ---------     Update  " + stick.position + "   " + swivel.position + "   " + swivel.rotation);

        float offset = stick.position.y / 3 + 150;
        // transform.position = position;
        text.transform.position = new Vector2 (position.x, position.y + offset);
        hp.transform.position = new Vector2 (position.x, position.y + offset - 30);

        // MyLog.Log ("屏幕高度  " + Camera.main.transform.parent.transform.position.z);

    }
}