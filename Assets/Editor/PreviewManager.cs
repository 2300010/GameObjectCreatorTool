using UnityEditor;
using UnityEngine;

public class PreviewManager
{
    private static PreviewManager instance;
    public static PreviewManager Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new PreviewManager();
            }
            return instance; 
        }
    }
    private PreviewManager() { }

    public GameObject CreatePreviewPrimitiveObject(PrimitiveType selectedPrimitiveType)
    {
        GameObject previewObject = GameObject.CreatePrimitive(selectedPrimitiveType);
        SetObjectToTransparent(previewObject, 0.5f);
        return previewObject;
    }

    public GameObject CreatePreviewPrefabObject(GameObject selectedPrefab)
    {
        GameObject previewObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
        SetObjectToTransparent(previewObject, 0.5f);
        return previewObject;
    }

    public void SetObjectToTransparent(GameObject previewObject, float transparency)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] originalMaterials = renderer.sharedMaterials;
            Material[] newMaterials = new Material[originalMaterials.Length];

            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                Material clonedMaterial = new Material(renderer.sharedMaterials[i]);

                clonedMaterial.SetFloat("_Mode", 3); // Transparent mode for Standard Shader (3)
                clonedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                clonedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                clonedMaterial.SetInt("_ZWrite", 0);
                clonedMaterial.DisableKeyword("_ALPHATEST_ON");
                clonedMaterial.EnableKeyword("_ALPHABLEND_ON");
                clonedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                clonedMaterial.renderQueue = 3000;

                Color color = clonedMaterial.color;
                color.a = transparency;
                clonedMaterial.color = color;

                newMaterials[i] = clonedMaterial;
            }
            renderer.materials = newMaterials;
        }
    }

}
