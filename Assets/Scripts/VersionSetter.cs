using UnityEngine;
using UnityEngine.UI;

public class VersionSetter : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = "SCP: Intrusion " + Application.version + " | Made With Unity Ver. " + Application.unityVersion;
    }
}
