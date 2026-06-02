using UnityEngine;

public enum BrickMaterialTreatment
{
    Normal,
    Shimmering,
    Crystal,
    Lava,
    Metallic,
    Stone,
    Earth
}

[CreateAssetMenu(
    fileName = "BrickMaterialDefinition",
    menuName = "Ashlar/Brick Material Definition")]
public class BrickMaterialDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string materialId = "new-material";
    [SerializeField] private string displayName = "New Material";

    [Header("Color")]
    [SerializeField] private Color baseColor = Color.white;

    [Header("Treatment")]
    [SerializeField] private BrickMaterialTreatment treatment = BrickMaterialTreatment.Normal;

    [Header("Visual Material")]
    [SerializeField] private Material visualMaterial;

    [Header("Generation")]
    [SerializeField] private int rarityWeight = 100;

    public string MaterialId => materialId;
    public string DisplayName => displayName;
    public Color BaseColor => baseColor;
    public BrickMaterialTreatment Treatment => treatment;
    public Material VisualMaterial => visualMaterial;
    public int RarityWeight => rarityWeight;

    public bool HasVisualMaterial => visualMaterial != null;
}