using UnityEngine;
using UnityEngine.UI;
public class DebugToucher : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)// to be able to toggle a Toggle
    {
        // Check if the object is a Toggle
        Toggle toggle = other.GetComponent<Toggle>();
        if (toggle != null)
        {
            // Toggle the state
            toggle.isOn = !toggle.isOn;
        }
    }
}
