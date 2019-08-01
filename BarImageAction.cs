using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarImageAction : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        iTween.RotateBy (gameObject, iTween.Hash ("z", 1, "easeType", "easeInOutBack", "loopType", "loop", "delay", 1));
        // iTween.MoveBy (gameObject, iTween.Hash ("x", 50, "easeType", "easeInOutExpo", "loopType", "loop", "delay", .1));
    }

    // Update is called once per frame
    void Update () {

    }
}