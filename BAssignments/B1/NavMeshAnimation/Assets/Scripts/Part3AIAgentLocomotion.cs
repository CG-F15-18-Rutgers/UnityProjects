using UnityEngine;
using System.Collections;

public class Part3AIAgentLocomotion : MonoBehaviour {

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool selected = false;
    private bool running = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        } else
        {
            animator.SetBool("Jumping", false);
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
