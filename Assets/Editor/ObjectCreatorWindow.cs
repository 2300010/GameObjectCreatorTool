using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCreatorWindow : EditorWindow
{
    private PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;

    private GameObject selectedPrefab;
    private GameObject previewObject;

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
        UIController.Instance.ShowObjectParametersUI();

        UIController.Instance.ShowSelectedGameObjectTypeUI();

        InstantiateSelectedGameObject();
    }

    private void InstantiateSelectedGameObject()
    {
        GameObject instance;

        switch (UIController.Instance.SelectedObjectType)
        {
            case TypeOfObject.Primitive:

                selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Type", selectedPrimitiveType);

                if (GUILayout.Button("Instantiate Primitive"))
                {
                    instance = GameObject.CreatePrimitive(selectedPrimitiveType);
                    PropertiesManager.Instance.SetGameObjectParameters(instance, isPlacingObject);
                    Selection.activeGameObject = instance;

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Primitive");
                }

                EditorGUILayout.Space(20);

                if (GUILayout.Button("Place Primitive Manually"))
                {
                    previewObject = ObjectPlacer.Instance.CreatePreviewPrimitiveObject(selectedPrimitiveType);
                    previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                    isPlacingObject = true;
                    PropertiesManager.Instance.SetGameObjectParameters(previewObject, isPlacingObject);
                }
                break;

            case TypeOfObject.Prefab:

                selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", selectedPrefab, typeof(GameObject), false);

                if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
                {
                    if (GUILayout.Button("Instantiate Prefab"))
                    {
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        PropertiesManager.Instance.SetGameObjectParameters(instance, isPlacingObject);
                        Selection.activeGameObject = instance;

                        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Prefab");
                    }

                    EditorGUILayout.Space(20);

                    if (GUILayout.Button("Place Prefab Manually"))
                    {
                        previewObject = ObjectPlacer.Instance.CreatePreviewPrefabObject(selectedPrefab);
                        previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                        isPlacingObject = true;
                        PropertiesManager.Instance.SetGameObjectParameters(previewObject, isPlacingObject);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a valid prefab.", MessageType.Warning);
                }
                break;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        currentEvent = Event.current;

        if (isPlacingObject && previewObject != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (currentEvent.shift)
            {

            }

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


                if (ray.direction.y != 0)
                {
                    float distanceToGround = -ray.origin.y / ray.direction.y;
                    Vector3 intersectionPoint = ray.origin + ray.direction * distanceToGround;

                    previewObject.transform.position = new Vector3(intersectionPoint.x, objectHeight / 2, intersectionPoint.z);
                }
                else
                {
                    previewObject.transform.position = new Vector3(ray.origin.x, objectHeight / 2, ray.origin.z);
                }
            }


            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                if (UIController.Instance.SelectedObjectType == TypeOfObject.Primitive)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrimitiveType);
                }
                else if (UIController.Instance.SelectedObjectType == TypeOfObject.Prefab)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrefab);
                }

                DestroyImmediate(previewObject);
                isPlacingObject = false;
                PropertiesManager.Instance.ObjectPosition = Vector3.zero;
                Repaint();
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
            {
                DestroyImmediate(previewObject);
                isPlacingObject = false;
                PropertiesManager.Instance.ObjectPosition = Vector3.zero;
                Repaint();
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseMove)
            {
                PropertiesManager.Instance.ObjectPosition = previewObject.transform.position;

                Repaint();
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
