using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer triangleARenderer;
    [SerializeField] private Renderer triangleBRenderer;

    [Header("Colors")]
    [SerializeField] private BrickColorPalette colorPalette;
    [SerializeField] private bool randomizeColorsOnStart = true;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (randomizeColorsOnStart)
        {
            RandomizeColors();
        }
    }

    [ContextMenu("Randomize Colors")]
    public void RandomizeColors()
    {
        EnsurePropertyBlockExists();

        if (colorPalette == null)
        {
            Debug.LogWarning($"{name} has no BrickColorPalette assigned.");
            return;
        }

        colorPalette.GetRandomColorPair(out Color colorA, out Color colorB);
        SetColors(colorA, colorB);
    }

    public void SetColors(Color colorA, Color colorB)
    {
        EnsurePropertyBlockExists();

        ApplyColor(triangleARenderer, colorA);
        ApplyColor(triangleBRenderer, colorB);
    }

    private void ApplyColor(Renderer targetRenderer, Color color)
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name} has a missing triangle renderer reference.");
            return;
        }

        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_BaseColor", color);
        propertyBlock.SetColor("_Color", color);

        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void EnsurePropertyBlockExists()
    {
        propertyBlock ??= new MaterialPropertyBlock();
    }
}