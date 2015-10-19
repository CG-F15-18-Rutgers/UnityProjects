using UnityEngine;
using System.Collections;

public class Part3AIAgentLocomotion : MonoBehaviour {

    private Animator animator;

    private Vector3 lastPos = Vector3.zero;

    private bool firstFrame = true;
    private bool firstPos = true;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        float speed = ((transform.position - lastPos).magnitude / Time.deltaTime);
        lastPos = transform.position;
        if (!firstPos)
        {
            animator.SetFloat("Speed", speed);
        }
        if (!firstFrame && speed == 0.0 && firstPos)
        {
            firstPos = false;
        }
        firstFrame = false;
        float direction = transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, direction, 0);

    }
}
