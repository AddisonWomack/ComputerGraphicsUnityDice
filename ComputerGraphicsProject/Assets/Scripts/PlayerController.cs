﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using UnityEngine;
using System.Threading;

/// <summary>
/// This class defines the ways that a player can interact with our environment
/// </summary>
public class PlayerController : MonoBehaviour
{
    private enum TogglableObject
    {
        D6,
        D20,
        Bottle
    }

    private Color[] colors = {Color.blue, Color.green, Color.red, Color.cyan, Color.magenta, Color.yellow};
    // die objects that the player can create
    public Rigidbody d6Prefab;
    public Rigidbody d20Prefab;
    public Rigidbody bottlePrefab;

    // The spinning selector objects
    public Rigidbody d6Selector;
    public Rigidbody d20Selector;
    public Rigidbody bottleSelector;

    // text that is rendered on the scorebard
    public UnityEngine.UI.Text scoreboardText;

    // starting point of created object
    public int objectCreationPositionX;
    public int objectCreationPositionY;
    public int objectCreationPositionZ;

    // Custom Cursor handling
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private GameObject lightGameObject;
    private GameObject resultText;
    private TextMesh textComp;
    private Light lightComp;

    // most recent tally of thrown objects
    private string currentDieResult;
    private bool showResult;

    // force applied to object causing it to move forward
    private float objectReleaseIntensity;

    // the angular velocity of the throwable object before release
    private float angularVelocityPitchRate;
    private float angularVelocityRollRate;
    private float angularVelocityYawRate;

    // type of object being created
    private TogglableObject objectSelection;

    // object to be thrown
    private Rigidbody CurrentObjectToBeThrown;

    // has the current object been thrown yet?
    private bool isThrown = false;


    private List<GameObject> resultTextList = new List<GameObject>();
    private bool moveResultTowardUser = true;
    private int resultCount = 0;

    // Set of constants associated with this class
    private const float defaultObjectReleaseIntensity = 30;
    private const float intensityReleaseChangePerFrame = 5;
    private const float maxObectReleaseIntensity = 2000;
    private const float objectRespawnTime = 0.8f;


    private const float selectorInactive = 100f;
    private const float selectorActive = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // set to die by default
        objectSelection = TogglableObject.D6;
        CurrentObjectToBeThrown = instantiateSelectedObject();

        d6Selector.angularDrag = selectorActive;

        objectReleaseIntensity = defaultObjectReleaseIntensity;

        // Add the light component

        lightGameObject = new GameObject("The Light");
        lightComp = lightGameObject.AddComponent<Light>();

        // Set color and position
        lightComp.color = new Color(255, 244, 214, 255);
        lightComp.intensity = 0.005f;
        lightComp.type = LightType.Directional;

        // Set the position (or any transform property)
        lightGameObject.transform.position = new Vector3(14.01f, 13.34f, 2.92f);
        lightGameObject.transform.eulerAngles = new Vector3(
            lightGameObject.transform.eulerAngles.x + 50,
            lightGameObject.transform.eulerAngles.y -30,
            lightGameObject.transform.eulerAngles.z
        );


        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        Time.timeScale = 2;
        Physics.gravity = new Vector3(0.0f, -6.5f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // user is throwing an object
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (CurrentObjectToBeThrown != null)
                throwObject();
        }

        // user is charging up the dice throw
        if (Input.GetKey(KeyCode.Space))
        {
            if (CurrentObjectToBeThrown != null)
            {
                if (objectReleaseIntensity < maxObectReleaseIntensity)
                    objectReleaseIntensity += intensityReleaseChangePerFrame;
                
                rotateCurrentObjectX(objectReleaseIntensity);

                if(objectSelection != TogglableObject.Bottle)
                    rotateCurrentObjectZ(objectReleaseIntensity);
            }
        }

