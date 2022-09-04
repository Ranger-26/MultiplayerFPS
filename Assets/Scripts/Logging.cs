using System;
using System.IO;
using UnityEngine;

public class Logging : MonoBehaviour
{
    string filename = "";

    private void Start()
    {
        string d = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData) + "/Eternity Studios/SCP Intrusion/Logs";
        Directory.CreateDirectory(d);
        filename = d + "/y" + DateTime.Now.Year.ToString() + "_m" + DateTime.Now.Month.ToString() + "_d" + DateTime.Now.Day.ToString() + "_hr" + DateTime.Now.Hour.ToString() + "_min" + DateTime.Now.Minute.ToString() + "_sec" + DateTime.Now.Second.ToString() + "_mil" + DateTime.Now.Millisecond.ToString() + "_session_log.txt";
        if (File.Exists(filename))
        {
            File.WriteAllText(filename, string.Empty);
        }
    }

    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (filename == "")
        {
            string d = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData) + "/Eternity Studios/SCP Intrusion/Logs";
            Directory.CreateDirectory(d);
            filename = d + "/y" + DateTime.Now.Year.ToString() + "_m" + DateTime.Now.Month.ToString() + "_d" + DateTime.Now.Day.ToString() + "_hr" + DateTime.Now.Hour.ToString() + "_min" + DateTime.Now.Minute.ToString() + "_sec" + DateTime.Now.Second.ToString() + "_mil" + DateTime.Now.Millisecond.ToString() + "_session_log.txt";
        }

        try
        {
            string content = $"[{DateTime.Now}] {logString}";
            File.AppendAllText(filename, logString + "\n");
        }
        catch { }
    }
}