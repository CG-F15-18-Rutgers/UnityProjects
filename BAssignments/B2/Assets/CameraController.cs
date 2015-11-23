using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject[] cameras;
    private int currCam = 0;


	// Use this for initialization
	void Start () {
        cameras = GameObject.FindGameObjectsWithTag("Camera");
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Z)) {
            currCam--;
        } else if (Input.GetKeyDown(KeyCode.X)) {
            currCam++;
        }

        Debug.Log(cameras.Length);

        int index = Mathf.Abs(currCam % cameras.Length);
        Debug.Log(index);

        foreach (GameObject camera in cameras) {
            camera.SetActive(false);
        }
        cameras[index].SetActive(true);
    }
}
