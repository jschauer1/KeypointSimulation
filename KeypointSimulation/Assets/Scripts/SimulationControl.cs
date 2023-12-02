using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.Experimental.GlobalIllumination;
// Author: Jaxon Schauer
/// <summary>
/// Orchestrates the processes needed to simulate a fuelcap in 3d space. 
/// These actions include: 
/// moving the camera,
/// rotating the fuelcap,
/// and collecting essential information to be saved to json.
/// </summary>
public class SimulationControl : MonoBehaviour
{
    [Header("Scene/Environment Configuration")]
    [SerializeField, Tooltip("List of data for each scene/environment.")]
    private List<SceneData> sceneDataList;

    [SerializeField, Tooltip("Component for handling data saving.")]
    private SaveData captureImages;

    [SerializeField, Tooltip("Main camera used in the scene.")]
    private Camera cam;

    [Header("Keypoint Tracking")]
    [SerializeField, Tooltip("List of keypoint gameobjects on fuelcap")]
    private List<Transform> keypoints;

    [Header("Offset Configuration")]
    [SerializeField, Tooltip("Horizontal offset movement between frames.")]
    private float offsetx;

    [SerializeField, Tooltip("Vertical offset movement between frames.")]
    private float offsety;

    [SerializeField, Tooltip("Range for distance offset.")]
    private Vector2 offsetz;

    [Header("Scene Objects")]
    [SerializeField, Tooltip("Terrain object in the scene.")]
    private GameObject terrainObj;

    [SerializeField, Tooltip("Directional light used in the scene.")]
    private Light directionalLight;

    [SerializeField, Tooltip("Fuel cap object.")]
    private GameObject fuelCap;

    [SerializeField, Tooltip("Inner part of the fuel cap.")]
    private GameObject innerFuelCap;

    [Header("Data Storage")]
    /// <summary>
    /// Dictionary to store descriptive frame data.
    /// </summary>
    private Dictionary<string, DescriptiveFrameData> frameDataDictionary;

    /// <summary>
    /// Dictionary to store general frame data.
    /// </summary>
    private Dictionary<string, FrameData> allFrameDataDictionary;

    /// <summary>
    /// Path to the directory where captures are saved.
    /// </summary>
    private string directoryPath;

    [Header("Scene Status")]
    /// <summary>
    /// Flag to determine if the start position has been found.
    /// </summary>
    private bool startPosFound;

    /// <summary>
    /// The start position of the camera.
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// Counts number of images taken.
    /// </summary>
    private int countCaptures;

    /// <summary>
    /// Trail number for quick unique image naming.
    /// </summary>
    private int imageTrail;

    /// <summary>
    /// Index to keep track of the current scene data
    /// </summary>
    private int sceneDataIndex;



    /// <summary>
    /// Initializes the scene settings and prepares data structures at the start of the scene.
    /// </summary>
    void Start()
    {
        InitializeDirectory();
        InitializeDataStructures();
        InitializeSceneSettings();
    }

