using UnityEngine;
using UnityEditor;


public static class ObjectPlacer
{
    public static GameObject CreatePreviewPrimitiveObject(PrimitiveType selectedPrimitiveType)
    {
        GameObject previewObject = GameObject.CreatePrimitive(selectedPrimitiveType);
        SetObjectTransparency(previewObject, 0.5f);
        return previewObject;
    }

    public static GameObject CreatePreviewPrefabObject(GameObject selectedPrefab)
    {
        GameObject previewObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
        SetObjectTransparency(previewObject, 0.5f);
        return previewObject;
    }

    private static void SetObjectTransparency(GameObject previewObject, float transparency)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                material.SetFloat("_Mode", 3); // Transparent mode for Standard Shader
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

                Color color = material.color;
                color.a = transparency;
                material.color = color;
            }
        }
    }

    public static void PlaceObject(GameObject previewObject, PrimitiveType selectedPrimitiveType)
    {
        // Instantiate the final object at the preview object's position
        GameObject finalObject = GameObject.CreatePrimitive(selectedPrimitiveType);

        if (finalObject != null)
        {
            finalObject.transform.position = previewObject.transform.position;
            SetObjectTransparency(finalObject, 1f);
            Undo.RegisterCreatedObjectUndo(finalObject, "Place Object");
        }
    }

    public static void PlaceObject(GameObject previewObject, GameObject selectedPrefab)
    {
        GameObject finalObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);

        if (finalObject != null)
        {
            finalObject.transform.position = previewObject.transform.position;
            SetObjectTransparency(finalObject, 1f);
            Undo.RegisterCreatedObjectUndo(finalObject, "Place Object");
        }
    }
}
