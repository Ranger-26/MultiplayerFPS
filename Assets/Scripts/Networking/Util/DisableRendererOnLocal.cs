using UnityEngine;
using Mirror;

public class DisableRendererOnLocal : NetworkBehaviour
{
    public MeshRenderer[] Renderers;

    private void Start()
    {
        if (hasAuthority)
        {
            foreach (MeshRenderer rend in Renderers)
            {
                rend.enabled = false;
            }
        }
        else
        {
            enabled = false;
        }
    }
}
