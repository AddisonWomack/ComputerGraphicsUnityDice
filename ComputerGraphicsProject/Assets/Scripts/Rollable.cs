using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This class defines special behaviors that should occur as a rollable object rolls
/// </summary>
public abstract class Rollable : MonoBehaviour
{
    
    /// <summary>
    /// The type of comet trail being rendered for this object
    /// </summary>
    protected enum TrailType{
        edgyShadow,
        archimedeanSprial,
        rainbow,
        sithLord,
        iceMagic,
        basic
    }

    AudioSource audioSource;

    // private variables

    // array of sounds that the rollable object can make upon collision
    private AudioClip[] diceSounds = new AudioClip[5];

    // line renderer for this particular object - randomly initialized
    private LineRenderer currentTrailRenderer;

    // are rollable objects generating their trails?
    private static bool cometTrailEnabled = true;

    // initial velocity of the object
    private float initialVelocity = 20.0f;
    private Queue<Vector3> previousPositions;

    private bool isInitialized;

    private TrailType trailType = TrailType.rainbow;

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

    /// <summary>
    /// Triggers a sound effect based on the intensity of a collision between this object and another
    /// </summary>
    /// <param name="collision"> Instance of two objects colliding</param>
    void OnCollisionEnter(Collision collision)
    {
        if (rigidBody != null && rigidBody.name.CompareTo("bottle(Clone)") != 0)
        {
            float audioLevel = collision.relativeVelocity.magnitude * Random.value / 15.0f;
            audioSource.PlayOneShot(diceSounds[Random.Range(0, 5)], audioLevel);
        }
    }

    /// <summary>
    /// Called once every frame
    /// </summary>
    private void Update()
    {
        if (cometTrailEnabled)
        {
            if (rigidBody != null && currentTrailRenderer != null)
            {
                if (previousPositions.Count >= QUEUE_SIZE)
                {
                    previousPositions.Dequeue();
                }

                if (rigidBody.position != mostRecentPosition)
                {
                    previousPositions.Enqueue(rigidBody.position);
                    mostRecentPosition = rigidBody.position;
                    currentTrailRenderer.SetPositions(applyTransformation(previousPositions));
                }

            }

            if (rigidBody != null && !isInitialized)
            {
                initialize();
            }

            if (hasReturnedValue && previousPositions.Count > 0)
            {

                previousPositions.Dequeue();
                currentTrailRenderer.SetPositions(applyTransformation(previousPositions));

                if (previousPositions.Count == 0)
                {
                    currentTrailRenderer.positionCount = 0;
                }

            }
        }
    }

    /// <summary>
    /// Randomly returns a trail type
    /// </summary>
    /// <returns>The randomly selected type of trail that this object will draw behind it</returns>
    private TrailType getRandomTrailType()
    {
        var r = new System.Random();

        int trailSelection = r.Next() % Enum.GetNames(typeof(TrailType)).Length;

        switch (trailSelection)
        {
            case 5:
                return TrailType.edgyShadow;
            case 4:
                return TrailType.basic;
            case 3:
                return TrailType.iceMagic;
            case 2:
                return TrailType.sithLord;
            case 1:
                return TrailType.archimedeanSprial;
            case 0:
            default:
                return TrailType.rainbow;
        }
    }

    /// <summary>
    /// Takes set of points that the object has passed through and
    /// applies a transformation to them to achieve a desired trail
    /// effect
    /// </summary>
    /// <param name="positions">A queue containing the previous positions of the 
    /// rollable object</param>
    /// <returns>An array of 3d points to be assigned and drawn by a line renderer</returns>
    private Vector3[] applyTransformation(Queue<Vector3> positions)
    {
        var values = positions.ToArray();

        switch(trailType)
        {
            case TrailType.edgyShadow:
                return applyTrailToShadow(values);
            case TrailType.iceMagic:
                return applyIceMagic(values);
            case TrailType.sithLord:
                return applySithLordPointTransformation(values);
            case TrailType.archimedeanSprial:
                return applyArcimedeanSpiral(values);
            case TrailType.basic:
            case TrailType.rainbow:
            default:
                return values;
        }

    }
    
