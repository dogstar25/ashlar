using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer triangleARenderer;
    [SerializeField] private Renderer triangleBRenderer;

    [Header("Materials")]
    [SerializeField] private BrickMaterialPalette materialPalette;
    [SerializeField] private bool randomizeMaterialsOnStart = true;

    [Header("Current Triangle Materials")]
    [SerializeField] private BrickMaterialDefinition triangleAMaterial;
    [SerializeField] private BrickMaterialDefinition triangleBMaterial;

    private MaterialPropertyBlock propertyBlock;

    public BrickMaterialDefinition TriangleAMaterial => triangleAMaterial;
    public BrickMaterialDefinition TriangleBMaterial => triangleBMaterial;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (randomizeMaterialsOnStart)
        {
            RandomizeMaterials();
        }
        else
        {
            ApplyCurrentMaterials();
        }
    }

    [ContextMenu("Randomize Materials")]
    public void RandomizeMaterials()
    {
        EnsurePropertyBlockExists();

        if (materialPalette == null)
        {
            Debug.LogWarning($"{name} has no BrickMaterialPalette assigned.");
            return;
        }

        materialPalette.GetRandomMaterialPair(
            out BrickMaterialDefinition materialA,
            out BrickMaterialDefinition materialB);

        SetMaterials(materialA, materialB);
    }

    [ContextMenu("Apply Current Materials")]
    public void ApplyCurrentMaterials()
    {
        EnsurePropertyBlockExists();

        ApplyMaterial(triangleARenderer, triangleAMaterial, "Triangle A Renderer");
        ApplyMaterial(triangleBRenderer, triangleBMaterial, "Triangle B Renderer");
    }

    public void SetMaterials(
        BrickMaterialDefinition materialA,
        BrickMaterialDefinition materialB)
    {
        triangleAMaterial = materialA;
        triangleBMaterial = materialB;

        ApplyCurrentMaterials();
    }

    public void SwapTriangleMaterials()
    {
        SetMaterials(triangleBMaterial, triangleAMaterial);
    }

    private void ApplyMaterial(
        Renderer targetRenderer,
        BrickMaterialDefinition brickMaterial,
        string rendererName)
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name} has a missing {rendererName} reference.");
            return;
        }

        if (brickMaterial == null)
        {
            Debug.LogWarning($"{name} has no material assigned for {rendererName}.");
            return;
        }

        if (brickMaterial.HasVisualMaterial)
        {
            targetRenderer.sharedMaterial = brickMaterial.VisualMaterial;
        }

        ApplyMaterialColor(targetRenderer, brickMaterial.BaseColor);
    }

    private void ApplyMaterialColor(Renderer targetRenderer, Color color)
    {
        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_BaseColor", color);
        propertyBlock.SetColor("_Color", color);
        propertyBlock.SetColor("_TintColor", color);
        propertyBlock.SetColor("_EmissionColor", color);

        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void EnsurePropertyBlockExists()
    {
        propertyBlock ??= new MaterialPropertyBlock();
    }
}