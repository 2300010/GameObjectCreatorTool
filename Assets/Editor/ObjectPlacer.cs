using UnityEditor;
using UnityEngine;

public class ObjectPlacer
{
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
        //Debug.Log("Transparency = " + transparency);
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

    public static void PlaceObject(GameObject previewObject, PrimitiveType selectedPrimitiveType)
    {
        GameObject finalObject = GameObject.CreatePrimitive(selectedPrimitiveType);

        if (finalObject != null)
        {
            finalObject.name = previewObject.name;
            finalObject.transform.position = previewObject.transform.position;
            finalObject.transform.localScale = previewObject.transform.localScale;
            Selection.activeObject = finalObject;
            Undo.RegisterCreatedObjectUndo(finalObject, "Place Object");
        }
    }

    public static void PlaceObject(GameObject previewObject, GameObject selectedPrefab)
    {
        GameObject finalObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);

        if (finalObject != null)
        {
            finalObject.name = previewObject.name;
            finalObject.transform.position = previewObject.transform.position;
            finalObject.transform.localScale = previewObject.transform.localScale;
            Selection.activeObject = finalObject;
            Undo.RegisterCreatedObjectUndo(finalObject, "Place Object");
        }
    }
}
