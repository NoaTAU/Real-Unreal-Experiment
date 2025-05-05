# TAUXR Framework Documentation

## 🎯 Quick Start Guide

### Project Setup
1. Import the TAUXR package into your Unity project
2. Open the Base Scene from `Assets/TAUXR/Base Scene/Base Scene.unity`
3. The scene includes all necessary prefabs and managers
4. Use `ProjectInitializer.cs` to set up your project-specific configurations

### Core Components
- `TXRPlayer`: Central VR player management
- `TXRDataManager`: Data logging and analytics
- `TXR_RoomCalibrator`: Room setup and calibration

## 📁 Directory Structure

```
Assets/TAUXR/
├── Base Scene/           # Core scene and prefabs
├── Shaders/             # Custom shaders
├── Meta components/     # Meta-specific integrations
├── Utilities/          # Helper scripts
├── Detectors/          # Interaction detection
└── Flow Management/    # Experimental flow system
```

## 🎮 Core Systems

### 1. Player System (`TXRPlayer.cs`)
The central VR player management system.

#### Key Features
- Hand tracking
- Eye tracking
- Face tracking
- Input management
- Passthrough control

#### Usage Example
```csharp
// Access player components
var headPosition = TXRPlayer.Instance.PlayerHead.position;
var rightHand = TXRPlayer.Instance.HandRight;

// Enable passthrough
TXRPlayer.Instance.SetPassthrough(true);

// Fade view
TXRPlayer.Instance.FadeViewToColor(Color.black, 1f);
```

### 2. Data Logging System (`TXRDataManager.cs`)
Comprehensive data collection and logging system.

#### Logged Data
- Head position and rotation
- Hand positions and rotations
- Eye tracking data (when enabled)
- Face expressions
- Custom analytics

#### Usage Example
```csharp
// Log custom analytics
TXRDataManager.Instance.LogLineToFile("CustomEvent,Value1,Value2");

// Access continuous data writer
var continuousWriter = TXRDataManager.Instance.ContinuousWriter;
```

### 3. Input System
Two input management options:

#### Pinch-based Input (`PinchingInputManager.cs`)
```csharp
// Wait for pinch
await PinchingInputManager.Instance.WaitForPress(HandType.Right);
```

#### Controller-based Input (`ControllersInputManager.cs`)
```csharp
// Wait for trigger press
await ControllersInputManager.Instance.WaitForPress(HandType.Right);
```

### 4. Interaction System

#### Pinchable Objects
1. Inherit from `APinchable`
2. Implement required methods
3. Add collider component

```csharp
public class MyPinchableObject : APinchable
{
    protected override void OnPinchEnter()
    {
        // Handle pinch start
    }

    protected override void OnPinchExit()
    {
        // Handle pinch end
    }
}
```

## 👁️ Eye Tracking System

The TAUXR Eye Tracking system provides comprehensive eye gaze tracking capabilities using Meta's OVREyeGaze system.

### Components

1. **TXREyeTracker** (`TXREyeTracker.cs`)
   - Core component for eye tracking functionality
   - Handles gaze detection and object focus
   - Provides real-time eye position and gaze direction

2. **FollowEyeSphere** (`FollowEyeSphere.cs`)
   - Visual feedback component
   - Smoothly follows the current gaze point
   - Useful for debugging and visualization

### Setup

1. Add the `TXREyeTracker` prefab to your scene
2. Configure eye tracking in Oculus settings
3. (Optional) Add `FollowEyeSphere` for visual feedback

### Usage

#### Basic Eye Tracking
```csharp
// Access eye tracking data
var focusedObject = TXRPlayer.Instance.EyeTracker.FocusedObject;
var gazePosition = TXRPlayer.Instance.EyeTracker.EyeGazeHitPosition;
var eyePosition = TXRPlayer.Instance.EyeTracker.EyePosition;

// Check if eye tracking is available
if (TXRPlayer.Instance.IsEyeTrackingEnabled)
{
    // Use eye tracking data
}
```

#### Visual Feedback
```csharp
// Add visual feedback to gaze point
var followSphere = Instantiate(FollowEyeSpherePrefab);
followSphere.lerpSpeed = 5f; // Adjust smoothness
```

### Key Features

1. **Gaze Detection**
   - Tracks where the user is looking
   - Provides hit point in world space
   - Identifies focused objects

2. **Confidence Threshold**
   - Only tracks when confidence is above 0.5
   - Prevents inaccurate tracking
   - Returns `NOTTRACKINGVECTORVALUE` when confidence is low

