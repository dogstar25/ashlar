using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BrickColorPalette",
    menuName = "Ashlar/Brick Color Palette")]
public class BrickColorPalette : ScriptableObject
{
    [SerializeField]
    private Color[] colors =
    {
        new Color32(230,  57,  70, 255), // red
        new Color32(241,  91,  76, 255),
        new Color32(244, 140,   6, 255), // orange
        new Color32(249, 199,  79, 255), // yellow
        new Color32(255, 221,  89, 255),
        new Color32(144, 190, 109, 255), // light green
        new Color32( 67, 170, 139, 255), // teal green
        new Color32(  0, 168, 150, 255),
        new Color32( 77, 144, 142, 255),
        new Color32( 39, 125, 161, 255), // blue
        new Color32( 87, 117, 144, 255),
        new Color32( 67,  97, 238, 255),
        new Color32( 63,  55, 201, 255),
        new Color32(114,   9, 183, 255), // purple
        new Color32(181,  23, 158, 255),
        new Color32(247,  37, 133, 255), // pink
        new Color32(255, 143, 171, 255),
        new Color32(255, 173, 173, 255),
        new Color32(255, 214, 165, 255),
        new Color32(253, 255, 182, 255),
        new Color32(202, 255, 191, 255),
        new Color32(155, 246, 255, 255),
        new Color32(160, 196, 255, 255),
        new Color32(189, 178, 255, 255),
        new Color32(255, 198, 255, 255),
        new Color32(110,  57,  35, 255), // brown
        new Color32(166, 124,  82, 255),
        new Color32(221, 161,  94, 255),
        new Color32( 52,  78,  65, 255), // dark green
        new Color32( 53,  80, 112, 255), // slate blue
        new Color32(108, 117, 125, 255), // gray
        new Color32( 33,  37,  41, 255), // charcoal
    };

    public int ColorCount => colors.Length;

    public Color GetRandomColor()
    {
        if (colors == null || colors.Length == 0)
        {
            Debug.LogWarning("BrickColorPalette has no colors. Returning white.");
            return Color.white;
        }

        int index = Random.Range(0, colors.Length);
        return colors[index];
    }

    public void GetRandomDistinctColorQuad(
        out Color top,
        out Color right,
        out Color bottom,
        out Color left)
    {
        if (colors == null || colors.Length == 0)
        {
            Debug.LogWarning("BrickColorPalette has no colors. Returning white for all brick sides.");

            top = Color.white;
            right = Color.white;
            bottom = Color.white;
            left = Color.white;
            return;
        }

        if (colors.Length < 4)
        {
            Debug.LogWarning("BrickColorPalette needs at least 4 colors to create a brick with 4 different colors. Some colors may repeat.");

            top = GetRandomColor();
            right = GetRandomColor();
            bottom = GetRandomColor();
            left = GetRandomColor();
            return;
        }

        List<Color> availableColors = new List<Color>(colors);

        top = TakeRandomColor(availableColors);
        right = TakeRandomColor(availableColors);
        bottom = TakeRandomColor(availableColors);
        left = TakeRandomColor(availableColors);
    }

    private Color TakeRandomColor(List<Color> availableColors)
    {
        int index = Random.Range(0, availableColors.Count);
        Color selectedColor = availableColors[index];

        availableColors.RemoveAt(index);

        return selectedColor;
    }
}