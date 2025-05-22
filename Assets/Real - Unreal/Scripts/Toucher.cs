using UnityEngine;

public class EditorToucher : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rayLength = 0.1f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Jump"); // Space/ctrl for up/down
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, moveY, moveZ) * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
            {
                Debug.Log("Touched: " + hit.collider.gameObject.name);
                // Example: trigger UnityEvent on Interactable or send message
                hit.collider.SendMessage("OnTouch", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
// This script allows the user to move a GameObject in the scene using WASD or arrow keys
// and to simulate a touch event by clicking the mouse. The script uses raycasting to detect