3. **Layer Masking**
   - Configurable layer mask for gaze detection
   - Default excludes layer 7
   - Customizable through inspector

### Best Practices

1. **Performance**
   - Check confidence before using gaze data
   - Use layer masks to optimize raycasts
   - Consider using the FollowEyeSphere for debugging

2. **Error Handling**
   ```csharp
   if (TXRPlayer.Instance.EyeTracker.EyeGazeHitPosition != NOTTRACKINGVECTORVALUE)
   {
       // Use gaze data
   }
   ```

3. **Visualization**
   - Use `Debug.DrawRay` for gaze direction
   - Implement `FollowEyeSphere` for user feedback
   - Consider adding visual indicators for low confidence

### Troubleshooting

1. **No Eye Tracking Data**
   - Verify eye tracking is enabled in Oculus settings
   - Check if the user has granted permissions
   - Ensure the headset supports eye tracking

2. **Inaccurate Tracking**
   - Adjust the confidence threshold if needed
   - Check for proper headset fit
   - Verify lighting conditions

3. **Missing Components**
   - Ensure `TXREyeTracker` prefab is in the scene
   - Check for proper OVREyeGaze component setup
   - Verify layer mask configuration

## 🏗️ Base Scene Components

The Base Scene provides the foundation for all TAUXR experiences. It includes essential components for room calibration, scene management, and environment setup.

### Project Initialization

#### ProjectInitializer
Entry point for TAUXR experiences. Handles initial setup and calibration.

```csharp
// Configure in inspector
[SerializeField] private bool _shouldProjectUseCalibration;     // Enable physical space calibration
[SerializeField] private bool _shouldCalibrateOnEditor;        // Allow calibration in editor
```

### Room Calibration System

#### TXR_RoomCalibrator
Manages the calibration of the virtual environment to physical space.

##### Components
1. **EnvironmentCalibrator** (`EnvironmentCalibrator.cs`)
   - Handles room calibration process
   - Manages calibration points
   - Adjusts environment scale and position

2. **CalibrationPoint** (Prefab)
   - Visual marker for calibration
   - Used to align virtual and physical space

##### Usage
```csharp
// Start calibration
await EnvironmentCalibrator.Instance.CalibrateRoom();

// Check calibration status
bool isCalibrated = EnvironmentCalibrator.Instance.IsCalibrated;
```

### Scene Management

#### TXRSceneManager
Controls scene transitions and player positioning.

##### Components
1. **TXRSceneManager** (`TXRSceneManager.cs`)
   - Manages scene loading and transitions
   - Handles player positioning
   - Coordinates with room calibration

2. **PlayerRepositioner** (`PlayerRepositioner.cs`)
   - Handles player position adjustments
   - Manages scene-specific player placement

3. **PlayerScenePositioner** (Prefab)
   - Visual indicator for player positioning
   - Helps with scene setup

##### Usage
```csharp
// Initialize scene manager
TXRSceneManager.Instance.Init(useCalibration: true);

// Reposition player
await PlayerRepositioner.Instance.RepositionPlayer();
```

### Environment Setup

#### Center Model
Provides the base environment for TAUXR experiences.

##### Components
1. **Arena V2** (Prefab)
   - Main environment model
   - Includes floor and walls
   - Configurable materials and textures

2. **Materials**
   - `walls.mat`: Environment wall materials
   - Customizable textures and properties

3. **Models**
   - `Floor.fbx`: Base floor model
   - Additional environment models

##### Setup
1. Place `Arena V2` prefab in scene
2. Configure materials as needed
3. Adjust scale and position based on calibration

### Best Practices

1. **Calibration**
   - Always calibrate in the target physical space
   - Use calibration points for accurate alignment
   - Verify calibration before starting experience

2. **Scene Management**
   - Use `TXRSceneManager` for all scene transitions
   - Implement proper player positioning
   - Handle calibration state appropriately

3. **Environment**
   - Keep environment scale consistent
   - Use provided materials for visual consistency
   - Consider physical space constraints

### Troubleshooting

1. **Calibration Issues**
   - Verify physical space matches virtual space
   - Check calibration point placement
   - Ensure proper lighting for tracking

2. **Scene Loading Problems**
   - Check scene build settings
   - Verify scene manager initialization
   - Ensure proper player positioning

3. **Environment Setup**
   - Verify material assignments
   - Check model scale and orientation
   - Ensure proper collision setup

