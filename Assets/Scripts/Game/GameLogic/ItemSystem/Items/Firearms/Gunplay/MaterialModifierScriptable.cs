using UnityEngine;

[CreateAssetMenu(fileName = "New Material Modifier", menuName = "Material Modifier")]
public class MaterialModifierScriptable : ScriptableObject
{
    public GameObject DecalOverride;

    public float WallbangMultiplier = 1f;
}
