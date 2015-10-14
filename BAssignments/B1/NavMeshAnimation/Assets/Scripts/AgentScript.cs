using UnityEngine;
using System.Collections;

public class AgentScript : MonoBehaviour {

    public float thrust = 500;
    public KeyCode left = KeyCode.A, right = KeyCode.D, up = KeyCode.W, down = KeyCode.S;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();   
	}
	
    // Update is called every frame
    void Update ()
    {
        float moveHorizontal = Input.GetKey(left) ? -1 : 0;
        moveHorizontal = Input.GetKey(right) ? 1 : moveHorizontal;
        float moveVertical = Input.GetKey(up) ? 1 : 0;
        moveVertical = Input.GetKey(down) ? -1 : moveVertical;
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        rigidBody.AddForce(Time.deltaTime * movement * thrust);
    }

	// FixedUpdate is called every fixed framerate frame
	void FixedUpdate () {
        
    }

    private Rigidbody rigidBody;
}
