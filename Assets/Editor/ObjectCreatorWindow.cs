using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPlacer))]
public class ObjectCreatorWindow : EditorWindow
{
    private string objectName = "New Object";
    private float objectScale = 1f;
    private Vector3 objectPosition = Vector3.zero;

    private enum ObjectType { Primitive, Prefab }
    private ObjectType selectedObjectType = ObjectType.Primitive;

    private PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;

    private GameObject selectedPrefab;
    private bool showObjectSettings = false;

    [MenuItem("Tools/Object Creator")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreatorWindow>("Object Creator");
    }

    private void OnGUI()
    {
        ShowObjectParametersUI();

        SelectGameObjectType();

        InstantiateSelectedGameObject();

    }

    private void ShowObjectParametersUI()
    {
        showObjectSettings = EditorGUILayout.Foldout(showObjectSettings, "Object Settings", true);

        if (showObjectSettings)
        {
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

            objectName = EditorGUILayout.TextField("Object Name", objectName);
            objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.1f, 10f);
            objectPosition = EditorGUILayout.Vector3Field("Object Position", objectPosition);
        }
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

                if (GUILayout.Button("Instantiate Primitive"))
                {
                    instance = GameObject.CreatePrimitive(selectedPrimitiveType);
                    SetGameObjectParameters(instance);

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Primitive");
                }

                if (GUILayout.Button("Place Primitive Manually"))
                {

                }
                break;

            case ObjectType.Prefab:
                
                selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

                if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
                {
                    if (GUILayout.Button("Instantiate Prefab"))
                    {
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        SetGameObjectParameters(instance);

                        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
                    }

                    if (GUILayout.Button("Place Prefab Manually"))
                    {

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
