using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum togglableObject
    {
        Die
    }

    // die object that the player can create
    public Rigidbody dieObject;

    // starting point of created object
    public int objectCreationPositionX;
    public int objectCreationPositionY;
    public int objectCreationPositionZ;

    private int currentDieResult;

    private float objectReleaseIntensity;


    // type of object being created
    private togglableObject objectSelection;

    private const float defaultObjectReleaseIntensity = 30;
    private const float intensityReleaseChangePerFrame = 5;

    // Start is called before the first frame update
    void Start()
    {
        // set to die by default
        objectSelection = togglableObject.Die;
        objectReleaseIntensity = defaultObjectReleaseIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        // user is throwing an object
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Rigidbody objectToBeCreated = instantiateSelectedObject();

            objectToBeCreated.AddForce(new Vector3(0.0f, 0.0f, objectReleaseIntensity));

            objectReleaseIntensity = defaultObjectReleaseIntensity;
        }

        // user is charging up the dice throw
        if (Input.GetKey(KeyCode.Space))
        {
            objectReleaseIntensity += intensityReleaseChangePerFrame;
        }

        // clear playing area of rollable objects
        if (Input.GetKeyUp(KeyCode.C))
        {
            deleteRollableObjects();
        }

    }

    // deletes all present rollable objects
    private void deleteRollableObjects()
    {
        var rollableObjects = FindObjectsOfType<Rollable>();
        foreach (var rollableObject in rollableObjects)
        {
            Destroy(rollableObject.gameObject);
        }
    }

    // returns rolled result of all rollable objects
    private int getResult()
    {
        int result = 0;
        var rollableObjects = FindObjectsOfType<Rollable>();
        foreach (var rollableObject in rollableObjects)
        {
            result += rollableObject.GetValue();
        }

        return result;
    }

    // constructs a RigidBody based on the user's current selection
    private Rigidbody instantiateSelectedObject()
    {
        // creates selected object with default orientation at given x,y,z
        switch (objectSelection)
        {
            case togglableObject.Die:
                return Instantiate(dieObject,
                    new Vector3(objectCreationPositionX, objectCreationPositionY, objectCreationPositionZ),
                    Quaternion.identity);
            default:
                return Instantiate(dieObject,
                    new Vector3(objectCreationPositionX, objectCreationPositionY, objectCreationPositionZ),
                    Quaternion.identity);
        }
    }
}
