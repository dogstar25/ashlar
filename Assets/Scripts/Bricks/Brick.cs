using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer triangleARenderer;
    [SerializeField] private Renderer triangleBRenderer;

    [Header("Colors")]
    [SerializeField] private BrickColorPalette colorPalette;
    [SerializeField] private bool randomizeColorsOnStart = true;

    [Header("Current Triangle Colors")]
    [SerializeField] private Color triangleAColor = Color.white;
    [SerializeField] private Color triangleBColor = Color.white;

    private MaterialPropertyBlock propertyBlock;

    public Color TriangleAColor => triangleAColor;
    public Color TriangleBColor => triangleBColor;

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
        else
        {
            ApplyCurrentColors();
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

    [ContextMenu("Apply Current Colors")]
    public void ApplyCurrentColors()
    {
        EnsurePropertyBlockExists();

        ApplyColor(triangleARenderer, triangleAColor, "Triangle A Renderer");
        ApplyColor(triangleBRenderer, triangleBColor, "Triangle B Renderer");
    }

    public void SetColors(Color colorA, Color colorB)
    {
        triangleAColor = colorA;
        triangleBColor = colorB;

        ApplyCurrentColors();
    }

    public void SwapTriangleColors()
    {
        SetColors(triangleBColor, triangleAColor);
    }

    // Kept for compatibility in case anything still calls the old rotation-color methods.
    public void RotateColorsClockwise()
    {
        SwapTriangleColors();
    }

    // Kept for compatibility in case anything still calls the old rotation-color methods.
    public void RotateColorsCounterClockwise()
    {
        SwapTriangleColors();
    }

    private void ApplyColor(Renderer targetRenderer, Color color, string rendererName)
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"{name} has a missing {rendererName} reference.");
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