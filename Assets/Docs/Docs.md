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

