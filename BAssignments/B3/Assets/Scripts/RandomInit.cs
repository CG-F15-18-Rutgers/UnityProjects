using UnityEngine;
using System.Collections;

public class RandomInit : MonoBehaviour {
    public Transform spawnpoint1;
    public Transform spawnpoint2;
    public Transform spawnpoint3;

    // Use this for initialization
    void Start () {
        Transform[] points = new Transform[3];
        points[0] = spawnpoint1;
        points[1] = spawnpoint2;
        points[2] = spawnpoint3;
        transform.position = points[Random.Range(0, 3)].position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
