using UnityEngine;
public class UIManager : MonoBehaviour
{
    public GameObject dialogGameObject;
    private Dialog dialog;


    private void Start()
    {
        dialog = new Dialog(dialogGameObject);


        //Test:
        dialog.ShowAndWaitForButtonPress("מאיה מאיה אחד שיפט אחד");


    }

}




