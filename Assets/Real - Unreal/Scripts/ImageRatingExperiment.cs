// using System;
// using System.Collections.Generic;
// using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
// using UnityEngine;
// using UnityEngine.UI;


// public class ImageRatingExperiment : RatingExperiment<GameObject>
// {
//     public override string stimuliPath => "Images/2D/2Dsnacks";
//     public GameObject twoDBackground;
//     protected override string ExperimentType => "2D";
//     private List<GameObject> _baselineList = new List<GameObject>();
//     public override List<GameObject> baselineList => _baselineList;

//     public BgToggle bgToggle; // Reference to the BgToggle script to control the background
//     // private string baselinePath = "Images/Baseline";
//     private GameObject currentInstance;
//     private Transform imageParent;

//     protected override void Start()
//     {

//         base.Start();
//         imageParent = SceneReferencer.Instance.twoDDisplay.transform;
//     }

//         // imageDisplay.enabled = false; // Ensure the image display is initially hidden
//         // var baselineArray = Resources.LoadAll<GameObject>(baselinePath);
//         // _baselineList.AddRange(baselineArray);
//         // Debug.Log("Loaded baseline prefabs: " + _baselineList.Count);
//         // base.Start();
//         // // Debug.Log($"Loaded {baselineList.Count} baseline images from {baselinePath}");
//         // Debug.Log(string.Join(", ", baselineList));
//     protected override void InitStimuli()
//     {
//         GameObject snacksPrefab2D = Resources.Load<GameObject>(stimuliPath);
//         if (snacksPrefab2D == null)
//         {
//             Debug.LogError($"Could not load prefab at path: {stimuliPath}");
//             return;
//         }
//         GameObject snacks2DInstance = Instantiate(snacksPrefab2D);
//         snacks2DInstance.SetActive(false);
//         imageParent = SceneReferencer.Instance.twoDDisplay.transform;

//         // Collect each child as a separate stimulus
//         stimuliList = new List<GameObject>();
//         foreach (Transform child in snacks2DInstance.transform)
//         {
//             GameObject childClone = Instantiate(child.gameObject, imageParent);
//             childClone.SetActive(false);
//             if (childClone.name.StartsWith("Bamba_red") || childClone.name.StartsWith("Baflot") || childClone.name.StartsWith("Bisli"))
//             {
//                 // Debug.Log($"Adding {childClone.name} to baseline list");
//                 baselineList.Add(childClone);

//             }
//             else
//             {
//                 stimuliList.Add(childClone);
//                 // Debug.Log($"Adding {childClone.name} to stimuliList list");
//             }
//         }

//         // TXRDataManager.Instance.LogLineToFile($"Loaded {stimuliList.Count} stimuli from prefab {stimuliPath}");
//         // LoadStimuliNames()
//         ShuffleChildren();
//         stimuliListWithBaseline = MergeLists(_baselineList, stimuliList);
//         Debug.Log(string.Join(", ", stimuliList));
//         // Debug.Log("Stimulus count: " + stimuliList.Count);
//         LogHelper.Log("finished init stimuli", "blue");
//         // Optional: destroy the instance since we only needed its children
//         Destroy(snacks2DInstance);
//     }
    
//         protected override void HideStimulus()
//     {
//         bgToggle.UseSolid(); // Switch back to solid color background
//         if (currentInstance != null)
//         {
//             Destroy(currentInstance);
//             currentInstance = null;
//             // twoDBackground.SetActive(false);
//         }

//     }

//     protected override void ShowStimulus()
//     {
//         // twoDBackground.SetActive(true);
//         bgToggle.UseSkybox(); // Use the skybox for 2D images
//         if (currentInstance != null)
//             Destroy(currentInstance);

//         // GameObject prefabToShow = stimuliListWithBaseline[currentStimulusIndex];
//         currentInstance = Instantiate(stimuliListWithBaseline[currentStimulusIndex], imageParent);
//         currentInstance.SetActive(true);

//         // RendererActivator.Instance.ShowRenderers();
       
//     }
    
//     void ShuffleChildren()
//     {
//         int count = imageParent.childCount;
//         for (int i = 0; i < count; i++)
//         {
//             int randomIndex = UnityEngine.Random.Range(i, count);
//             imageParent.GetChild(randomIndex).SetSiblingIndex(i);
//         }
//         // Debug.Log("Shuffled children order.");
//     }


// }