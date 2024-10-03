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

    //private Material previewMaterial;

    //private Material[] originalMaterials = null;
    //private Renderer[] originalRenderers = null;

    private bool showObjectSettings = false;
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
        //if (previewMaterial == null)
        //{
        //    SetPreviewMaterialParameters();
        //}
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
                    SetGameObjectParameters(instance);

                    Undo.RegisterCreatedObjectUndo(instance, "Instantiate Primitive");
                }

                if (GUILayout.Button("Place Primitive Manually"))
                {
                    previewObject = objectPlacer.CreatePreviewPrimitiveObject(selectedPrimitiveType);
                    previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                    isPlacingObject = true;
                }
                break;

            case TypeOfObject.Prefab:

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

                        previewObject = objectPlacer.CreatePreviewPrefabObject(selectedPrefab);
                        previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                        isPlacingObject = true;
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

                //Debug.Log("Object height = " + objectHeight);

                if (ray.direction.y != 0) // Check to avoid division by zero
                {
                    float distanceToGround = -ray.origin.y / ray.direction.y;
                    Vector3 intersectionPoint = ray.origin + ray.direction * distanceToGround;

                    // Set the preview object position to the intersection point
                    previewObject.transform.position = new Vector3(intersectionPoint.x, objectHeight / 2, intersectionPoint.z);
                }
                else
                {
                    // Handle cases where ray direction is parallel to the ground (y == 0)
                    previewObject.transform.position = new Vector3(ray.origin.x, objectHeight / 2, ray.origin.z); // or keep it at the original ray position
                }
            }

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
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

    //private void SetPreviewMaterialParameters()
    //{
    //    previewMaterial.SetFloat("_Mode", 3); // Transparent mode for Standard Shader (3)
    //    previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    //    previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //    previewMaterial.SetInt("_ZWrite", 0);
    //    previewMaterial.DisableKeyword("_ALPHATEST_ON");
    //    previewMaterial.EnableKeyword("_ALPHABLEND_ON");
    //    previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //    previewMaterial.renderQueue = 3000;
    //}
}
