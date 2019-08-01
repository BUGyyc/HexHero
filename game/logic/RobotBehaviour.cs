using UnityEngine;

public class RobotBehaviour : MonoBehaviour {
    private void Awake () {
        MyLog.Log ("Robot -- awake");
    }

    private void Start () {
        MyLog.Log ("Robot -- start");
    }
}