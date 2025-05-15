using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure only the first object is active at start
        if (objects.Count > 0)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].SetActive(i == 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleObjectsvis()
    {
        if (objects.Count == 0) return;

        // Find the next valid object index
        int nextIndex = (currentIndex + 1) % objects.Count;
        while (objects[nextIndex] == null && nextIndex != currentIndex)
        {
            nextIndex = (nextIndex + 1) % objects.Count;
        }

        // If we found a valid next object
        if (objects[nextIndex] != null)
        {
            // Deactivate current object if it exists
            if (objects[currentIndex] != null)
            {
                objects[currentIndex].SetActive(false);
            }

            // Activate next object
            objects[nextIndex].SetActive(true);
            currentIndex = nextIndex;
        }
    }
}
