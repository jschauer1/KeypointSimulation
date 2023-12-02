using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DescriptiveFrameData
{
    public List<KeypointData> keypoints;
    public ObjectData cameraData;
    public ObjectData fuelCapData;
    public SerializableBoundingBox bbox;

    public DescriptiveFrameData(Transform FuelCap, List<Vector2> boundingbox, List<Transform> keypoints)
    {
        this.keypoints = new List<KeypointData>();
        cameraData = new ObjectData
        {
            rotation = new SerializableQuaternion(Camera.main.transform.rotation),
            position = new SerializableVector3(Camera.main.transform.position)
        };
        fuelCapData = new ObjectData
        {
            rotation = new SerializableQuaternion(FuelCap.rotation),
            position = new SerializableVector3(FuelCap.position)
        };
        bbox = new SerializableBoundingBox
        {
            topLeft = new SerializableVector2(boundingbox[0]),
            bottomRight = new SerializableVector2(boundingbox[1])
        };
        foreach (Transform keypoint in keypoints)
        {
            this.keypoints.Add(new KeypointData
            {
                pos = new SerializableVector2(Camera.main.WorldToScreenPoint(keypoint.position)),
                name = keypoint.name
            });
        }
    }
    [System.Serializable]
    public struct SerializableVector2
    {
        public float x;
        public float y;

        public SerializableVector2(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }
    }

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
    }

    [System.Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
    }
    [System.Serializable]
    public struct KeypointData
    {
        public SerializableVector2 pos;
        public string name;
    }
    [System.Serializable]
    public struct ObjectData
    {
        public SerializableQuaternion rotation;
        public SerializableVector3 position;
    }
    // Struct to hold all data to be serialized
    [System.Serializable]
    public struct SerializableBoundingBox
    {
        public SerializableVector2 topLeft;
        public SerializableVector2 bottomRight;

        public SerializableBoundingBox(Vector2 topLeft, Vector2 bottomRight)
        {
            this.topLeft = new SerializableVector2(topLeft);
            this.bottomRight = new SerializableVector2(bottomRight);
        }
    }
}