        // increase gravity
        if (Input.GetKey(KeyCode.Z))
        {
            if (Physics.gravity.y < -3.05)
            {
                Physics.gravity = new Vector3(0.0f, Physics.gravity.y + 0.02f, 0.0f);
            }
        }

        // decrease gravity
        if (Input.GetKey(KeyCode.X))
        {
            if(Physics.gravity.y > -15) { 
                Physics.gravity = new Vector3(0.0f, Physics.gravity.y - 0.02f, 0.0f);
            }
        }

        // decrease light intensity
        if (Input.GetKey(KeyCode.A))
        {
            if (lightComp.intensity > 0.001)
            {
                lightComp.intensity = lightComp.intensity - 0.0001f;
            }
        }

        // increase light intensity
        if (Input.GetKey(KeyCode.S))
        {
            if (lightComp.intensity < 0.015)
            {
                lightComp.intensity = lightComp.intensity + 0.0001f;
            }
        }

        // toggle die result floating towards user
        if (Input.GetKey(KeyCode.R))
        {
            moveResultTowardUser = !moveResultTowardUser;
        }

        // change to normal dice box surface
        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            setNormalSurface();
        }

        // change to icy dice box surface
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            setIcySurface();
        }

        // change to sand paper surface
        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            setSandpaperSurface();
        }

        // switch to d6 object
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            objectSelection = TogglableObject.D6;
            Destroy(CurrentObjectToBeThrown.gameObject);
            CurrentObjectToBeThrown = instantiateSelectedObject();

            d6Selector.angularDrag = selectorActive;
            d20Selector.angularDrag = selectorInactive;
            bottleSelector.angularDrag = selectorInactive;
        }

        // switch to d20 object
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            objectSelection = TogglableObject.D20;
            Destroy(CurrentObjectToBeThrown.gameObject);
            CurrentObjectToBeThrown = instantiateSelectedObject();
            
            d6Selector.angularDrag = selectorInactive;
            d20Selector.angularDrag = selectorActive;
            bottleSelector.angularDrag = selectorInactive;
        }

        // switch to bottle object
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            objectSelection = TogglableObject.Bottle;
            Destroy(CurrentObjectToBeThrown.gameObject);
            CurrentObjectToBeThrown = instantiateSelectedObject();

            d6Selector.angularDrag = selectorInactive;
            d20Selector.angularDrag = selectorInactive;
            bottleSelector.angularDrag = selectorActive;
        }

        // turn comet trail on
        if (Input.GetKeyUp(KeyCode.O))
        {
            enableCometTrail();
        } else if (Input.GetKeyUp(KeyCode.P))
        {
            // disable comet trail
            disableCometTrail();
        }

        // clear playing area of rollable objects
        if (Input.GetKeyUp(KeyCode.C))
        {
            deleteRollableObjects();
        }

        // rotate object forward (away from user)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotateCurrentObjectX(-1 * objectReleaseIntensity);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            // rotate backward (towards user)
            rotateCurrentObjectX(objectReleaseIntensity);
        }

        // rotate object to the left (negative yaw)
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotateCurrentObjectY(objectReleaseIntensity);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            // rotate object to the right (positive yaw)
            rotateCurrentObjectY(-1 * objectReleaseIntensity);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        this.updateDice();
        updateResults();
        int result = this.getResult();
    }

    private void OnGUI()
    {
        if (showResult)
        {
            // Make a multiline text area that modifies stringToEdit.
            //GUI.Label(new Rect(10, 10, 200, 100), currentDieResult);
            System.Timers.Timer aTimer = new System.Timers.Timer(2500);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
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
        if (isThrown)
        {
            CurrentObjectToBeThrown = instantiateSelectedObject();
            isThrown = false;
        }
    }

    private void throwObject()
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, objectReleaseIntensity);
        
        // enable gravity
        CurrentObjectToBeThrown.useGravity = true;
        CurrentObjectToBeThrown.detectCollisions = true;

        // Get the location under the crosshair and set the throw direction towards it
        // Also sets the throw speed proportional to the intensity
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            direction = -0.1f*objectReleaseIntensity*(CurrentObjectToBeThrown.position - hit.point);
        }

        if (objectSelection == TogglableObject.Bottle)
            direction[1] = 2.0f;

        // launch the object forward with the given intensity
        CurrentObjectToBeThrown.AddForce(direction);

        // launch the object with spin, unless it's a bottle
        if (objectSelection != TogglableObject.Bottle)
            CurrentObjectToBeThrown.AddTorque(direction);

        // reset object release intensity to default
        objectReleaseIntensity = defaultObjectReleaseIntensity;
        isThrown = true;
        // calls method after specified amount of time
        Invoke("respawnObject", objectRespawnTime);
    }

    // deletes all present rollable objects except the held one
    // Also removes any result text objects
    private void deleteRollableObjects()
    {
        var rollableObjects = FindObjectsOfType<Rollable>();
        foreach (var rollableObject in rollableObjects)
        {
            if(!(rollableObject.getRigidBody() == CurrentObjectToBeThrown))
                Destroy(rollableObject.gameObject);
        }

        var texts = GameObject.FindGameObjectsWithTag("ResultText");
        foreach (var text in texts)
        {
            Destroy(text);
        }
        resultCount = 0;
    }

    // returns rolled result of all rollable objects
    private int getResult()
    {
        int result = 0;
        var rollableObjects = FindObjectsOfType<Rollable>();
        foreach (var rollableObject in rollableObjects)
        {
            if (rollableObject.IsDoneMoving() && !rollableObject.HasReturnedValue())
            {
                int thisResult = rollableObject.GetValue();
                result += thisResult;
                if (thisResult == 0)
                {
                    Vector3 vec = rollableObject.getPosition();
                    spawnTextResult("?", vec);
                    showResult = true;
                }
                else
                {
                    Vector3 vec = rollableObject.getPosition();
                    vec.y += 4;
                    spawnTextResult(thisResult.ToString(), vec);
                    scoreboardText.text += " " + thisResult;
                    resultCount++;
                    showResult = true;
                }

            }
        }

        return result;
    }

    private void spawnTextResult(String result, Vector3 position)
    {
        resultText = new GameObject("Result Text");
        resultText.tag = "ResultText";
        textComp = resultText.AddComponent<TextMesh>();
        resultText.transform.position = new Vector3(position.x, position.y + 2, position.z);
        resultText.transform.eulerAngles = new Vector3(
            resultText.transform.eulerAngles.x + 30,
            resultText.transform.eulerAngles.y,
            resultText.transform.eulerAngles.z
        );

        textComp.text = result;
        textComp.characterSize = 0.1f;
        textComp.fontSize = 100;

        textComp.color = colors[result != "?" ? Int32.Parse(result)%6 : 0];

        resultTextList.Add(resultText);
    }

    /// <summary>
    /// Cause result of thrown object to float upwards
    /// </summary>
    private void updateResults()
    {
        int i = 0;
        if (resultTextList.Count == 10)
        {
            if (resultTextList.Count > 0)
            {
                if (resultTextList[0] != null)
                    resultTextList[0].transform.position = new Vector3(100, 100, 100);
                resultTextList.RemoveAt(0);
            }

        }

        foreach (GameObject obj in resultTextList)
        {
            if (obj != null)
            {
                float step;
                if (moveResultTowardUser)
                {
                    step = 1.8f * Time.deltaTime;
                    obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-15, 19, -11), step);
                    obj.transform.localScale = Vector3.MoveTowards(obj.transform.localScale, new Vector3(obj.transform.localScale.x * 1.2f, obj.transform.localScale.y * 1.2f, obj.transform.localScale.z * 1.2f), step);
                    if (obj.transform.localScale.x > 8)
                    {
                        TextMesh mesh = obj.GetComponent<TextMesh>();
                        mesh.text = "";
                    }
                }
                else
                {
                    step = 3f * Time.deltaTime;
                    if (resultCount < 25)
                    {
                        obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-4 + ((resultCount - 1) * 0.4f), 15.9f, 2), step);
                    }
                    else
                    {
                        int temp = resultCount - 24;
                        if (temp > 32)
                        {
                            temp -= 32;
                            if (temp % 32 < 6)
                                obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-7.9f + ((temp - 1) * 0.45f), 14.5f, 2), step);
                            else
                                obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-7.9f + ((temp - 1) * 0.38f), 14.5f, 2), step);
                        }
                        else
                        {
                            if (temp % 32 < 6)
                                obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-7.9f + ((temp - 1) * 0.45f), 15.4f, 2), step);
                            else
                                obj.transform.position = Vector3.MoveTowards(obj.transform.position, new Vector3(-7.9f + ((temp - 1) * 0.38f), 15.4f, 2), step);
                        }

                    }
                    if (obj.transform.position.z < 2.000001f && obj.transform.position.z > 1.999999f)
                    {
                        TextMesh mesh = obj.GetComponent<TextMesh>();
                        mesh.text = "";
                    }
                }
            }            
            
        }
    }

    private void OnTimedEvent(System.Object source, ElapsedEventArgs e)
    {
        showResult = false;
    }

    // updates all internal states of dice
    private void updateDice()
    {
        var rollableObjects = FindObjectsOfType<Rollable>();
        foreach (var rollableObject in rollableObjects)
        {
            try
            {
                rollableObject.UpdateMovement();
            }
            catch (NullReferenceException e)
            {
            }
        }
    }

    private void enableCometTrail()
    {
        var rollableObjects = FindObjectsOfType<Rollable>();

        foreach(var rollableObject in rollableObjects)
        {
            rollableObject.enableCometTrail();
        }
    }

    private void disableCometTrail()
    {
        var rollableObjects = FindObjectsOfType<Rollable>();

        foreach (var rollableObject in rollableObjects)
        {
            rollableObject.disableCometTrail();
        }
    }

    // constructs a RigidBody based on the user's current selection
    private Rigidbody instantiateSelectedObject()
    {
        Rigidbody rigidbody;

        var creationPosition = new Vector3(objectCreationPositionX, objectCreationPositionY, objectCreationPositionZ);

        // creates selected object with default orientation at given x,y,z
        switch (objectSelection)
        {
            case TogglableObject.D6:
                rigidbody = Instantiate(d6Prefab,
                    creationPosition,
                    Quaternion.identity);
                break;
            case TogglableObject.D20:
                rigidbody = Instantiate(d20Prefab,
                    creationPosition,
                    Quaternion.identity);
                break;
            case TogglableObject.Bottle:
                rigidbody = Instantiate(bottlePrefab,
                    creationPosition,
                    Quaternion.identity);
                break;
            default:
                rigidbody = Instantiate(d6Prefab,
                    creationPosition,
                    Quaternion.identity);
                break;
        }
        // initially not affected by gravity
        rigidbody.useGravity = false;
        rigidbody.detectCollisions = false;
        return rigidbody;
    }

    private void setNormalSurface()
    {
        updateSurfaces(SettableFriction.surfaceType.NORMAL);
    }

    private void setIcySurface()
    {
        updateSurfaces(SettableFriction.surfaceType.ICE);
    }

    private void setSandpaperSurface()
    {
        updateSurfaces(SettableFriction.surfaceType.SANDPAPER);
    }

    private void updateSurfaces(SettableFriction.surfaceType surfaceType)
    {
        var objectsWithSettableFrictions = FindObjectsOfType<SettableFriction>();
        foreach (var obj in objectsWithSettableFrictions)
        {
            obj.applyTextureChange(surfaceType);
        }
    }
}