    /// <summary>
    /// Initializes the directory for saving capture data.
    /// </summary>
    private void InitializeDirectory()
    {
        directoryPath = Path.Combine(Application.dataPath, "../../Captures/");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Initializes dictionaries used for storing frame data.
    /// </summary>
    private void InitializeDataStructures()
    {
        frameDataDictionary = new Dictionary<string, DescriptiveFrameData>();
        allFrameDataDictionary = new Dictionary<string, FrameData>();
    }

    /// <summary>
    /// Sets initial scene settings and parameters.
    /// </summary>
    private void InitializeSceneSettings()
    {
        startPosFound = false;
        countCaptures = 0;
        sceneDataIndex = 0;
        ApplyInitialSceneData();
    }

    /// <summary>
    /// Applies the initial scene data based on the first element of the scene data list.
    /// </summary>
    private void ApplyInitialSceneData()
    {
        if (sceneDataList != null && sceneDataList.Count > 0)
        {
            SetTerrain(sceneDataList[0].GetTerrainLayer());
        }
        else
        {
            Debug.LogError("Scene data list is empty or not set.");
        }
    }




    void Update()
    {
        if (!startPosFound)
        {
            FindStartPos();
        }
        else
        {
            AdjustPerFrame();
            CollectImageData();
        }
    }




    /// <summary>
    /// Finds and sets the starting position of the camera based on bounding box constraints.
    /// </summary>
    private void FindStartPos()
    {
        List<Vector2> bbox = FindBbox();

        // Adjust camera position based on bounding box constraints
        AdjustCameraPositionForBoundingBox(bbox);

        // If the bounding box is out of bounds in both x and y, set the start position
        if (!InBoundsx(bbox) && !InBoundsy(bbox))
        {
            SetStartPos();
        }
    }

    /// <summary>
    /// Adjusts the camera position based on bounding box constraints.
    /// </summary>
    /// <param name="bbox">Bounding box coordinates.</param>
    private void AdjustCameraPositionForBoundingBox(List<Vector2> bbox)
    {
        Vector3 positionAdjustment = Vector3.zero;

        if (InBoundsx(bbox))
        {
            positionAdjustment.x += 0.01f;
        }

        if (InBoundsy(bbox))
        {
            positionAdjustment.y -= 0.01f;
        }

        cam.transform.position += positionAdjustment;
    }

    /// <summary>
    /// Sets the start position for the camera.
    /// </summary>
    private void SetStartPos()
    {
        cam.transform.position += new Vector3(-0.05f, 0.05f, 0);
        startPos = cam.transform.position;
        startPosFound = true;
    }




    /// <summary>
    /// Takes a photo and captures frame data.
    /// This method is responsible for capturing images and their corresponding frame data 
    /// based on bounding box and keypoint data. 
    /// </summary>
    private void CollectImageData()
    {
        List<Vector2> initialbbox = FindBbox();
        if (!InBoundsx(initialbbox) || !InBoundsy(initialbbox))
        {
            return; // Bounding box not within required bounds, exit the method
        }

        SceneData sceneData = sceneDataList[sceneDataIndex];
        if (countCaptures < sceneData.GetTotalPhotos())
        {
            CaptureAndStoreImage(sceneData);
        }
        else
        {
            SaveFrameDataAndReset(sceneData);
        }
    }

    /// <summary>
    /// Captures an image and stores the corresponding frame data.
    /// </summary>
    /// <param name="sceneData">Data for the current scene.</param>
    private void CaptureAndStoreImage(SceneData sceneData)
    {
        Debug.Log("Images Taken On Scene: " + countCaptures);

        imageTrail++;
        countCaptures++;

        Vector2 gameViewSize = Handles.GetMainGameViewSize();
        DateTime now = DateTime.Now;
        string timestamp = now.ToString("yyyyMMddHHmmss") + imageTrail.ToString();
        List<Vector2> bbox = FindBbox();

        DescriptiveFrameData currentFrameData = new DescriptiveFrameData(fuelCap.transform, bbox, keypoints);
        FrameData frameData = new FrameData(fuelCap.transform, bbox, keypoints, sceneData.GetIntendedDirName());

        frameDataDictionary.Add("sim_" + timestamp, currentFrameData);
        allFrameDataDictionary.Add("sim_" + timestamp, frameData);

        captureImages.Save(directoryPath, sceneData.GetIntendedDirName(), timestamp, width: (int)gameViewSize.x, height: (int)gameViewSize.y);
    }

    /// <summary>
    /// Saves all captured frame data and updates the scene/environment.
    /// </summary>
    /// <param name="sceneData">Data for the current scene.</param>
    private void SaveFrameDataAndReset(SceneData sceneData)
    {
        string savePath = Path.Combine(directoryPath, "Images", sceneData.GetIntendedDirName());
        captureImages.SaveDescriptiveFrames(savePath, frameDataDictionary);
        frameDataDictionary.Clear();
        countCaptures = 0;
        UpdateScene();
    }




    /// <summary>
    /// Updates the current scene based on the scene data index.
    /// </summary>
    private void UpdateScene()
    {
        if (sceneDataIndex + 1 < sceneDataList.Count)
        {
            sceneDataIndex++;
            SceneData sceneData = sceneDataList[sceneDataIndex];
            ApplySceneData(sceneData);
        }
        else
        {
            EndScene();
        }
    }

    /// <summary>
    /// Applies the settings of the scene data to the current scene.
    /// </summary>
    /// <param name="sceneData">The scene data to apply.</param>
    private void ApplySceneData(SceneData sceneData)
    {
        directionalLight.intensity = sceneData.GetLightIntensity();
        SetTerrain(sceneData.GetTerrainLayer());
        ChangeMaterial(sceneData.GetMaterial());
        ResetCameraPosition();
    }

    /// <summary>
    /// Changes the material of the target object.
    /// </summary>
    /// <param name="fuelCapMaterial">The new material to be applied to the target object.</param>
    void ChangeMaterial(Material fuelCapMaterial)
    {
        Renderer renderer = innerFuelCap.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = fuelCapMaterial;
        }
        else
        {
            Debug.LogError("Renderer component not found on the target object.");
        }
    }

    /// <summary>
    /// Sets the terrain based on the specified terrain layer.
    /// </summary>
    /// <param name="terrainLayer">The terrain layer to apply.</param>
    private void SetTerrain(int terrainLayer)
    {
        Terrain terrain = terrainObj.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Terrain component not found.");
            return;
        }

