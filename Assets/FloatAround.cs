using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAround : MonoBehaviour
{
    float originalX;
    float originalY;
    float originalZ;

    public float floatStrength = 0.05f; // You can change this in the Unity Editor to 
                                        // change the range of y positions that are possible.
    private float x_str;
    private float y_str;
    private float z_str;

    public float rotation_max = 15.0f;

    Vector3 rotation;
    float rotation_speed;

    void Start()
    {
        x_str = Random.Range(-floatStrength, floatStrength);
        y_str = Random.Range(-floatStrength, floatStrength);
        z_str = Random.Range(-floatStrength, floatStrength);

        this.originalX = this.transform.position.x;
        this.originalY = this.transform.position.y;
        this.originalZ = this.transform.position.z;

        rotation = new Vector3(Random.Range(-rotation_max, rotation_max), Random.Range(-rotation_max, rotation_max), Random.Range(-rotation_max, rotation_max));
        rotation_speed = Random.Range(-rotation_max, rotation_max);
    }

    void Update()
    {
        transform.position = new Vector3(originalX + ((float)Mathf.Sin(Time.time) * x_str)
                                       , originalY + ((float)Mathf.Sin(Time.time) * y_str)
                                       , originalZ + ((float)Mathf.Sin(Time.time) * z_str));

        transform.Rotate(rotation * Time.deltaTime * 5);
    }
}
