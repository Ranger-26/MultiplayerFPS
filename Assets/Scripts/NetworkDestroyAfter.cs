using Mirror;
using UnityEngine;
using System.Collections;

public class NetworkDestroyAfter : NetworkBehaviour
{
    public float Timer = 1f;

    private void Start()
    {
        StartCoroutine(Des());
    }
    

    IEnumerator Des()
    {
        yield return new WaitForSeconds(Timer);
        Destroy(gameObject);
    }
}
