using UnityEngine;
using System.Collections;

public class AIAgentLocomotion : MonoBehaviour {

    private Animator animator;

    private Vector3 lastPos = Vector3.zero;
    private float speed = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
	}

    void FixedUpdate()
    {
        speed = ((transform.position - lastPos).magnitude / Time.deltaTime);
        lastPos = transform.position;
        if(speed != 0)
        {
            Debug.Log(speed);
        }
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("Jumping", true);
        } else
        {
            animator.SetBool("Jumping", false);
        }

        animator.SetFloat("Speed", speed);
        transform.eulerAngles = new Vector3(0, direction, 0);

    }
}
