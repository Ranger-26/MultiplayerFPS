using UnityEngine;
using TMPro;
using Mirror;

namespace Game.Player
{
    public class PlayerNameDisplay : NetworkBehaviour
    {
        Transform cam;

        NetworkGamePlayer player;

        [SerializeField]
        TMP_Text txt;

        private void Awake()
        {
            player = GetComponent<NetworkGamePlayer>();
        }

        private void Start()
        {
            cam = Camera.main.transform;

            if (hasAuthority)
                txt.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(player.playerName))
                txt.SetText(player.playerName);

            txt.transform.parent.LookAt(cam);
        }
    }
}