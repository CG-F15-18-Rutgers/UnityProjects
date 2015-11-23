using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 delta = new Vector3(x, 0, z);
        this.transform.position += delta/3;
	}
}
