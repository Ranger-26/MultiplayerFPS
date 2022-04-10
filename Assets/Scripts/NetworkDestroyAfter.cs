using Mirror;
using UnityEngine;
using System.Collections;

public class NetworkDestroyAfter : NetworkBehaviour
{
    public float Timer = 1f;

    AudioSource au;

    bool hasAudio;

    private void Awake()
    {
        au = GetComponent<AudioSource>();

        if (au != null)
        {
            hasAudio = true;
        }
    }

    private void Start()
    {
        StartCoroutine(Des());
    }
    

    IEnumerator Des()
    {
        yield return new WaitForSeconds(Timer);
        if (hasAudio)
        {
            StartCoroutine(AudioFadeOut.FadeOut(au, 0.1f));
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
