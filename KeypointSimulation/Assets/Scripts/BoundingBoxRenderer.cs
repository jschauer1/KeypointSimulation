using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


/// <summary>
///  A Class that draws a bounding box around a given <see cref="GameObject"/> from the perspective of the <see cref="Camera"/> that this script is attached to.
/// </summary>
public class BoundingBoxRenderer : MonoBehaviour
{
    public GameObject target; // Assign this in the Unity Editor
    void OnGUI()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned.");
            return;
        }

        Camera camera = GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("Camera component not found.");
            return;
        }

        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found on the target.");
            return;
        }

        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // World space
            vertices[i] = target.transform.TransformPoint(vertices[i]);
            // GUI space
            vertices[i] = camera.WorldToScreenPoint(vertices[i]);
            vertices[i].y = Screen.height - vertices[i].y;
        }

        Vector3 min = vertices[0];
        Vector3 max = vertices[0];
        for (int i = 1; i < vertices.Length; i++)
        {
            min = Vector3.Min(min, vertices[i]);
            max = Vector3.Max(max, vertices[i]);
        }

        // Construct a rect of the min and max positions and draw the box
        Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        GUI.Box(r, "");
    }


}

public class BoundingBoxInfo
{
    public int ObjectId { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float TimeSecs { get; private set; }

    public BoundingBoxInfo(int objectId, int x, int y, int width, int height, float timeSecs)
    {
        ObjectId = objectId;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        TimeSecs = timeSecs;
    }
}
