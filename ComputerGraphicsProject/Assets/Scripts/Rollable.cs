using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Rollable : MonoBehaviour
{
    
    protected enum TrailType{
        archimedeanSprial,
        basic
    }

    AudioSource audioSource;
    private AudioClip[] diceSounds = new AudioClip[5];

    // private variables

    private LineRenderer cometTrailRenderer;

    private bool cometTrailEnabled = true;

    private float initialVelocity = 20.0f;
    private Queue<Vector3> previousPositions;

    private bool isInitialized;

    private TrailType trailType = TrailType.basic;

    private Vector3 mostRecentPosition;

    private const float START_WIDTH = 1.0f;
    private const float END_WIDTH = 0.0f;
    private const int QUEUE_SIZE = 70;

    // protected variables
    protected Rigidbody rigidBody;

    protected bool isMoving = false;
    protected bool hasStartedMoving = false;
    protected bool isFinishedMoving = false;
    protected bool hasReturnedValue = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        initialVelocity = 1;
        isInitialized = false;
        mostRecentPosition = new Vector3(0.0f, 0.0f, 0.0f);

        audioSource = gameObject.AddComponent<AudioSource>();

        diceSounds[0] = Resources.Load("dice1") as AudioClip;
        diceSounds[1] = Resources.Load("dice2") as AudioClip;
        diceSounds[2] = Resources.Load("dice3") as AudioClip;
        diceSounds[3] = Resources.Load("dice4") as AudioClip;
        diceSounds[4] = Resources.Load("dice5") as AudioClip;
    }

    // Remove die when it leaves field of view
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //Used for sounds
    void OnCollisionEnter(Collision collision)
    {
        if (rigidBody.name.CompareTo("bottle(Clone)") != 0)
        {
            float audioLevel = collision.relativeVelocity.magnitude * Random.value / 15.0f;
            audioSource.PlayOneShot(diceSounds[Random.Range(0, 5)], audioLevel);
        }
    }

    private void Update()
    {
        if (cometTrailEnabled)
        {
            if (rigidBody != null && cometTrailRenderer != null)
            {
                if (previousPositions.Count >= QUEUE_SIZE)
                {
                    previousPositions.Dequeue();
                }

                if (rigidBody.position != mostRecentPosition)
                {
                    previousPositions.Enqueue(rigidBody.position);
                    mostRecentPosition = rigidBody.position;
                    cometTrailRenderer.SetPositions(applyTransformation(previousPositions));
                }

            }

            if (rigidBody != null && !isInitialized)
            {
                initialize();
            }

            if (hasReturnedValue && previousPositions.Count > 0)
            {

                previousPositions.Dequeue();
                cometTrailRenderer.SetPositions(applyTransformation(previousPositions));

                if (previousPositions.Count == 0)
                {
                    cometTrailRenderer.positionCount = 0;
                }

            }
        }
    }

    private TrailType getRandomTrailType()
    {
        var r = new System.Random();

        int trailSelection = r.Next() % Enum.GetNames(typeof(TrailType)).Length;

        switch (trailSelection)
        {
            case 1:
                return TrailType.archimedeanSprial;
            case 0:
            default:
                return TrailType.basic;
        }
    }

    private Vector3[] applyTransformation(Queue<Vector3> positions)
    {
        var values = positions.ToArray();

        switch(trailType)
        {
            case TrailType.archimedeanSprial:
                return applyArcimedeanSpiral(values);
            case TrailType.basic:
            default:
                return values;
        }

    }

    private Vector3[] applyArcimedeanSpiral(Vector3[] positions)
    {
        var result = new List<Vector3>();

        float theRadiusFactor = 1.0f;

        float decreasePerValue = (positions.Length > 0) ? theRadiusFactor / positions.Length : 0.99f;

        foreach (var value in positions)
        {
            float theta = value.x * 2;
            float zComponent = (theta * initialVelocity * Mathf.Cos(theta) * theRadiusFactor) + value.z;
            float yComponent = (theta * initialVelocity * Mathf.Sin(theta) * theRadiusFactor) + value.y;

            result.Add(new Vector3(value.x, yComponent, zComponent));

            theRadiusFactor -= decreasePerValue;
        }

        return result.ToArray();
    }

    public void initialize()
    {

        trailType = getRandomTrailType();


        // make trail be a fabulous gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f), new GradientAlphaKey(0.8f, 0.5f), new GradientAlphaKey(1.0f, 1.0f) }
        );



        rigidBody = GetComponent<Rigidbody>();
        previousPositions = new Queue<Vector3>(QUEUE_SIZE);
        cometTrailRenderer = gameObject.AddComponent<LineRenderer>();
        
        cometTrailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        cometTrailRenderer.startWidth = START_WIDTH;
        cometTrailRenderer.positionCount = QUEUE_SIZE;
        cometTrailRenderer.endWidth = END_WIDTH;
        cometTrailRenderer.generateLightingData = false;
        cometTrailRenderer.loop = false;
        cometTrailRenderer.colorGradient = gradient;

        isInitialized = true;
    }

    public void setInitialVelocity(float initialVelocity)
    {
        this.initialVelocity = initialVelocity;
    }

    public void enableCometTrail()
    {
        cometTrailEnabled = true;
        if (cometTrailRenderer != null)
            cometTrailRenderer.positionCount = QUEUE_SIZE;
    }

    public Vector3 getPosition()
    {
        return rigidBody.position;
    }

    public Rigidbody getRigidBody()
    {
        return rigidBody;
    }

    public void disableCometTrail()
    {
        cometTrailEnabled = false;
        if (cometTrailRenderer != null)
            cometTrailRenderer.positionCount = 0;
    }

    public abstract bool IsDoneMoving();

    public abstract bool HasReturnedValue();

    public abstract void SetHasReturned(bool returned);

    public abstract void UpdateMovement();

    // returns the result of the rolled object
    public abstract int GetValue();
}
