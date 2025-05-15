using UnityEngine;

public class LogHelper : MonoBehaviour
{
    public static void Log(string message, string color = null)
    {
        if (color != null)
        {
            print(message.Color(color));
            return;
        }
        print(message);

    }

}
