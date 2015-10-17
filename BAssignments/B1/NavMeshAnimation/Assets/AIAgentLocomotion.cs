using UnityEngine;
using System.Collections;

public class AIAgentLocomotion : MonoBehaviour {

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float speed = 0;
        float direction = transform.eulerAngles.y;

        if (Input.GetKey(KeyCode.W))
        {
            speed = 1;
            direction = 0;
        } else if (Input.GetKey(KeyCode.A))
        {
            speed = 1;
            direction = 90;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speed = 1;
            direction = 180;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            speed = 1;
            direction = 270;
        }

        if (speed == 1 && Input.GetKey(KeyCode.LeftShift))
        {
            speed = 2;
        }

        animator.SetFloat("Speed", speed);
        transform.eulerAngles = new Vector3(0, direction, 0);

    }
}
