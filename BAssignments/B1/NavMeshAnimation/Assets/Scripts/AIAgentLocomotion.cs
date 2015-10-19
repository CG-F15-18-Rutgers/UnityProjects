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
        float localDirection = 0;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            speed = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            localDirection = -1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            localDirection = 1;
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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            localDirection += 180;
        }
        transform.Rotate(new Vector3(0, localDirection * Time.deltaTime * 50, 0));

    }
}
