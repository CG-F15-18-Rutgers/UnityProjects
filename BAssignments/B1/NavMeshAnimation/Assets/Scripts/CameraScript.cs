using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    public GameObject player;
	// Use this for initialization
	void Start () {
        offset = this.transform.position - player.transform.position;
	}
	
	void LateUpdate () {
        this.transform.position = player.transform.position + offset;
	}

    private Vector3 offset;
}
