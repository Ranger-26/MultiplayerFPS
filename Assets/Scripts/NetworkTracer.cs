using UnityEngine;
using Mirror;

public class NetworkTracer : NetworkBehaviour
{
    LineRenderer ln;

    [SyncVar(hook = "UpdateTracerStart")]
    [HideInInspector]
    public Vector3 start;
    [SyncVar(hook = "UpdateTracerEnd")]
    [HideInInspector]
    public Vector3 end;

    private void Awake()
    {
        ln = GetComponent<LineRenderer>();
    }

    public void UpdateTracerStart(Vector3 oldValue, Vector3 newValue)
    {
        ln.SetPosition(0, newValue);
    }

    public void UpdateTracerEnd(Vector3 oldValue, Vector3 newValue)
    {
        ln.SetPosition(1, newValue);
    }
}
