using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Button toggleButton;
    private CameraThirdPerson cameraScript;
    private bool rotationFollow;
	// Use this for initialization
	void Start () {
        cameraScript = Camera.main.GetComponent<CameraThirdPerson>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void toggleRotationFollow()
    {
        rotationFollow = !rotationFollow;
        cameraScript.setRotationFollow(rotationFollow);
    }
}
