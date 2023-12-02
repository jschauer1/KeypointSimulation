using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Author: Jaxon Schauer
/// The SaveData class is a MonoBehaviour responsible for handling data persistence in a Unity application.
/// It provides functionalities for saving different types of data, such as frame data, images, and other
/// relevant information, typically related to game states, configurations, or user progress. This class
/// might include methods for saving to various formats (like JSON or binary files) and managing directories,
/// ensuring data integrity and easy access. It's designed to work in conjunction with other components
/// that generate or manipulate data that needs to be persisted across sessions or used for analytics and reporting.
/// </summary>
public class SaveData : MonoBehaviour
{
    /// <summary>
    /// Initiates the saving of an image from the main camera after the current frame.
    /// </summary>

    public void Save(string directory, string directoryName, string timestamp, int width = -1, int height = -1)
    {
        // Start a coroutine to save the image at the end of the frame
        StartCoroutine(WaitForEndOfFrameAndSave(directory, directoryName, timestamp, width, height));
    }

    /// <summary>
    /// Coroutine that waits for the end of the frame and then saves an image from the main camera.
    /// </summary>
    /// <returns>IEnumerator for coroutine sequencing.</returns>
    private IEnumerator WaitForEndOfFrameAndSave(string directory, string directoryName, string timestamp, int width, int height)
    {
        // Wait until the end of the frame so all camera rendering is complete
        yield return new WaitForEndOfFrame();

        // Call the method to save the image from the camera
        SaveImg(Camera.main, directory, directoryName, timestamp, width, height);
    }


    /// <summary>
    /// Saves an image captured from the camera to a file.
    /// </summary>
    private void SaveImg(Camera cam, string directoryPath, string directoryName, string timestamp, int width, int height)
    {
        // Create necessary directories if they do not exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (!Directory.Exists(Path.Combine(directoryPath, "Images")))
        {
            Directory.CreateDirectory(Path.Combine(directoryPath, "Images"));
        }
        string imageDirectory = Path.Combine(directoryPath, "Images", directoryName);
        if (!Directory.Exists(imageDirectory))
        {
            Directory.CreateDirectory(imageDirectory);
        }

        // Full path for the image file
        string imagePath = Path.Combine(imageDirectory, $"sim_{timestamp}.png");

        // Create a new texture to store the captured image
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Save the current camera's render target
        var prevActiveRT = RenderTexture.active;
        var prevCameraRT = cam.targetTexture;

        // Render the camera's view to the texture
        cam.Render();

        // Set the active render texture to capture the camera's output
        RenderTexture.active = cam.targetTexture;

        // Read pixels from the render texture into the new texture
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // Encode the texture as a PNG
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(imagePath, bytes);

        // Restore the previous render target settings
        cam.targetTexture = prevCameraRT;
        RenderTexture.active = prevActiveRT;

        // Clean up the created texture to free memory
        Object.Destroy(tex);
    }
    /// <summary>
    /// Saves descriptive frame data to a JSON file, appending to existing data if available.
    /// </summary>
    /// <param name="directoryPath">The directory path to save the JSON file.</param>
    /// <param name="frameDataDictionary">The dictionary containing frame data to be saved.</param>
    public void SaveDescriptiveFrames(string directoryPath, Dictionary<string, DescriptiveFrameData> frameDataDictionary)
    {
        // Flag to check if existing JSON file is present
        bool exists = false;
        string jsonFileName = "DescriptiveFrameData.json";
        string jsonFilePath = Path.Combine(directoryPath, jsonFileName);

        // Read existing JSON if it exists
        string existingJson = "";
        if (File.Exists(jsonFilePath))
        {
            existingJson = File.ReadAllText(jsonFilePath);
            // Remove the closing brace of the existing JSON
            existingJson = existingJson.TrimEnd('}');
            if (existingJson.Length > 1)
            {
                // If existing JSON is not empty, prepare for concatenation
                existingJson += ",";
            }
            exists = true;
        }

        // Serialize the new data to JSON, trimming the starting brace for concatenation
        string newJson = JsonConvert.SerializeObject(frameDataDictionary, Formatting.Indented).TrimStart('{');

        // Concatenate old and new JSON
        string combinedJson = exists ? existingJson + newJson : "{" + existingJson + newJson;

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Write the combined JSON to file
        File.WriteAllText(jsonFilePath, combinedJson);
        Debug.Log($"JSON Data saved to {jsonFilePath}");

        SaveFrameKeys(directoryPath, frameDataDictionary);
    }
    /// <summary>
    /// Saves all frame data to a JSON file, appending to existing data if it exists.
    /// </summary>
    /// <param name="directoryPath">The directory path to save the JSON file.</param>
    /// <param name="frameDataDictionary">The dictionary containing the frame data to be saved.</param>
    public void SaveAllFrameData(string directoryPath, Dictionary<string, FrameData> frameDataDictionary)
    {
        // Flag to check if the existing JSON file is present
        bool exists = false;
        string jsonFileName = "AllFrameData.json";
        string jsonFilePath = Path.Combine(directoryPath, jsonFileName);

        // Read existing JSON if it exists
        string existingJson = "";
        if (File.Exists(jsonFilePath))
        {
            existingJson = File.ReadAllText(jsonFilePath);
            // Remove the closing brace of the existing JSON
            existingJson = existingJson.TrimEnd('}');
            if (existingJson.Length > 1)
            {
                // If existing JSON is not empty, prepare for concatenation
                existingJson += ",";
            }
            exists = true;
        }

        // Serialize the new data to JSON, trimming the starting brace for concatenation
        string newJson = JsonConvert.SerializeObject(frameDataDictionary, Formatting.Indented).TrimStart('{');

        // Concatenate old and new JSON
        string combinedJson = exists ? existingJson + newJson : "{" + existingJson + newJson;

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Write the combined JSON to file
        File.WriteAllText(jsonFilePath, combinedJson);
        Debug.Log($"JSON Data saved to {jsonFilePath}");

        //save frame key
        SaveFrameKeys(directoryPath, frameDataDictionary);
    }


