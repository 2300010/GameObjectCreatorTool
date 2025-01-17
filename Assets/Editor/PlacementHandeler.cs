using UnityEditor;
using UnityEngine;

public class PlacementHandeler
{
    private static PlacementHandeler instance;
    public static PlacementHandeler Instance 
    {  
        get 
        {
            if (instance == null)
            {
                instance = new PlacementHandeler();
            }
            return instance; 
        } 
    }
    private PlacementHandeler() { }

    public static void PlaceObject(GameObject previewObject, PrimitiveType selectedPrimitiveType)
    {
        GameObject finalObject = GameObject.CreatePrimitive(selectedPrimitiveType);
        FinalizePlacement(previewObject, finalObject);
    }

    public static void PlaceObject(GameObject previewObject, GameObject selectedPrefab)
    {
        GameObject finalObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
        FinalizePlacement(previewObject, finalObject);
    }

    private static void FinalizePlacement(GameObject previewObject, GameObject finalObject)
    {
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
