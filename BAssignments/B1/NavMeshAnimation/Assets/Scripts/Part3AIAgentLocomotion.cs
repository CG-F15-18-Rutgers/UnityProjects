using UnityEngine;
using System.Collections;

public class Part3AIAgentLocomotion : MonoBehaviour {

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool selected = false;
    private bool running = false;
    private bool jumping = false;
    private int jumpTimer = 0;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoTraverseOffMeshLink = false;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            navMeshAgent.speed = 4;
        } else
        {
            navMeshAgent.speed = 2;
        }
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        if (navMeshAgent.isOnOffMeshLink)
        {
            animator.SetBool("Jumping", true);
            if(!jumping)
            {
                jumpTimer = 70;
            }
            jumping = true;
        } else
        {
            animator.SetBool("Jumping", false);
            jumping = false;
        }
        if(jumpTimer > 0)
        {
            jumpTimer--;
            Debug.Log(jumpTimer);
            if(jumpTimer == 0)
            {
                navMeshAgent.CompleteOffMeshLink();
            }
        }
    }

    public void moveTo(Vector3 position)
    {
        running = false;
        navMeshAgent.destination = position;
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void toggleSelected()
    {
        selected = !selected;
        transform.Find("Indicator").gameObject.SetActive(selected);
    }

    public bool isSelected()
    {
        return selected;
    }

    public void unselectedClick()
    {
        running = true;
    }
}
