using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 벽 투명화 재질 관리
/// </summary>
public class MaterialApplier
{
    private Material transparentMaterial;
    private List<Renderer> currentTransparentWalls = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterialsMap = new Dictionary<Renderer, Material[]>();

    public MaterialApplier(Material transparentMaterial)
    {
        this.transparentMaterial = transparentMaterial;
    }

    public void ApplyTransparency(Collider[] colliders, List<GameObject> excludedObjects)
    {
        RestoreOriginalMaterials();

        foreach (Collider col in colliders)
        {
            if (excludedObjects.Contains(col.gameObject)) continue;

            Renderer rend = col.GetComponent<Renderer>();
            if (rend == null) continue;

            if (!originalMaterialsMap.ContainsKey(rend))
                originalMaterialsMap[rend] = rend.materials;

            Material[] newMaterials = new Material[rend.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
                newMaterials[i] = transparentMaterial;

            rend.materials = newMaterials;
            currentTransparentWalls.Add(rend);
        }
    }

    public void RestoreOriginalMaterials()
    {
        foreach (Renderer rend in currentTransparentWalls)
        {
            if (rend && originalMaterialsMap.ContainsKey(rend))
                rend.materials = originalMaterialsMap[rend];
        }

        currentTransparentWalls.Clear();
    }
}