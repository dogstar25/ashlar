using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Renderer triangleTopRenderer;
    [SerializeField] private Renderer triangleRightRenderer;
    [SerializeField] private Renderer triangleBottomRenderer;
    [SerializeField] private Renderer triangleLeftRenderer;

    [Header("Colors")]
    [SerializeField] private BrickColorPalette colorPalette;
    [SerializeField] private bool randomizeColorsOnStart = true;

    [Header("Current Side Colors")]
    [SerializeField] private Color topColor = Color.white;
    [SerializeField] private Color rightColor = Color.white;
    [SerializeField] private Color bottomColor = Color.white;
    [SerializeField] private Color leftColor = Color.white;

    private MaterialPropertyBlock propertyBlock;

    public Color TopColor => topColor;
    public Color RightColor => rightColor;
    public Color BottomColor => bottomColor;
    public Color LeftColor => leftColor;

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

        colorPalette.GetRandomDistinctColorQuad(
            out Color top,
            out Color right,
            out Color bottom,
            out Color left);

        SetColors(top, right, bottom, left);
    }

    [ContextMenu("Apply Current Colors")]
    public void ApplyCurrentColors()
    {
        EnsurePropertyBlockExists();

        ApplyColor(triangleTopRenderer, topColor, "Triangle Top Renderer");
        ApplyColor(triangleRightRenderer, rightColor, "Triangle Right Renderer");
        ApplyColor(triangleBottomRenderer, bottomColor, "Triangle Bottom Renderer");
        ApplyColor(triangleLeftRenderer, leftColor, "Triangle Left Renderer");
    }

    public void SetColors(Color top, Color right, Color bottom, Color left)
    {
        topColor = top;
        rightColor = right;
        bottomColor = bottom;
        leftColor = left;

        ApplyCurrentColors();
    }

    public void RotateColorsClockwise()
    {
        Color oldTop = topColor;
        Color oldRight = rightColor;
        Color oldBottom = bottomColor;
        Color oldLeft = leftColor;

        SetColors(
            oldLeft,
            oldTop,
            oldRight,
            oldBottom);
    }

    public void RotateColorsCounterClockwise()
    {
        Color oldTop = topColor;
        Color oldRight = rightColor;
        Color oldBottom = bottomColor;
        Color oldLeft = leftColor;

        SetColors(
            oldRight,
            oldBottom,
            oldLeft,
            oldTop);
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