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

    // Custom Cursor handling
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // most recent tally of thrown objects
    private int currentDieResult;

    // force applied to object causing it to move forward
    private float objectReleaseIntensity;

    private float angularVelocityPitchRate;
    private float angularVelocityRollRate;
    private float angularVelocityYawRate;

    // type of object being created
    private togglableObject objectSelection;

    // object to be thrown
    private Rigidbody CurrentObjectToBeThrown;

    private bool isThrown = false;

    private const float defaultObjectReleaseIntensity = 30;
    private const float intensityReleaseChangePerFrame = 5;
    private const float maxObectReleaseIntensity = 2000;
    private const float objectRespawnTime = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        // set to die by default
        objectSelection = togglableObject.Die;
        objectReleaseIntensity = defaultObjectReleaseIntensity;
        CurrentObjectToBeThrown = instantiateSelectedObject();

        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        Time.timeScale = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // user is throwing an object
        if (Input.GetKeyUp(KeyCode.Space))
        {
            throwObject();
        }

        // user is charging up the dice throw
        if (Input.GetKey(KeyCode.Space))
        {
            if(objectReleaseIntensity < maxObectReleaseIntensity)
                objectReleaseIntensity += intensityReleaseChangePerFrame;

            rotateCurrentObjectZ(objectReleaseIntensity);
            rotateCurrentObjectX(objectReleaseIntensity);
        }

        // clear playing area of rollable objects
        if (Input.GetKeyUp(KeyCode.C))
        {
            deleteRollableObjects();
            Invoke("respawnObject", objectRespawnTime);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotateCurrentObjectX(-1 * objectReleaseIntensity);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            rotateCurrentObjectX(objectReleaseIntensity);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotateCurrentObjectY(objectReleaseIntensity);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotateCurrentObjectY(-1 * objectReleaseIntensity);
        }

    }

    private void rotateCurrentObjectX(float delta)
    {
        if (!isThrown)
        {
            var deltaQuarternion = Quaternion.Euler(delta * Time.deltaTime, 0, 0);
            CurrentObjectToBeThrown.MoveRotation(CurrentObjectToBeThrown.rotation * deltaQuarternion);
        }
    }

    private void rotateCurrentObjectY(float delta)
    {
        if (!isThrown)
        {
            var deltaQuarternion = Quaternion.Euler(0, delta * Time.deltaTime, 0);
            CurrentObjectToBeThrown.MoveRotation(CurrentObjectToBeThrown.rotation * deltaQuarternion);
        }
    }

    private void rotateCurrentObjectZ(float delta)
    {
        if (!isThrown)
        {
            var deltaQuarternion = Quaternion.Euler(0, 0, delta * Time.deltaTime);
            CurrentObjectToBeThrown.MoveRotation(CurrentObjectToBeThrown.rotation * deltaQuarternion);
        }
    }

    private void respawnObject()
    {
        CurrentObjectToBeThrown = instantiateSelectedObject();
        isThrown = false;
    }

    private void throwObject()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, objectReleaseIntensity);
        
        // enable gravity
        CurrentObjectToBeThrown.useGravity = true;

        // Get the location under the crosshair and set the throw direction towards it
        // Also sets the throw speed proportional to the intensity
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            direction = -0.1f*objectReleaseIntensity*(CurrentObjectToBeThrown.position - hit.point);
        }

        // launch the object forward with the given intensity
        CurrentObjectToBeThrown.AddForce(direction);

        // launch the object forward with the given intensity
        CurrentObjectToBeThrown.AddTorque(direction);

        // reset object release intensity to default
        objectReleaseIntensity = defaultObjectReleaseIntensity;
        isThrown = true;
        // calls method after specified amount of time
        Invoke("respawnObject", objectRespawnTime);
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
        Rigidbody rigidbody;

        var creationPosition = new Vector3(objectCreationPositionX, objectCreationPositionY, objectCreationPositionZ);

        // creates selected object with default orientation at given x,y,z
        switch (objectSelection)
        {
            case togglableObject.Die:
                rigidbody = Instantiate(dieObject,
                    creationPosition,
                    Quaternion.identity);
                break;
            default:
                rigidbody = Instantiate(dieObject,
                    creationPosition,
                    Quaternion.identity);
                break;
        }
        // initially not affected by gravity
        rigidbody.useGravity = false;
        return rigidbody;
    }
}
