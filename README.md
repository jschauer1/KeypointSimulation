# Keypoint Simulation

This document provides a comprehensive guide on setting up and using the Keypoint Simulation in Unity. The project simulates keypoint detection on a fuelcap using a Unity environment. See [RealsenseFuelcap](https://github.com/keenanbuckley/RealsenseFuelcap) for more information on the project

The simulation is used to quickly generate thousands of annotated images for machine learning model training. 
## Dependencies
- **Unity:** The only dependency for this project is Unity. Install unity 2023.1.3(Other versions will likely work as well).

## Setup Instructions

### Cloning and Opening the Project
1. **Clone the Project:** Start by cloning the project from GitHub. You can find the project at [KeypointSimulation](https://github.com/jschauer1/KeypointSimulation.git).
2. **Open the Project in Unity:**
    - Launch Unity Hub.
    - Click on "Open".
    - Navigate to the cloned project directory.
    - Open the `KeypointSimulation` folder within the project directory.
3. **Adjust Project Settings:**
    - Once the project loads in Unity, set the resolution to 1280x720.

### Image Quality and Lighting
- **Check and Adjust Lighting:**
    - Compare the project's image quality with the provided sample image.
    - If the lighting differs significantly, bake the lighting:
        - Go to `Window > Rendering > Lighting`.
        - Click on "Generate Lighting", this may take a few minutes.

### Starting the Simulation and Recording Images
- **Begin the Simulation:**
    - To start recording images, locate and click the grey play button in the Unity interface. This will initiate the image capture process as defined in the SimulationController settings. Unity will auto exit when images have been collected.

## Key GameObjects

### SimulationController
- **Functionality:** Manages the simulation. It controls image capture, lighting, and the material of the fuel cap for each scene in the sceneDataList.
- **Components:**
    - A list of scene objects to configure images, lighting, and fuelcap material.
    - Access to essential GameObjects like the camera and fuelcap, this allows it to manipulate their positions and orientation.
    - Settings for frame-by-frame changes to the fuelcap.

### Fuelcap
- **Description:** A GameObject representing the fuelcap, imported as an .obj scaled to 0.001 to match real-world dimensions.
- **Features:** 
    - 10 child GameObjects to mark the positions of the keypoints.

### Terrain
- **Purpose:** Provides the background for the simulated photos.

### Camera
- **Configuration:** A modified default Unity camera.
- **Settings:** 
    - Horizontal POV is set to 84 to emulate the real-world camera.

## Data Collection

- **Data Storage Method:** Data is organized and saved in JSON files, which offer a structured and easily readable format.
- **Image and Data Correlation:** Each image is associated with a unique frame key, stored in a text file. This setup allows for straightforward referencing between images and their corresponding data.
- **JSON Structure Overview:**
  - **Image Identifier:** The JSON file is keyed by a unique identifier for each image.
  - **Keypoints:** This section stores an array of keypoints, with each keypoint represented by its coordinates and an identifier. It provides critical data for the keypoint detection aspect of the simulation.
  - **Camera Data:** Contains details about the camera's position and orientation during the simulation. This helps in understanding the perspective from which each image was captured.
  - **Fuel Cap Data:** Includes information about the fuel cap's position and orientation. This helps in understanding the perspective from which each image was captured.
  - **Bounding Box (bbox):** Describes the bounding box for the object in the image, including the top left corner and bottom right corner.
  - **Image Directory (img_dir):** Indicates the directory where the corresponding image for the data is stored, ensuring easy retrieval of visual data.

## Contributors

- [Jaxon Schauer](https://github.com/jschauer1) - <https://github.com/jschauer1>
- [Keenan Buckley](https://github.com/keenanbuckley) - <https://github.com/keenanbuckley>
- [David Munro](https://github.com/damunro) - <https://github.com/damunro>
- [Xavier Cotton](https://github.com/Eldarch) - <https://github.com/Eldarch>

## License

This project uses a permissive BSD Zero-Clause License. For more information, see the accompanying [LICENSE](/LICENSE) file.
