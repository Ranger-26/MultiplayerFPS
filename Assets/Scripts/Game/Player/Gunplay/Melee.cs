using UnityEngine;
using Mirror;
using System.Collections;
using Game.Player.Movement;
using Game.Player.Gunplay.IdentifierComponents;
using UnityEngine.Rendering.HighDefinition;
using Game.Player.Gunplay;

public class Melee : MonoBehaviour
{
    public static float Range = 2f;
    public static float Tagging = 1f;
    public static float Damage = 40f;
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

        if (Input.GetMouseButtonDown(0))
        {
            LightAttack();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            HeavyAttack();
        }
    }

    public void LightAttack()
    {
        if (delay) return;

        if (anim != null)
            anim.Play(StringKeys.MeleeLightAnimation, -1, 0f);
    }

    public void HeavyAttack()
    {
        if (delay) return;

        if (anim != null)
            anim.Play(StringKeys.MeleeHeavyAnimation, -1, 0f);
    }

    public void Light()
    {
        nsm.CmdMelee(spreadPoint.position, spreadPoint.forward, 1f);
    }

    public void Heavy()
    {
        nsm.CmdMelee(spreadPoint.position, spreadPoint.forward, 2f);
    }
}
