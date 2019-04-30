using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script permits a particular object that uses it to have its material, texture, and friction 
/// settable to a set of pre-defined textures
/// </summary>
public class SettableFriction : MonoBehaviour
{

    // Textures corresponding to different types of surfaces
    public Texture2D normalTexture;
    public Texture2D icyTexture;
    public Texture2D sandPaperTexture;

    // Materials that determine the appearance of the different surfaces
    public Material normalMaterial;
    public Material iceMaterial;
    public Material sandMaterial;

    // physics material object, which controls friction
    public PhysicMaterial currentPhysicsMaterial;
    public surfaceType currentSurfaceType;

    // values corresponding to the different types of friction
    public const float NORMAL_STATIC_FRICTION = 0.6f;
    public const float NORMAL_DYNAMIC_FRICTION = 0.6f;
    public const float ICY_STATIC_FRICTION = 0.000f;
    public const float ICY_DYNAMIC_FRICTION = 0.000f;
    public const float SANDPAPER_STATIC_FRICTION = 2f;
    public const float SANDPAPER_DYNAMIC_FRICTION = 4f;

    public enum surfaceType
    {
        NORMAL,
        ICE,
        SANDPAPER
    }

    /// <summary>
    /// Modifies this object to have the specified surface type
    /// </summary>
    /// <param name="type"> The type of surface that this surface is being changed to</param>
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
            GetComponent<MeshCollider>().material = currentPhysicsMaterial;
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