    /// <summary>
    /// Saves the keys of frame data to a text file, appending new keys to avoid duplication.
    /// </summary>
    /// <param name="directoryPath">The directory path to save the text file.</param>
    /// <param name="frameDataDictionary">The dictionary containing the frame data.</param>
    private void SaveFrameKeys(string directoryPath, Dictionary<string, FrameData> frameDataDictionary)
    {
        string textFileName = "FrameKeys.txt";
        string textFilePath = Path.Combine(directoryPath, textFileName);
        StringBuilder sb = new StringBuilder();

        // Check if the file already exists
        bool fileExists = File.Exists(textFilePath);
        HashSet<string> existingKeys = new HashSet<string>();

        // If the file exists, read existing keys to avoid duplication
        if (fileExists)
        {
            string[] currentKeys = File.ReadAllLines(textFilePath);
            foreach (var key in currentKeys)
            {
                existingKeys.Add(key);
            }
        }

        // Append new keys that are not already in the file
        foreach (var key in frameDataDictionary.Keys)
        {
            if (!existingKeys.Contains(key))
            {
                sb.AppendLine(key);
            }
        }

        // Append new keys to the file
        File.AppendAllText(textFilePath, sb.ToString());
        Debug.Log($"Frame keys saved to {textFilePath}");
    }
    /// <summary>
    /// Saves the keys of frame data to a text file, appending new keys to avoid duplication.
    /// </summary>
    /// <param name="directoryPath">The directory path to save the text file.</param>
    /// <param name="frameDataDictionary">The dictionary containing the frame data.</param>
    private void SaveFrameKeys(string directoryPath, Dictionary<string, DescriptiveFrameData> frameDataDictionary)
    {
        string textFileName = "FrameKeys.txt";
        string textFilePath = Path.Combine(directoryPath, textFileName);
        StringBuilder sb = new StringBuilder();

        // Check if the file already exists
        bool fileExists = File.Exists(textFilePath);
        HashSet<string> existingKeys = new HashSet<string>();

        // If the file exists, read existing keys to avoid duplication
        if (fileExists)
        {
            string[] currentKeys = File.ReadAllLines(textFilePath);
            foreach (var key in currentKeys)
            {
                existingKeys.Add(key);
            }
        }

        // Append new keys that are not already in the file
        foreach (var key in frameDataDictionary.Keys)
        {
            if (!existingKeys.Contains(key))
            {
                sb.AppendLine(key);
            }
        }

        // Append new keys to the file
        File.AppendAllText(textFilePath, sb.ToString());
        Debug.Log($"Frame keys saved to {textFilePath}");
    }
}
