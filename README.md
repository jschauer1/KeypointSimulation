# Keypoint Simulation

This document provides a comprehensive guide on setting up and using the Keypoint Simulation in Unity. The project simulates keypoint detection on a fuel cap model using a Unity-based environment.

## Dependencies
- **Unity:** The only dependency for this project is Unity. Install unity 2023.1.3(Other versions will likely work as well).

## Setup Instructions

### Cloning and Opening the Project
1. **Clone the Project:** Start by cloning the project from GitHub. You can find the project at [RealsenseFuelcap](https://github.com/jschauer1/KeypointSimulation.git).
2. **Open the Project in Unity:**
    - Launch Unity Hub.
    - Click on "Open".
    - Navigate to the cloned project directory.
    - Open the `KeypointSimulation` folder within the project directory.
3. **Adjust Project Settings:**
    - Once the project loads in Unity, set the resolution to 1280x720 for optimal display.

### Image Quality and Lighting
- **Check and Adjust Lighting:**
    - Compare the project's image quality with the provided sample images.
    - If the lighting differs significantly, bake the lighting:
        - Go to `Window > Rendering > Lighting`.
        - Click on "Generate Lighting", this may take a few minutes.

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
  - **Image Identifier:** Each JSON file is keyed by a unique identifier for each image.
  - **Keypoints:** This section stores an array of keypoints, with each keypoint represented by its coordinates and an identifier. It provides critical data for the keypoint detection aspect of the simulation.
  - **Camera Data:** Contains details about the camera's position and orientation during the simulation. This helps in understanding the perspective from which each image was captured.
  - **Fuel Cap Data:** Includes information about the fuel cap's position and orientation, which is crucial for accurately correlating the keypoints with the object.
  - **Bounding Box (bbox):** Describes the bounding box for the object in the image, essential for object detection and spatial analysis.
  - **Image Directory (img_dir):** Indicates the directory where the corresponding image for the data is stored, ensuring easy retrieval of visual data.

## Contributors

- Keenan Buckley: [GitHub Profile](https://github.com/keenanbuckley)
- David Munro: [GitHub Profile](https://github.com/damunro)

## License

This project is under the BSD Zero-Clause License. For more details, refer to the [LICENSE](/
