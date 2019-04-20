using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettableFriction : MonoBehaviour
{

    public Texture2D normalTexture;
    public Texture2D icyTexture;
    public Texture2D sandPaperTexture;

    public Material normalMaterial;
    public Material iceMaterial;
    public Material sandMaterial;

    public PhysicMaterial currentPhysicsMaterial;
    public surfaceType currentSurfaceType;

    public const float NORMAL_STATIC_FRICTION = 0.6f;
    public const float NORMAL_DYNAMIC_FRICTION = 0.6f;
    public const float ICY_STATIC_FRICTION = 0.1f;
    public const float ICY_DYNAMIC_FRICTION = 0.1f;
    public const float SANDPAPER_STATIC_FRICTION = 2f;
    public const float SANDPAPER_DYNAMIC_FRICTION = 2f;

    public enum surfaceType
    {
        NORMAL,
        ICE,
        SANDPAPER
    }

    public void applyTextureChange(surfaceType type)
    {
        if (type != currentSurfaceType)
        {
            switch (type)
            {
                case surfaceType.NORMAL:
                    currentPhysicsMaterial.staticFriction = NORMAL_STATIC_FRICTION;
                    currentPhysicsMaterial.dynamicFriction = NORMAL_DYNAMIC_FRICTION;
                    GetComponent<Renderer>().material = normalMaterial;
                    GetComponent<Renderer>().material.mainTexture = normalTexture;
                    break;
                case surfaceType.ICE:
                    currentPhysicsMaterial.staticFriction = ICY_STATIC_FRICTION;
                    currentPhysicsMaterial.dynamicFriction = ICY_DYNAMIC_FRICTION;
                    GetComponent<Renderer>().material = iceMaterial;
                    GetComponent<Renderer>().material.mainTexture = icyTexture;
                    break;
                case surfaceType.SANDPAPER:
                    currentPhysicsMaterial.staticFriction = SANDPAPER_STATIC_FRICTION;
                    currentPhysicsMaterial.dynamicFriction = SANDPAPER_DYNAMIC_FRICTION;
                    GetComponent<Renderer>().material = sandMaterial;
                    GetComponent<Renderer>().material.mainTexture = sandPaperTexture;

                    break;
            }

            currentSurfaceType = type;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        currentPhysicsMaterial.staticFriction = NORMAL_STATIC_FRICTION;
        currentPhysicsMaterial.dynamicFriction = NORMAL_DYNAMIC_FRICTION;
        currentPhysicsMaterial.bounciness = 0;
        currentSurfaceType = surfaceType.NORMAL;
        GetComponent<Renderer>().material.mainTexture = normalTexture;
    }
}
