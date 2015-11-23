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
        /*if (Input.GetKeyDown(KeyCode.Z)) {
            currCam--;
        } else if (Input.GetKeyDown(KeyCode.X)) {
            currCam++;
        }


        int index = Mathf.Abs(currCam % cameras.Length);

		SetCamera (index);
		*/
    }

	public void SetCamera(int index) {
		foreach (GameObject camera in cameras) {
			camera.SetActive(false);
		}
		cameras[index].SetActive(true);
	}
}
