using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SavingExperiment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        saveExperiment();
    }

    void saveExperiment()
    {
        string directoryPath = Path.Combine(Application.dataPath, "../../Captures/");

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Construct the full file path
        string filePath = Path.Combine(directoryPath, "FrameKeys.txt");

        // Create and write to the file
        // If the file already exists, this will overwrite it
        // To append to the file, use File.AppendText instead
        File.WriteAllText(filePath, "Initial content for FrameKeys.txt");

        Debug.Log("File created at: " + filePath);
    }
}
