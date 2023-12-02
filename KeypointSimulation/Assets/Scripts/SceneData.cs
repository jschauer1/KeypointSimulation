using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Author: Jaxon Schauer
/// <summary>
/// Creates an object that contains variables for each scene/environment
/// </summary>
[System.Serializable]
public class SceneData
{
    [SerializeField] 
    private float lightIntensity = 0;
    [SerializeField]
    private int terrainLayer;
    [SerializeField]
    private bool randomizeRotation;
    [SerializeField]
    private Vector3 rotation;
    [SerializeField]
    private bool randomizeDistance;
    [SerializeField]
    private float distance;
    [SerializeField]
    private int totalPhotos;
    [SerializeField]
    private string intendedDirName;
    [SerializeField]
    private Material material;
    public Material GetMaterial()
    {
        return material;
    }
    public string GetIntendedDirName()
    {
        return intendedDirName;
    }
    public float GetLightIntensity()
    {
        return lightIntensity;
    }
    public int GetTerrainLayer()
    {
        return terrainLayer;
    }
    public bool GetRandomizeRotation()
    {
        return randomizeRotation;
    }
    public Vector3 GetRotation()
    {
        return rotation;
    }
    public bool GetRandomizeDistance()
    {
        return randomizeDistance;
    }
    public float GetDistance()
    {
        return distance;
    }
    public int GetTotalPhotos()
    {
        return totalPhotos;
    }
}