    /// <summary>
    /// Modifies the given set of previous positions such that a
    /// chaotic lightning effect branches out toward the object
    /// </summary>
    /// <param name="positions">The set of previous positions of the object</param>
    /// <returns>Set of previous points displaced to appear as an increasingly large wall of electricity</returns>
    private Vector3[] applySithLordPointTransformation
        (Vector3[] positions)
    {
        var result = new List<Vector3>();

        Random randomizer = new Random();

        int count = 0;

        foreach (var position in positions)
        {
            var randomComponentForThisFrame = Random.Range(-0.07f, 0.07f);

            // randomly displace points to create chaotic effect the further advanced this queue is
            // the more discplaced points can be
            var x = position.x + (count * Random.Range(-0.05f, 0.05f));
            var y = position.y + (count * Random.Range(-0.05f, 0.05f));
            var z = position.z + (count * Random.Range(-0.05f, 0.05f));

            if (count < 50)
                count++;

            var generatedPoint = new Vector3(x, y, z);

            result.Add(generatedPoint);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates an icy pattern of points by interpolating points between all
    /// real points, then randomly displaces it by a small amount to create a
    /// focused stream of pseudo-random points
    /// </summary>
    /// <param name="positions">Set of previous points that the object has been at</param>
    /// <returns>Set of points that creates an icy effect</returns>
    private Vector3[] applyIceMagic(Vector3[] positions)
    {
        var result = new List<Vector3>();

        Random randomizer = new Random();

        if (positions.Length > 1)
        {
            for (int i = 1; i < positions.Length; i++)
            {
                var previousPosition = positions[i - 1];
                var currentPosition = positions[i];

                // interpolate a point x,y,z inbetween two real points
                var interpolatedX = (currentPosition.x + previousPosition.x) / 2;
                var interpolatedY = (currentPosition.y + previousPosition.y) / 2;
                var interpolatedZ = (currentPosition.z + previousPosition.z) / 2;

                // apply random changes to interpolated point to give chaotic effect with intensity based on initial velocity
                interpolatedX += Random.Range(-1 * initialVelocity, initialVelocity);
                interpolatedY += Random.Range(-1 * initialVelocity, initialVelocity);
                interpolatedZ += Random.Range(-1 * initialVelocity, initialVelocity);

                var interpolatedPoint = new Vector3(interpolatedX, interpolatedY, interpolatedZ);

                result.Add(previousPosition);
                result.Add(interpolatedPoint);

            }
        }

        if (positions.Length > 0)
        {
            result.Add(positions[positions.Length - 1]);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Simple effect that creates a shadowy trail effect that hovers on the ground of the box in the wake of
    /// the object
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    private Vector3[] applyTrailToShadow(Vector3[] positions)
    {
        var result = new List<Vector3>();

        foreach (var position in positions)
        {
            var transformedPoint = new Vector3(position.x, 0.1f, position.z);
            result.Add(transformedPoint);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Creates a spiral effect for the object's trail by applying the spiral of archimedes to it
    /// </summary>
    /// <param name="positions">The original set of points that the object actually traveled through</param>
    /// <returns>Points with a spiral transformation applied to them</returns>
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

    /// <summary>
    /// Creates a new line renderer object that draws a rainbow from the given array of points
    /// </summary>
    /// <returns></returns>
    private LineRenderer generateRainBowLineRenderer()
    {
        // make trail be a fabulous gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f), new GradientAlphaKey(0.8f, 0.5f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        var rainbowTrailRenderer = gameObject.AddComponent<LineRenderer>();

        rainbowTrailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        rainbowTrailRenderer.startWidth = START_WIDTH;
        rainbowTrailRenderer.positionCount = QUEUE_SIZE;
        rainbowTrailRenderer.endWidth = END_WIDTH;
        rainbowTrailRenderer.generateLightingData = false;
        rainbowTrailRenderer.loop = false;
        rainbowTrailRenderer.colorGradient = gradient;

        return rainbowTrailRenderer;
    }

    /// <summary>
    /// Creates a new line renderer object that simulates light blue to
    /// white effect for lightning and icy effects
    /// </summary>
    /// <returns></returns>
    private LineRenderer generateSithLordLineRenderer()
    {

        var lightBlue = new Color(0.7f, 0.7f, 0.90f);
        var lighterBlue = new Color(0.8f, 0.8f, 0.98f);

        // make trail be a malevelonet light blue gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(lightBlue, 0.0f), new GradientColorKey(lighterBlue, 0.5f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f), new GradientAlphaKey(0.8f, 0.5f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        var sithLordRenderer = gameObject.AddComponent<LineRenderer>();

        sithLordRenderer.material = new Material(Shader.Find("Sprites/Default"));
        sithLordRenderer.startWidth = START_WIDTH;
        sithLordRenderer.positionCount = QUEUE_SIZE;
        sithLordRenderer.endWidth = END_WIDTH;
        sithLordRenderer.generateLightingData = false;
        sithLordRenderer.loop = false;
        sithLordRenderer.colorGradient = gradient;

        return sithLordRenderer;
    }

    /// <summary>
    /// Creates a line renderer object that draws a transparent dark purple fading
    /// to black effect
    /// </summary>
    /// <returns></returns>
    private LineRenderer generateShadowLineRenderer()
    {

        Color spookyPurple = new Color(75.0f / 255, 0, 130.0f / 255);

        // make trail be solid red
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(spookyPurple, 0.0f), new GradientColorKey(Color.black, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );

        var basicLineRenderer = gameObject.AddComponent<LineRenderer>();

        basicLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        basicLineRenderer.startWidth = START_WIDTH * 3;
        basicLineRenderer.positionCount = QUEUE_SIZE;
        basicLineRenderer.endWidth = END_WIDTH;
        basicLineRenderer.generateLightingData = false;
        basicLineRenderer.loop = false;
        basicLineRenderer.colorGradient = gradient;

        return basicLineRenderer;
    }

    /// <summary>
    /// Creates a simple line renderer that draws a solid red line
    /// </summary>
    /// <returns></returns>
    private LineRenderer generateBasicLineRenderer()
    {

        // make trail be solid red
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f) }
        );

        var basicLineRenderer = gameObject.AddComponent<LineRenderer>();

        basicLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        basicLineRenderer.startWidth = START_WIDTH;
        basicLineRenderer.positionCount = QUEUE_SIZE;
        basicLineRenderer.endWidth = END_WIDTH;
        basicLineRenderer.generateLightingData = false;
        basicLineRenderer.loop = false;
        basicLineRenderer.colorGradient = gradient;

        return basicLineRenderer;
    }

    // initializes fields for this object
    public void initialize()
    {

        trailType = getRandomTrailType();


        switch (trailType)
        {
            case TrailType.edgyShadow:
                currentTrailRenderer = generateShadowLineRenderer();
                break;
            case TrailType.basic:
                currentTrailRenderer = generateBasicLineRenderer();
                break;
            case TrailType.iceMagic:
            case TrailType.sithLord:
                currentTrailRenderer = generateSithLordLineRenderer();
                break;
            case TrailType.archimedeanSprial:
            case TrailType.rainbow:
            default:
                currentTrailRenderer = generateRainBowLineRenderer();
                break;
        }



        rigidBody = GetComponent<Rigidbody>();
        previousPositions = new Queue<Vector3>(QUEUE_SIZE);


        isInitialized = true;
    }

    public void setInitialVelocity(float initialVelocity)
    {
        this.initialVelocity = initialVelocity;
    }

    public void enableCometTrail()
    {
        cometTrailEnabled = true;
        if (currentTrailRenderer != null)
            currentTrailRenderer.positionCount = QUEUE_SIZE;
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
        if (currentTrailRenderer != null)
            currentTrailRenderer.positionCount = 0;
    }



    // abstract methods
    public abstract bool IsDoneMoving();

    public abstract bool HasReturnedValue();

    public abstract void SetHasReturned(bool returned);

    public abstract void UpdateMovement();

    // returns the result of the rolled object
    public abstract int GetValue();
}
