using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieMovement : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        speed = 50;
    }

    void FixedUpdate()
    {
        float moveHorizonal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        var v1 = new Vector3(moveHorizonal, 0.0f, moveVertical);

        rigidBody.AddForce(v1 * speed);
    }
}
