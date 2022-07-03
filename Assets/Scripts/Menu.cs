using Game.UI;
using UnityEngine;

public class Menu : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        MenuOpen.IsOpen = true;
    }

    private void OnDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MenuOpen.IsOpen = false;
    }
}
