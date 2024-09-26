using UnityEditor;
using UnityEngine;

public class ObjectCreatorWindow : EditorWindow
{
    private string objectName = "New Object";
    private float objectScale = 1f;
    private Vector3 objectPosition = Vector3.zero;

    private enum ObjectType { Primitive, Prefab }
    private ObjectType selectedObjectType = ObjectType.Primitive;

    private PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;

    private GameObject selectedPrefab;

    [MenuItem("Tools/Object Creator")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreatorWindow>("Object Creator");
    }

    private void OnGUI()
    {

        EditorGUILayout.LabelField("New Object Parameters", EditorStyles.boldLabel);

        objectName = EditorGUILayout.TextField("Object Name", objectName);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.1f, 10f);
        objectPosition = EditorGUILayout.Vector3Field("Object Position", objectPosition);

        SelectGameObjectType();

        //EditorGUILayout.LabelField("Select Prefab", EditorStyles.boldLabel);
        //selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

        InstantiateSelectedGameObject();

    }


    private void SelectGameObjectType()
    {
        GUILayout.Label("Select Object Type", EditorStyles.boldLabel);

        selectedObjectType = (ObjectType)EditorGUILayout.EnumPopup("Object Type", selectedObjectType);
    }

    private void InstantiateSelectedGameObject()
    {
        GameObject instance;

        switch (selectedObjectType)
        {
            case ObjectType.Primitive:

                selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Type", selectedPrimitiveType);

                // Button to instantiate the selected primitive
                if (GUILayout.Button("Instantiate Primitive"))
                {
                    instance = GameObject.CreatePrimitive(selectedPrimitiveType);
                    SetGameObjectParameters(instance);

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Primitive");
                }
                break;

            case ObjectType.Prefab:
                // Show prefab selection field
                selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

                // Check if a valid prefab is selected
                if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
                {
                    if (GUILayout.Button("Instantiate Prefab"))
                    {
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        SetGameObjectParameters(instance);

                        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a valid prefab.", MessageType.Warning);
                }
                break;
        }
    }

    private void SetGameObjectParameters(GameObject instance)
    {
        instance.name = objectName;
        instance.transform.localScale = Vector3.one * objectScale;
        instance.transform.localPosition = objectPosition;
    }

}
