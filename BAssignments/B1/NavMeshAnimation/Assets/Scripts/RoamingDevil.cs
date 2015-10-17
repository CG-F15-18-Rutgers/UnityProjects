using UnityEngine;
using System.Collections;

public class RoamingDevil : MonoBehaviour {

    public GameObject markerA;
    public GameObject markerB;
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 positionA = markerA.transform.position;
        Vector3 positionB = markerB.transform.position;

        Vector3 targetPosition = (direction == "A") ? positionA : positionB;
        Vector3 targetDirection = targetPosition - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        float speed = 5f;
        float threshold = 1;

        transform.position += targetDirection * Time.deltaTime * speed;
        Vector3 difference = transform.position - targetPosition;
        difference.y = 0;
        if (difference.sqrMagnitude < threshold)
        {
            switchDirection();
        }
	}

    private void switchDirection()
    {
        Debug.Log("Switching");
        if (direction == "A") direction = "B";
        else direction = "A";
    }

    private string direction = "A";
}
