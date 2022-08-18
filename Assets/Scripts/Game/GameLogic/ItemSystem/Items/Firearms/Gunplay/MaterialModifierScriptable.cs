using UnityEngine;

[CreateAssetMenu(fileName = "New Material Modifier", menuName = "Material Modifier")]
public class MaterialModifierScriptable : ScriptableObject
{
    public GameObject DecalOverride; // Handle this sometime, idk, different bullet holes on different materials

    public float WallbangMultiplier = 1f;
}
