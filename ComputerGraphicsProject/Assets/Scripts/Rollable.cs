using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rollable : MonoBehaviour
{
    protected Rigidbody rigidBody;
    private float initialVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        initialVelocity = 100;
    }

    // Remove die when it leaves field of view
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void setInitialVelocity(float initialVelocity)
    {
        this.initialVelocity = initialVelocity;
    }

    // returns the result of the rolled object
    public abstract int GetValue();
}