## 📊 Data Management System

The TAUXR Data Management system provides comprehensive data collection and logging capabilities for VR experiences.

### Components

#### TXRDataManager
Central manager for all data logging operations.

##### Core Components
1. **TXRDataManager** (`TXRDataManager.cs`)
   - Coordinates all data writers
   - Manages file streams
   - Handles data collection timing

2. **DataContinuousWriter** (`DataContinuousWriter.cs`)
   - Logs transform data (position, rotation)
   - Configurable logging intervals
   - Supports multiple tracked objects

3. **DataExporterFaceExpression** (`DataExporterFaceExpression.cs`)
   - Captures facial expression data
   - Logs blendshape weights
   - Integrates with OVRFaceExpressions

4. **AnalyticsWriter** (`AnalyticsWriter.cs`)
   - Handles custom analytics events
   - Supports multiple data tables
   - Manages CSV file organization

### Usage

#### Basic Data Logging
```csharp
// Log custom analytics
TXRDataManager.Instance.LogLineToFile("EventName,Value1,Value2");

// Access continuous data writer
var continuousWriter = TXRDataManager.Instance.ContinuousWriter;
```

#### Transform Tracking
```csharp
// Add transform to track
continuousWriter.AddTransformToTrack(transform);

// Configure logging interval
continuousWriter.SetLoggingInterval(0.1f); // Log every 100ms
```

#### Face Expression Logging
```csharp
// Initialize face tracking
var faceExporter = TXRDataManager.Instance.FaceExporter;
faceExporter.Init();

// Start logging
faceExporter.StartLogging();
```

### Data Structure

#### Continuous Data
- Head position and rotation
- Hand positions and rotations
- Eye tracking data (when enabled)
- Custom tracked transforms

#### Face Expression Data
- All OVRFaceExpressions blendshapes
- Timestamp for each frame
- Confidence values

#### Analytics Data
- Custom event names
- Timestamp
- Event-specific parameters

### Best Practices

1. **File Management**
   - Use unique file names for each session
   - Implement proper file closing
   - Handle file write permissions

2. **Performance**
   - Adjust logging intervals based on needs
   - Monitor memory usage with large datasets
   - Use appropriate data compression

3. **Data Organization**
   - Use consistent naming conventions
   - Implement proper data validation
   - Structure analytics events logically

### Troubleshooting

1. **File Access Issues**
   - Verify write permissions
   - Check disk space
   - Ensure proper file paths

2. **Data Collection Problems**
   - Verify component initialization
   - Check tracking references
   - Monitor logging intervals

3. **Performance Issues**
   - Adjust logging frequency
   - Optimize tracked transforms
   - Monitor memory usage

## 🔄 Experimental Flow System

### Session Management
```csharp
// Start a session
await SessionManager.Instance.RunSessionFlow();

// Configure rounds
var round = new Round();
// Add trials to round
```

### Trial Management
```csharp
// Create a trial
var trial = new Trial();
// Configure trial parameters

// Run trial
await TrialManager.Instance.RunTrialFlow(trial);
```

## 🛠️ Best Practices

1. **Player Setup**
   - Always use `TXRPlayer.Instance` to access player components
   - Check for null references when accessing optional features (eye tracking, face tracking)

2. **Data Logging**
   - Use `TXRDataManager` for all data collection
   - Implement custom analytics through the analytics system
   - Close writers properly in `OnApplicationQuit`

3. **Input Handling**
   - Use async/await pattern for input waiting
   - Implement both pinch and controller input for maximum compatibility
   - Test input on both hands

4. **Performance**
   - Use object pooling for frequently instantiated objects
   - Implement proper cleanup in `OnDestroy`
   - Monitor eye tracking confidence before using gaze data

## 🔍 Troubleshooting

### Common Issues
1. **Hand Tracking Not Working**
   - Check OVRHand prefab setup
   - Verify hand tracking is enabled in Oculus settings

2. **Eye Tracking Issues**
   - Ensure eye tracking is enabled in project settings
   - Check confidence values before using gaze data

3. **Data Logging Problems**
   - Verify write permissions in target directory
   - Check if all required components are initialized

## 📚 Additional Resources

- Check the `Examples` folder for sample implementations
- Review the `Utilities` folder for helper scripts
- See `Meta components` for platform-specific features

## 🔄 Version History

- Current Version: 1.0.0
- Last Updated: [Current Date]
- Compatible with Unity 2021.3+
- Requires Oculus Integration 47.0+

