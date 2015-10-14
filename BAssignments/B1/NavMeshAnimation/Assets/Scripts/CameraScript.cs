using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    public KeyCode
        rotate_left = KeyCode.A,
        rotate_right = KeyCode.D,
        forward = KeyCode.W,
        backward = KeyCode.S,
        up = KeyCode.E,
        down = KeyCode.Q;

    // Use this for initialization
    void Start () {
	}
	
	void Update () {
        Vector3 positionDifference = new Vector3();
        if (Input.GetKey(forward)) positionDifference.z = 1;
        if (Input.GetKey(backward)) positionDifference.z = -1;
        if (Input.GetKey(up)) positionDifference.y = 1;
        if (Input.GetKey(down)) positionDifference.y = -1;
        if (Input.GetKey(rotate_left)) positionDifference.x = -1;
        if (Input.GetKey(rotate_right)) positionDifference.x = 1;
        this.transform.position += positionDifference;
	}

}
