using UnityEngine;

namespace Game.Player.Gunplay
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        [Header("Stats")]
        [Tooltip("Amount of damage the gun applies")]
        public int Damage = 40;

        [Tooltip("Multiplier of damage for headshots")]
        public float HeadMultiplier = 4f;
        [Tooltip("Multiplier of damage for limbshots")]
        public float LimbMultiplier = 0.5f;
        [Tooltip("Max range in meters for this gun")]
        public float Range = 60f;
        [Tooltip("Fire rate, the amount of rounds it can shoot in a minute (still applies to semi-auto weapons)")]
        public float RPM = 735f;
        [Tooltip("Weapon chargeup delay, basically a delay before shooting (in seconds), like revolvers")]
        public float ChargeupTime = 0f;
        [Range(0, 1)]
        [Tooltip("Amount of slow when getting hit by this gun, percentage based")]
        public float Tagging = 0.9f;

        [Tooltip("Amount of bullets it shoots per trigger")]
        public int BulletCount = 1;

        [Tooltip("Firing mode for the gun")]
        public FiringMode GunFiringMode;

        [Header("Recoil & Inaccuracy")]
        [Tooltip("Spread amount each shot")]
        public float Spread = 0.5f;
        [Tooltip("Multiplier, not actual spread")]
        public float MovementSpread = 5f;
        [Tooltip("Spread cap for regular shooting")]
        public float MaxSpread = 3f;
        [Tooltip("Spread cap for moving")]
        public float MaxMovementSpread = 18f;
        [Tooltip("Starting spread for regular shooting")]
        public float StartingSpread = 0.05f;
        [Tooltip("Camera degrees raised after shooting")]
        public float Recoil = 0.5f;
        [Tooltip("Camera degrees sway after entering the swaying phase")]
        public float HorizontalRecoil = 1f;
        [Tooltip("Max degrees of sway allowed (switches direction when reached)")]
        public float MaxHorizontal = 4f;
        [Tooltip("Recoil decay per second, this is x10 by default")]
        public float RecoilDecay = 1f;
        [Tooltip("Amount of displacement (bullets go over the crosshair) per shot after the displacement phase begins")]
        public float Displacement = 1f;
        [Tooltip("Displacement cap for shooting")]
        public float MaxDisplacement = 4f;

        [Tooltip("How many rounds must be shot before starting to sway left and right, displacement is applied at 25% of this value")]
        public int SwayAfterRound = 8;

        [Tooltip("Determines if the swaying starts going right first")]
        public bool SwayStartRight = false;

        [Header("Scopes")]
        [Tooltip("Determines whether or not you can scope in with this gun")]
        public bool HasScope;

        [Tooltip("Zoomed in FOV of player")]
        public float ScopeFOV = 67.5f;

        [Tooltip("The overlay for the scope")]
        public Sprite ScopeImage;

        [Header("Ammunition & Reload")]
        [Tooltip("Amount of seconds required for reloading")]
        public float ReloadTime = 3f;

        [Tooltip("Maximum amount of ammo you can hold in a clip/magazine")]
        public int MaxAmmo = 30;
        [Tooltip("Amount of ammo you have in reserve")]
        public int ReserveAmmo = 90;

        [Header("Other Settings")]
        [Tooltip("Layers this gun can hit")]
        public LayerMask HitLayers;

        [Tooltip("The slot this gun occupies")]
        public WeaponSlot GunSlot;

        [Tooltip("The time it takes in seconds to pull the gun out")]
        public float DrawTime = 1f;

        [Header("Visuals & Audio")]
        [Tooltip("The particle/VFX it instantiates at the hit point when hitting something")]
        public GameObject HitObject;
        [Tooltip("The decal it instantiates at the hit point when hitting something")]
        public GameObject HitDecal;

        [Tooltip("The amount that the camera jumps when shooting in degrees")]
        public float AimPunch = 1f;
        [Tooltip("The duration for the camera jump")]
        public float AimPunchDuration = 0.05f;

        [Tooltip("Chargeup sounds, a sound in this list will be randomly selected to play when shooting")]
        public AudioClip[] ChargeupSounds;
        [Tooltip("Hit sounds (not for players), a sound in this list will be randomly selected to play when shooting")]
        public AudioClip[] HitSounds;
        [Tooltip("Shooting sounds, a sound in this list will be randomly selected to play when shooting")]
        public AudioClip[] ShootSounds;

        [Tooltip("Max distance that the sound can be heard in meters")]
        public float SoundMaxDistance = 100f;
        [Tooltip("Loudness of the sound")]
        public float SoundVolume = 1f;

        [Tooltip("Priority of the sound (sounds with low priority will get lowered when higher priority sounds are playing)")]
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
}