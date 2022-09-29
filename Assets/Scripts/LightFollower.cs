using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollower : MonoBehaviour
{
    public Transform ActorRoot;
    private Vector3 LightPosition;
    private Vector3 ActorPosition;
    private Vector3 LightDirection;
    private Quaternion LightRotation;


    // Start is called before the first frame update
    void Start()
    {
        LightPosition = transform.position;
        ActorPosition = ActorRoot.position;
        LightDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        LightDirection = ActorRoot.position - LightPosition;
        LightRotation = Quaternion.LookRotation(LightDirection);
        transform.rotation = LightRotation;
    }
}
