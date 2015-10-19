using UnityEngine;
using System.Collections;

public class Part3AIAgentDirector : MonoBehaviour {

    private Part3AIAgentLocomotion[] agents;

    // Use this for initialization
    void Start()
    {
        GameObject[] agentObjects = GameObject.FindGameObjectsWithTag("AIAgent");
        agents = new Part3AIAgentLocomotion[agentObjects.Length];
        int i = 0;
        foreach (GameObject agent in agentObjects)
        {
            agents[i++] = agent.GetComponent<Part3AIAgentLocomotion>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                // Check if the raycast hit an agent body.
                bool agentHit = false;
                foreach (Part3AIAgentLocomotion agent in agents)
                {
                    if (agent.getTransform() == hit.transform)
                    {
                        // Select this agent
                        agent.toggleSelected();
                        agentHit = true;
                        break;
                    }
                }

                // If an agent wasn't selected, then move all selected agents to the hit point.
                if (!agentHit)
                {
                    bool agentSelected = false;
                    foreach (Part3AIAgentLocomotion agent in agents)
                    {
                        if (agent.isSelected())
                        {
                            agent.moveTo(hit.point);
                            agent.toggleSelected();
                            agentSelected = true;
                        }
                    }
                    
                    if (!agentSelected)
                    {
                        foreach (Part3AIAgentLocomotion agent in agents)
                        {
                            agent.unselectedClick();
                        }
                    }
                }
            }
        }
    }
}
