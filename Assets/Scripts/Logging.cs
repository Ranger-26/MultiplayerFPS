using System;
using System.IO;
using UnityEngine;

public class Logging : MonoBehaviour
{
    string filename = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string d = System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.ApplicationData) + "/SCP_INTRUSION_LOGS";
        System.IO.Directory.CreateDirectory(d);
        filename = d + "/scp_intrusion_log.txt";
        if (System.IO.File.Exists(filename))
        {
            File.WriteAllText(filename, String.Empty);
        }
    }

    void OnEnable() { Application.logMessageReceived += Log;  }
    void OnDisable() { Application.logMessageReceived -= Log; }
 
    public void Log(string logString, string stackTrace, LogType type)
    {
        if (filename == "")
        {
            string d = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.ApplicationData) + "/SCP_INTRUSION_LOGS";
            System.IO.Directory.CreateDirectory(d);
            filename = d + "/scp_intrusion_log.txt";
        }
 
        try
        {
            string content = $"[{DateTime.Now}] {logString}";
            System.IO.File.AppendAllText(filename, logString + "\n");
        }
        catch { }
    }
}