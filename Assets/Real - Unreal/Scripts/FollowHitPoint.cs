using UnityEngine;

public class FollowHitPoint : MonoBehaviour
{

    TXRPlayer player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = TXRPlayer.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.EyeGazeHitPosition;
    }
}
