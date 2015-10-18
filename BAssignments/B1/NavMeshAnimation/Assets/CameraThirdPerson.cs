using UnityEngine;
using System.Collections;

public class CameraThirdPerson : MonoBehaviour {

    public Transform trackingObject;
    Vector3 offset;
    bool followRotation = false;
    // Use this for initialization
    void Start()
    {
        offset = transform.position - trackingObject.transform.position;
    }

    void LateUpdate()
    {
        
        float targetAngle = trackingObject.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
        //

        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, .05f);
        Vector3 rotatedOffset = rotation * offset;
        Vector3 targetPosition;
        if (followRotation)
            targetPosition = trackingObject.transform.position + rotatedOffset;
        else
            targetPosition = trackingObject.transform.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, .03f);
        transform.LookAt(trackingObject);
    }

    public void setRotationFollow(bool val)
    {
        followRotation = val;
    }
}