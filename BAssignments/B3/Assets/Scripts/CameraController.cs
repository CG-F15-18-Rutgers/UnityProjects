using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 delta = new Vector3(0, x, 0);
        
        transform.Rotate(delta);

        float y = transform.position.y;
        transform.Translate(z != 0.0f? transform.forward*(z/10.0f): Vector3.zero);
	}
}
