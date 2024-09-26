using UnityEditor;
using UnityEngine;

public class ObjectCreatorWindow : EditorWindow
{
    private string objectName = "New Object";
    private float objectScale = 1f;

    private GameObject selectedPrefab;

    [MenuItem("Tools/Object Creator")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreatorWindow>("Object Creator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Create New Object", EditorStyles.boldLabel);

        objectName = EditorGUILayout.TextField("Object Name", objectName);

        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.1f, 10f);



        if (GUILayout.Button("Create Cube"))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = objectName;
            cube.transform.localScale = Vector3.one * objectScale;
        }

        if (GUILayout.Button("Create Sphere"))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = objectName;
            sphere.transform.localScale = Vector3.one * objectScale;
        }

        EditorGUILayout.LabelField("Select Prefab", EditorStyles.boldLabel);
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

        if (selectedPrefab != null && PrefabUtility.IsPartOfAnyPrefab(selectedPrefab))
        {
            if (GUILayout.Button("Instantiate Selected Prefab"))
            {
                InstantiatePrefab();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a valid prefab.", MessageType.Warning);
        }
    }

    private void InstantiatePrefab()
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
        instance.name = objectName;
        instance.transform.localScale = Vector3.one * objectScale;

        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
    }
}
