using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    [Header("Stats")]
    public float Damage = 40f;
    public float Range = 60f;
    public float RPS = 9.5f;

    public int BulletCount = 1;
    
    public FiringMode GunFiringMode;

    [Header("Recoil & Inaccuracy")]
    public float Spread = 0.5f;
    public float MovementSpread = 5f;
    public float MaxSpread = 3f;
    public float MaxMovementSpread = 18f;
    public float StartingSpread = 0.05f;
    public float Recoil = 0.5f;
    public float HorizontalRecoil = 1f;
    public float MaxHorizontal = 4f;
    public float MovementMultiplier = 1f;
    public float ShootingMultiplier = 1f;
    public float RecoilDecay = 1f;

    public int SwayAfterRound = 8;

    public bool SwayStartRight = false;

    [Header("Scopes")]
    public bool HasScope;

    public float ScopeFOV = 67.5f;

    public Sprite ScopeImage;

    [Header("Ammunition & Reload")]
    public float ReloadTime = 3f;

    public int MaxAmmo = 30;
    public int ReserveAmmo = 90;

    [Header("Other Settings")]
    public LayerMask HitLayers;

    public GameObject DroppedForm;
    public GameObject ViewModel;

    public WeaponSlot GunSlot;

    public float DrawTime = 1f;

    [Header("Visuals & Audio")]
    public GameObject HitObject;
    public GameObject HitDecal;
    public GameObject Tracer;

    public float NoTracerRange = 10f;
    public float TracerPercentage = 0.75f;
    public float AimPunch = 1f;
    public float AimPunchDuration = 0.05f;

    public AudioClip[] ShootSounds;

    public float SoundMaxDistance = 100f;
    public float SoundVolume = 1f;

    public int SoundPriority = 128;
}

public enum FiringMode
{
    Auto,
    SemiAuto
}

public enum WeaponSlot
{
    Primary,
    Secondary
}
