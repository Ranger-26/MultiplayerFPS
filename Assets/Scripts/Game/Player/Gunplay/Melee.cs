using UnityEngine;

public class Melee : MonoBehaviour
{
    public static float Range = 2f;
    public static float Tagging = 1f;
    public static float Damage = 40f;
    public static LayerMask HitLayers = LayerMask.NameToLayer("BodyPart");

    public GameObject HitObject;
    public GameObject HitDecal;


}
