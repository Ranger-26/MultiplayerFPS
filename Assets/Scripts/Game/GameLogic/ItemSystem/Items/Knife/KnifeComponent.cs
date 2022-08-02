using System.Collections;
using Game.GameLogic.ItemSystem.Items.Firearms.Gunplay;
using Game.GameLogic.ItemSystem.Items.Firearms.Gunplay.IdentifierComponents;
using Game.Player.Movement;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

namespace Game.GameLogic.ItemSystem.Items.Knife
{
    public class KnifeComponent : MonoBehaviour
    {
        public static float DrawTime = 0.5f;
        public static LayerMask HitLayers = 6;

        public GameObject HitObject;
        public GameObject HitDecal;

        NetworkShootingManager nsm;

        NetworkIdentity ni;

        GameObject model;

        Transform firingPoint;
        Transform spreadPoint;

        Camera cam;

        PlayerMovement PM;
        PlayerLook PL;
        PlayerCrouch PC;

        Animator anim;

        float attackTimer;

        bool delay;

        private void Awake()
        {
            PM = GetComponentInParent<PlayerMovement>();
            PL = GetComponentInParent<PlayerLook>();
            PC = GetComponentInParent<PlayerCrouch>();

            cam = PL.cam;
            model = transform.GetChild(0).gameObject;
            anim = GetComponentInChildren<Animator>();
            anim.keepAnimatorControllerStateOnDisable = true;

            firingPoint = cam.GetComponentInChildren<FiringPoint>().transform;
            spreadPoint = cam.GetComponentInChildren<SpreadPoint>().transform;
        }

        private void Start()
        {
            ni = GetComponentInParent<NetworkIdentity>();

            if (!ni.isLocalPlayer)
            {
                Transform tempcam = GetComponentInParent<Camera>().transform;
                Destroy(tempcam.GetComponentInChildren<FiringPoint>().gameObject);
                tempcam.GetComponent<HDAdditionalCameraData>().enabled = false;
                tempcam.GetComponent<Camera>().enabled = false;
                tempcam.GetComponent<AudioListener>().enabled = false;
                enabled = false;
            }

            nsm = GetComponentInParent<NetworkShootingManager>();

            if (PM == null) { Debug.LogError("Player movement is null!"); }
            if (PL == null) { Debug.LogError("Player look is null!"); }

            if (PM != null) PM.weight = 0f;

            if (nsm == null) { Debug.LogError("Network Shooting Manager is null in the start!"); }

            StartCoroutine(Draw());
        }

        private void OnEnable()
        {
            if (PM != null)
                PM.weight = 0f;

            StartCoroutine(Draw());
        }

        private IEnumerator Draw()
        {
            delay = true;

            if (anim != null)
                anim.Play(StringKeys.GunDrawAnimation, -1, 0f);

            yield return new WaitForSeconds(DrawTime);

            if (anim != null)
                anim.Play(StringKeys.GunIdleAnimation, -1, 0f);

            delay = false;
        }

        private void Update()
        {
            if (nsm == null)
            {
                nsm = GetComponentInParent<NetworkShootingManager>();
                return;
            }

            if (!nsm.hasAuthority) enabled = false;

            if (attackTimer > 0f)
                attackTimer -= Time.deltaTime;
        }

        public void LightAttack(InputAction.CallbackContext ctx)
        {
            if (delay || attackTimer > 0f) return;

            attackTimer = 0.75f;

            if (anim != null)
            {
               anim.SetTrigger(StringKeys.MeleeLightAnimation);
            }

            NetworkClient.Send(new KnifeStrikeMessage()
            {
                Start = spreadPoint.position,
                forward = spreadPoint.forward
            });
        }

        public void HeavyAttack(InputAction.CallbackContext ctx)
        {
            if (delay || attackTimer > 0f) return;

            attackTimer = 1.5f;

            if (anim != null)
            {
                anim.SetTrigger(StringKeys.MeleeHeavyAnimation);
            }

            NetworkClient.Send(new KnifeStrikeMessage()
            {
                Start = spreadPoint.position,
                forward = spreadPoint.forward
            });
        }
    }
}