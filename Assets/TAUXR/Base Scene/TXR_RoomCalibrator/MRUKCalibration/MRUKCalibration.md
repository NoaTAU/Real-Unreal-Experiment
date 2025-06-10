# MRUK Calibration System

## Overview
This system enables calibration between real-world and virtual environments by aligning virtual objects with their real-world counterparts, by rotating the vr player. It's particularly useful for static environments that cannot be positoned and rotated by the MRUK spawner while keeping their lighting and performance properties.

## Prerequisites
- A virtual environment with objects that need calibration
- MRUK Calibration scripts
- Reference points in both virtual and real environments
- a headset with an enviroment set up. you should tag the object you want to use for calibration with an "other" tag,
and make sure it is the only object you tagged other. 

## Setup Instructions

### 1. Scene Setup
1. Create an empty GameObject in your scene
2. Attach the `MRUKCalibration` script to this GameObject
3. Create two empty GameObjects to serve as reference points:
   - Position reference point
   - Rotation reference point
   - Place these at strategic locations in your virtual environment (e.g., two corners of a desk)

### 2. Prefab Configuration
1. In your target prefab (e.g., desk):
   - Add two empty GameObjects
   - Attach the `MRUKReferencePoint` script to each
   - Configure one as position reference and one as rotation reference
   - Ensure these points match the corresponding points in your virtual environment

### 3. Spawning
- Use the `BiggestAnchorPrefabSpawner` script to instantiate your prefab in the scene

## Usage
1. Once the scene is set up and the prefab is spawned
2. Call the `StartCalibration()` method on the `MRUKCalibration` script
3. The system will automatically align the virtual environment with the real-world position

## Notes
- This system is ideal for static environments that cannot be repositioned
- Ensure reference points are placed at easily identifiable locations
- The calibration process will move the player to match the real-world position in the virtual environment
Make sure World Locking is disabled on the MRUK