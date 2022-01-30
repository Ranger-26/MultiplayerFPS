using UnityEngine;
using UnityEngine.UI;

public class ConnectionButton : MonoBehaviour
{
    [SerializeField]
    InputField input;

    public void ConnectToIP()
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            TitleScreen.Instance.Connect(input.text);
        }
    }
}
