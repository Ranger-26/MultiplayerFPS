using UnityEngine;
using Mirror;

public class TitleScreen : MonoBehaviour
{
    // This is a component on the Canvas in the Menu Scene, used to control the menu, doing things such as exiting the application

    public static TitleScreen Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Connect to IP
    public void Connect(string _ip)
    {
        NetworkManager.singleton.networkAddress = _ip;
        NetworkManager.singleton.StartClient();
    }

    // Starting a Server
    public void Host()
    {
        NetworkManager.singleton.StartServer();
    }
}
