using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(VideoPlayer))]
public class uGUI_MenuBackground : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] RenderTexture texture;
    [SerializeField] VideoPlayer player;

    void Awake()
    {
        player = this.GetComponent<VideoPlayer>();
        image = this.GetComponent<RawImage>();

        texture = new RenderTexture(Screen.width, Screen.height, 8);
        texture.name = "menuVideo";
        player.targetTexture = texture;
        image.texture = texture;
    }
    private void Start()
    {
        player.Play();
    }
}
