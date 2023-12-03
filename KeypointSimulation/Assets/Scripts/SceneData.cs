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
    public int GetTotalPhotos()
    {
        return totalPhotos;
    }
}
