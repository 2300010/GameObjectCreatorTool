using UnityEditor;
using UnityEngine;

public enum TypeOfObject { Primitive, Prefab }

//[CustomEditor(typeof(ObjectPlacer))]
public class ObjectCreatorWindow : EditorWindow
{
    ObjectPlacer objectPlacer = new ObjectPlacer();

    private string objectName = "New Object";
    private float objectScale = 1f;
    private Vector3 objectPosition = Vector3.zero;

    private TypeOfObject selectedObjectType = TypeOfObject.Primitive;

    private PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;

    private GameObject selectedPrefab;
    private GameObject previewObject;

    private bool showObjectSettings = true;
    private bool isPlacingObject = false;

    private float objectHeight = 0f;

    Event currentEvent;
    private LayerMask previewLayerMask;
    private const string PREVIEW_LAYER = "Preview";

    [MenuItem("Tools/Object Creator")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreatorWindow>("Object Creator");
    }

    private void OnEnable()
    {
        if (!LayerExists(PREVIEW_LAYER))
        {
            CreatePreviewLayer();
        }
        previewLayerMask = ~LayerMask.GetMask(PREVIEW_LAYER);
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
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
            if (GUILayout.Button("Reset Settings To Default"))
            {
                SetDefaultObjectParameters();
            }

            EditorGUILayout.Space(15);
        }
    }

    private void SelectGameObjectType()
    {
        GUILayout.Label("Select Object Type", EditorStyles.boldLabel);

        selectedObjectType = (TypeOfObject)EditorGUILayout.EnumPopup("Object Type", selectedObjectType);
    }

    private void InstantiateSelectedGameObject()
    {
        GameObject instance;

        switch (selectedObjectType)
        {
            case TypeOfObject.Primitive:

                selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Type", selectedPrimitiveType);

                if (GUILayout.Button("Instantiate Primitive"))
                {
                    instance = GameObject.CreatePrimitive(selectedPrimitiveType);
                    SetGameObjectParameters(instance, isPlacingObject);
                    Selection.activeGameObject = instance;

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Primitive");
                }

                EditorGUILayout.Space(20);

                if (GUILayout.Button("Place Primitive Manually"))
                {
                    previewObject = objectPlacer.CreatePreviewPrimitiveObject(selectedPrimitiveType);
                    previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                    isPlacingObject = true;
                    SetGameObjectParameters(previewObject, isPlacingObject);
                }
                break;

            case TypeOfObject.Prefab:

                selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

                if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
                {
                    if (GUILayout.Button("Instantiate Prefab"))
                    {
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        SetGameObjectParameters(instance, isPlacingObject);
                        Selection.activeGameObject = instance;

                        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
                    }

                    EditorGUILayout.Space(20);

                    if (GUILayout.Button("Place Prefab Manually"))
                    {
                        previewObject = objectPlacer.CreatePreviewPrefabObject(selectedPrefab);
                        previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                        isPlacingObject = true;
                        SetGameObjectParameters(previewObject, isPlacingObject);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a valid prefab.", MessageType.Warning);
                }
                break;
        }
    }

    private void SetDefaultObjectParameters()
    {
        GUI.FocusControl(null);

        objectName = "New Object";
        objectScale = 1f;
        objectPosition = Vector3.zero;
    }

    private void SetGameObjectParameters(GameObject instance, bool isPlacingObject)
    {
        if (!isPlacingObject)
        {
            instance.name = objectName;
            instance.transform.localScale = Vector3.one * objectScale;
            instance.transform.localPosition = objectPosition;
        }
        else
        {
            instance.name = objectName;
            instance.transform.localScale = Vector3.one * objectScale;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        currentEvent = Event.current;

        if (isPlacingObject && previewObject != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 400f, previewLayerMask))
            {
                if (hit.collider.gameObject != previewObject)
                {
                    previewObject.transform.position = hit.point;
                }
            }
            else
            {
                Renderer previewRenderer = previewObject.GetComponent<Renderer>();
                if (previewRenderer != null)
                {
                    objectHeight = previewRenderer.bounds.size.y * previewObject.transform.localScale.y;
                }


                if (ray.direction.y != 0) // Check to avoid division by zero
                {
                    float distanceToGround = -ray.origin.y / ray.direction.y;
                    Vector3 intersectionPoint = ray.origin + ray.direction * distanceToGround;

                    previewObject.transform.position = new Vector3(intersectionPoint.x, objectHeight / 2, intersectionPoint.z);
                }
                else
                {
                    previewObject.transform.position = new Vector3(ray.origin.x, objectHeight / 2, ray.origin.z); // or keep it at the original ray position
                }
            }


            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                if (selectedObjectType == TypeOfObject.Primitive)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrimitiveType);
                }
                else if (selectedObjectType == TypeOfObject.Prefab)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrefab);
                }

                DestroyImmediate(previewObject);
                isPlacingObject = false;
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 1)
            {
                DestroyImmediate(previewObject);
                isPlacingObject = false;
                currentEvent.Use();
            }

            sceneView.Repaint();
        }
    }

    private void CreatePreviewLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        for (int i = 0; i < layersProp.arraySize; i++)
        {
            SerializedProperty layer = layersProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = PREVIEW_LAYER; // Assign the layer name
                break;
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private bool LayerExists(string layerName)
    {
        return LayerMask.NameToLayer(layerName) != -1;
    }
}
