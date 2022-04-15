using UnityEngine;
using Mirror;

public class ViewModel : MonoBehaviour
{
    NetworkIdentity ni;

    private void Start()
    {
        ni = GetComponentInParent<NetworkIdentity>();

        if (ni.hasAuthority)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.gameObject.layer = 7;
            }
        }
    }
}
