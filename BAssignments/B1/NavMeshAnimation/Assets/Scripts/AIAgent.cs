using UnityEngine;
using System.Collections;

public class AIAgent : MonoBehaviour {

    private NavMeshAgent navMeshAgent;
    private bool selected = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void moveTo(Vector3 position)
    {
        navMeshAgent.destination = position;
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void toggleSelected()
    {
        selected = !selected;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (selected)
        {
            renderer.material.color = Color.red;
        } else
        {
            renderer.material.color = Color.black;
        }
    }

    public bool isSelected()
    {
        return selected;
    }
}
