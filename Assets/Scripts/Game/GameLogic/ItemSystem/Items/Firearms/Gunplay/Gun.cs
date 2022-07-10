using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Firearms.Gunplay
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
        [Tooltip("Amount of slow when getting hit by this gun")]
        public float Tagging = 0.9f;
        [Range(0, 1)]
        [Tooltip("Amount of speed reduction when equipped, percentage based")]
        public float Weight = 0.1f;

        [Tooltip("Amount of bullets it shoots per trigger")]
        public int BulletCount = 1;

        [Tooltip("Firing mode for the gun")]
        public FiringMode GunFiringMode;

        [Header("Recoil & Inaccuracy")]
        [Tooltip("Spread amount each shot")]
        public float Spread = 0.5f;
        [Tooltip("Multiplier, not actual spread")]
        public float MovementSpread = 5f;
        [Tooltip("Movement tolerance before applying movement spread")]
        public float MovementSpreadTolerance = 0.3f;
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
        [Tooltip("Recoil decay per second")]
        public float RecoilDecay = 1f;
        [Tooltip("Spread decay per second")]
        public float SpreadDecay = 2f;
        [Tooltip("Amount of displacement (bullets go over the crosshair) per shot after the displacement phase begins")]
        public float Displacement = 1f;
        [Tooltip("Displacement cap for shooting")]
        public float MaxDisplacement = 4f;
        [Tooltip("Multiplier applied to spread when crouching")]
        [Range(0f, 1f)]
        public float CrouchingMultiplier = 0.5f;

        [Tooltip("How many rounds must be shot before starting to sway left and right, displacement is applied at 25% of this value")]
        public int SwayAfterRecoil = 8;

        [Tooltip("Determines if the swaying starts going right first")]
        public bool SwayStartRight = false;

        [Header("Scopes")]
        [Tooltip("Determines whether or not you can scope in with this gun")]
        public bool HasScope;
        [Tooltip("Determines whether or not you exit the scope when you shoot")]
        public bool ExitScopeOnShoot;

        [Tooltip("Zoomed in FOV of player")]
        public float ScopeFOV = 67.5f;
        [Tooltip("Spread amount while scoped for each shot")]
        public float ScopedSpread = 0f;
        [Tooltip("Speed reduction when scoped in percentage, overrides weight")]
        [Range(0f, 1f)]
        public float ScopeSpeedReduction = 0.3f;

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

        [Tooltip("The time it takes in seconds to pull the gun out, as well as the time it takes to chamber the gun after reload")]
        public float DrawTime = 1f;

        [Header("Visuals & Audio")]
        [Tooltip("The particle/VFX it instantiates at the hit point when hitting something")]
        public GameObject HitObject;
        [Tooltip("The decal it instantiates at the hit point when hitting something")]
        public GameObject HitDecal;
        [Tooltip("The decal it instantiates at the hit point when hitting something")]
        public GameObject Tracer;

        [Tooltip("The amount that the view model moves back when spread is increasing")]
        public float MaxBacking = 0.1f;
        [Tooltip("The multiplier for the backing speed")]
        public float BackingMultiplier = 3f;

        [Tooltip("Chargeup sounds, a sound in this list will be randomly selected to play when shooting")]
        public AudioClip[] ChargeupSounds;
        [Tooltip("Shooting sounds, a sound in this list will be randomly selected to play when shooting")]
        public AudioClip[] ShootSounds;

        [Tooltip("Max distance that the sound can be heard in meters")]
        public float SoundMaxDistance = 100f;
        [Tooltip("Loudness of the sound")]
        public float SoundVolume = 1f;

        [Tooltip("Priority of the sound (sounds with low priority will get lowered when higher priority sounds are playing)")]
        public int SoundPriority = 128;

        [Tooltip("Unique id of the gun.")]
        public GunIDs UniqueGunID = GunIDs.Debug;
    }

    public enum FiringMode : byte
    {
        Auto,
        SemiAuto
    }

    public enum WeaponSlot : byte
    {
        Primary,
        Secondary,
        Melee
    }

    public enum GunIDs : byte
    {
        None,
        [Tooltip("Debug Gun")]
        Debug,
        [Tooltip("Melee")]
        Knife, 

        // T Pistols

        [Tooltip("Basic Free Pistol")]
        Makarov, 
        [Tooltip("Better Pistol (One tap headshot with no armor)")]
        TT33, 

        // T SMGs

        [Tooltip("Eco SMG")]
        Bizon,
        [Tooltip("Half Buy SMG")]
        Vityaz,

        // T Rifles

        [Tooltip("Half Buy Rifle")]
        VSS, 
        [Tooltip("Full Buy Rifle (One tap headshot)")]
        AN94, 

        // T Shotguns

        [Tooltip("Auto Shotty")]
        Saiga12, 



        // CT Pistols

        [Tooltip("Basic Free Pistol")]
        HK45CT, 
        [Tooltip("Better Pistol (One tap headshot with no armor)")]
        M1911,

        // CT SMGs

        [Tooltip("Eco SMG")]
        MP5K,
        [Tooltip("Half Buy SMG")]
        MP7, 

        // CT Rifles

        [Tooltip("Half Buy Rifle")]
        G36C, 
        [Tooltip("Full Buy Rifle")]
        HK416, 

        // CT Shotguns

        [Tooltip("Auto Shotty")]
        AA12, 

        // Other

        [Tooltip("Eco Shotgun")]
        Mossberg,
        [Tooltip("Eco Sniper")]
        M2010,
        [Tooltip("AWP (One hit kill body and head)")]
        Intervention,
        [Tooltip("ONE TRICK DEAG (One tap headshot)")]
        Deagle,
        [Tooltip("Auto Sniper")]
        FNFAL,
        [Tooltip("Chargeup Sniper, One Shot Anywhere")]
        Gauss,
        [Tooltip("Light Machine Gun")]
        Ultimax, 
        [Tooltip("Heavy Machine Gun")]
        M60 
    }
}