        UpdateTerrainLayer(terrain, terrainLayer);
    }

    /// <summary>
    /// Updates the terrain layer.
    /// </summary>
    /// <param name="terrain">The terrain to update.</param>
    /// <param name="newLayerIndex">The new layer index to set.</param>
    private void UpdateTerrainLayer(Terrain terrain, int newLayerIndex)
    {
        TerrainData terrainData = terrain.terrainData;
        float[,,] alphaMap = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    alphaMap[x, y, i] = i == newLayerIndex ? 1.0f : 0.0f;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }




    /// <summary>
    /// Adjusts the position of the camera based on the bounding box constraints.
    /// </summary>
    private void AdjustPerFrame()
    {
        List<Vector2> bbox = FindBbox();

        if (InBoundsx(bbox))
        {
            MoveCapRight();
            RandomizeCapRotation();
        }
        else if (InBoundsy(bbox))
        {
            MoveCapDown(startPos);
        }
        else
        {
            float distance = RandomizeCapDistance(offsetz.x, offsetz.y);
            CenterCap(distance);
        }
    }

    /// <summary>
    /// Moves the camera down by a predefined offset and adjusts its position based on the start position.
    /// </summary>
    /// <param name="startPos">The starting position of the camera.</param>
    private void MoveCapDown(Vector3 startPos)
    {
        cam.transform.position += new Vector3(0, offsety, 0);
        cam.transform.position = new Vector3(startPos.x, cam.transform.position.y, cam.transform.position.z);

        if (!InBoundsx(FindBbox()))
        {
            Debug.LogWarning("Fuel cap out of bounds on x axis immediately after downward movement. Moving fuel cap right by offset.");
            MoveCapDown(startPos - new Vector3(offsetx, 0, 0));
        }
    }

    /// <summary>
    /// Moves the camera to the right by a predefined offset.
    /// </summary>
    private void MoveCapRight()
    {
        cam.transform.position += new Vector3(-offsetx, 0, 0);
    }

    /// <summary>
    /// Randomizes the rotation of the fuel cap.
    /// </summary>
    private void RandomizeCapRotation()
    {
        float randX = UnityEngine.Random.Range(-50f, 50f);
        float randY = UnityEngine.Random.Range(-50f, 50f);
        float randZ = UnityEngine.Random.Range(0f, 360f);
        float randXLight = UnityEngine.Random.Range(20f, 90f);
        float randYLight = UnityEngine.Random.Range(-30f, 70f);
        directionalLight.transform.rotation = Quaternion.Euler(randXLight, randYLight, 0);


        fuelCap.transform.rotation = Quaternion.Euler(randX, randY, randZ);
    }

    /// <summary>
    /// Randomizes the distance of the fuel cap within a specified range.
    /// </summary>
    /// <returns>The randomized distance value.</returns>
    private float RandomizeCapDistance(float nearDistance, float farDistance)
    {
        startPosFound = false;
        return UnityEngine.Random.Range(nearDistance, farDistance);
    }

    /// <summary>
    /// Centers the camera at a specific Z position.
    /// </summary>
    /// <param name="zpos">The Z position to center the camera at.</param>
    private void CenterCap(float zpos)
    {
        cam.transform.position = new Vector3(0, 0, zpos);
    }
    private List<Vector2> FindBbox()
    {
        if (innerFuelCap == null)
        {
            Debug.LogError("Target is not assigned.");
            return new List<Vector2> { new Vector2 (0, 0) };
        }

        Camera camera = Camera.main.GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("Camera component not found.");
            return new List<Vector2> { new Vector2(0, 0) };
        }
        MeshFilter meshFilter = innerFuelCap.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found on the target.");
            return new List<Vector2> { new Vector2(0, 0) };
        }

        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // World space
            vertices[i] = innerFuelCap.transform.TransformPoint(vertices[i]);
            // GUI space
            vertices[i] = camera.WorldToScreenPoint(vertices[i]);
            vertices[i].y = vertices[i].y;
        }

        Vector3 min = vertices[0];
        Vector3 max = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            min = Vector3.Min(min, vertices[i]);
            max = Vector3.Max(max, vertices[i]);
        }
        List<Vector2> minmax = new List<Vector2>();
        // Construct a rect of the min and max positions and draw the box
        minmax.Add(max);
        minmax.Add(min);
        return minmax;
    }

    /// <summary>
    /// Checks if the bounding box is within the screen bounds along the X-axis.
    /// </summary>
    /// <returns>True if the bounding box is within the X-axis bounds, otherwise false.</returns>
    private bool InBoundsx(List<Vector2> bbox)
    {
        // Check if the bounding box is within the screen width
        return !(Screen.width < bbox[0].x || bbox[1].x < 0);
    }

    /// <summary>
    /// Checks if the bounding box is within the screen bounds along the Y-axis.
    /// </summary>
    /// <returns>True if the bounding box is within the Y-axis bounds, otherwise false.</returns>
    private bool InBoundsy(List<Vector2> bbox)
    {
        // Check if the bounding box is within the screen height
        return !(Screen.height < bbox[0].y || bbox[1].y < 0);
    }

    /// <summary>
    /// Resets the camera position to the origin.
    /// </summary>
    private void ResetCameraPosition()
    {
        cam.transform.position = Vector3.zero;
        startPosFound = false;
    }

    /// <summary>
    /// Ends the scene and exits play mode in the editor.
    /// </summary>
    private void EndScene()
    {
        Debug.Log("Quitting the scene.");
        EditorApplication.isPlaying = false;
    }
    /// <summary>
    /// Called when the object is destroyed. Saves all frame data.
    /// </summary>
    private void OnDestroy()
    {
        // Save all frame data to the specified directory
        captureImages.SaveAllFrameData(directoryPath, allFrameDataDictionary);
    }

}
