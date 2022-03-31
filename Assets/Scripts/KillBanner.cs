using UnityEngine;

public class KillBanner : MonoBehaviour
{
    public AudioClip KillAudio;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Trigger()
    {
        anim.Play(StringKeys.KillBannerAnimation, -1, 0f);
    }

    private void Update()
    {
        // Debug

        if (Input.GetKeyDown(KeyCode.K))
        {
            Trigger();
        }
    }
}
