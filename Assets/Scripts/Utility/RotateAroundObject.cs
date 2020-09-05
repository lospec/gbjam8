using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public Vector3 rotationVector;
    public float speed;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.position, rotationVector, speed);
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
