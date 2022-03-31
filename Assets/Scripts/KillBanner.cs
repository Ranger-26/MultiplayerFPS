using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillBanner : MonoBehaviour
{
    public AudioClip KillAudio;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Trigger(bool head)
    {
        if (head)
        {
            anim.Play(StringKeys.KillBannerHeadAnimation, -1, 0f);
        }
        else
        {
            anim.Play(StringKeys.KillBannerAnimation, -1, 0f);
        }
    }

    private void Update()
    {
        // Debug

        if (Input.GetKeyDown(KeyCode.K))
        {
            Trigger(false);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Trigger(true);
        }
    }
}
