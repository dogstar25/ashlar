using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BrickMaterialPalette",
    menuName = "Ashlar/Brick Material Palette")]
public class BrickMaterialPalette : ScriptableObject
{
    [SerializeField] private BrickMaterialDefinition[] materials;

    public int MaterialCount => materials == null ? 0 : materials.Length;

    public BrickMaterialDefinition GetRandomMaterial()
    {
        if (materials == null || materials.Length == 0)
        {
            Debug.LogWarning("BrickMaterialPalette has no materials.");
            return null;
        }

        int totalWeight = GetTotalWeight();

        if (totalWeight <= 0)
        {
            Debug.LogWarning("BrickMaterialPalette has no materials with positive rarity weight.");
            return materials[Random.Range(0, materials.Length)];
        }

        int randomWeight = Random.Range(0, totalWeight);
        int runningWeight = 0;

        foreach (BrickMaterialDefinition material in materials)
        {
            if (material == null || material.RarityWeight <= 0)
            {
                continue;
            }

            runningWeight += material.RarityWeight;

            if (randomWeight < runningWeight)
            {
                return material;
            }
        }

        return materials[materials.Length - 1];
    }

    public void GetRandomMaterialPair(
        out BrickMaterialDefinition materialA,
        out BrickMaterialDefinition materialB)
    {
        materialA = GetRandomMaterial();
        materialB = GetRandomMaterial();

        if (materials == null || materials.Length < 2)
        {
            return;
        }

        int safetyCounter = 0;

        while (materialB == materialA && safetyCounter < 10)
        {
            materialB = GetRandomMaterial();
            safetyCounter++;
        }
    }

    private int GetTotalWeight()
    {
        if (materials == null)
        {
            return 0;
        }

        int totalWeight = 0;

        foreach (BrickMaterialDefinition material in materials)
        {
            if (material == null || material.RarityWeight <= 0)
            {
                continue;
            }

            totalWeight += material.RarityWeight;
        }

        return totalWeight;
    }
}