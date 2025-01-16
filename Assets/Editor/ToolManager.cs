using UnityEditor;
using UnityEngine;

public enum TypeOfObject { Primitive, Prefab }

public delegate TypeOfObject TypeOfObjectHasChanged(TypeOfObject selectedType);
public delegate PrimitiveType SelectedPrimitiveTypeChanged(PrimitiveType selectedPrimitiveType);
public delegate GameObject SelectedPrefabChanged(GameObject selectedPrefab);


public class ToolManager : EditorWindow
{
    private PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;
    private GameObject selectedPrefab;
    private GameObject previewObject;

    private bool isPlacingObject = false;
    private float objectHeight = 0f;
    private float previousMousePositionOnY = 0f;
    private float currentMousePositionOnY = 0f;
    private float currentMouseDeltaY = 0f;
    private float previewObjectCurrentPositionOnY;

    //VARIABLES FOR NEW TEST OF SHIFT FEATURE
    private float lockedX;
    private float lockedZ;

    Event currentEvent;

    #region GUI Variables
    private GUILayoutOption[] placeObjectBtnOptions = { GUILayout.MinHeight(40) };
    private GUIStyle placeObjectBtnStyle = new GUIStyle();
    private GUIStyle basicButtonStyle = new GUIStyle();
    #endregion

    #region Constants
    private const string INSTANTIATE_GAMEOBJECT_BTN_TEXT = "Create GameObject To Default Position";
    private const string INSTANTIATE_GAMEOBJECT_UNDO_REGISTER = "Instantiate GameObject = ";
    private const string PLACE_GAMEOBJECT_MANUALLY_BTN_TEXT = "Place GameObject Manually";
    private const string PREFAB_TEXT = "Prefab ";
    private const string PRIMITIVE_TEXT = "Primitive ";
    private const string GAMEOBJECT_TEXT = "GameObject";
    #endregion

    [MenuItem("Tools/Object Creator")]
    public static void ShowWindow()
    {
        GetWindow<ToolManager>("Object Creator");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        TimeTracker.Instance.StartTimeTracker();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        basicButtonStyle = GUI.skin.button;
        placeObjectBtnStyle = GUI.skin.button;
        ObjectParametersUI.Instance.ShowSelectedGameObjectTypeUI();

        InstantiateSelectedGameObject();
    }

    private void InstantiateSelectedGameObject()
    {
        GameObject instance;

        //placeObjectBtnStyle.fontSize = 18;

        switch (ObjectParametersUI.Instance.SelectedObjectType)
        {
            case TypeOfObject.Primitive:

                selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup(PRIMITIVE_TEXT + GAMEOBJECT_TEXT, selectedPrimitiveType);

                ObjectParametersUI.Instance.ShowObjectParametersUI();

                if (GUILayout.Button(INSTANTIATE_GAMEOBJECT_BTN_TEXT, basicButtonStyle))
                {
                    instance = GameObject.CreatePrimitive(selectedPrimitiveType);
                    ObjectParametersManager.Instance.SetGameObjectParameters(instance, isPlacingObject);
                    Selection.activeGameObject = instance;

                    Undo.RegisterCreatedObjectUndo(instance, INSTANTIATE_GAMEOBJECT_UNDO_REGISTER + instance.name);
                }


                EditorGUILayout.Space(20);

                if (GUILayout.Button(PLACE_GAMEOBJECT_MANUALLY_BTN_TEXT, placeObjectBtnStyle, placeObjectBtnOptions))
                {
                    previewObject = ObjectPlacer.Instance.CreatePreviewPrimitiveObject(selectedPrimitiveType);
                    Renderer previewRenderer = previewObject.GetComponent<Renderer>();

                    if (previewRenderer != null)
                    {
                        objectHeight = previewRenderer.bounds.size.y * previewObject.transform.localScale.y;
                    }

                    previewObjectCurrentPositionOnY = objectHeight / 2;
                    Vector3 newPosition = new Vector3(0, previewObjectCurrentPositionOnY, 0);
                    previewObject.transform.position += newPosition;
                    //previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                    isPlacingObject = true;
                    ObjectParametersManager.Instance.SetGameObjectParameters(previewObject, isPlacingObject);
                }
                break;

            case TypeOfObject.Prefab:

                selectedPrefab = (GameObject)EditorGUILayout.ObjectField(PREFAB_TEXT + GAMEOBJECT_TEXT, selectedPrefab, typeof(GameObject), false);

                if (selectedPrefab != null && PrefabUtility.IsPartOfPrefabAsset(selectedPrefab))
                {
                    if (GUILayout.Button(INSTANTIATE_GAMEOBJECT_BTN_TEXT))
                    {
                        instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                        ObjectParametersManager.Instance.SetGameObjectParameters(instance, isPlacingObject);
                        Selection.activeGameObject = instance;

                        Undo.RegisterCreatedObjectUndo(instance, INSTANTIATE_GAMEOBJECT_UNDO_REGISTER + instance.name);
                    }

                    ObjectParametersUI.Instance.ShowObjectParametersUI();

                    EditorGUILayout.Space(20);

                    if (GUILayout.Button(PLACE_GAMEOBJECT_MANUALLY_BTN_TEXT, placeObjectBtnStyle, placeObjectBtnOptions))
                    {
                        previewObject = ObjectPlacer.Instance.CreatePreviewPrefabObject(selectedPrefab);
                        Renderer previewRenderer = previewObject.GetComponent<Renderer>();

                        if (previewRenderer != null)
                        {
                            objectHeight = previewRenderer.bounds.size.y * previewObject.transform.localScale.y;
                        }

                        previewObjectCurrentPositionOnY = objectHeight / 2;
                        Vector3 newPosition = new Vector3(0, previewObjectCurrentPositionOnY, 0);
                        previewObject.transform.position += newPosition;
                        //previewObject.layer = LayerMask.NameToLayer(PREVIEW_LAYER);
                        isPlacingObject = true;
                        ObjectParametersManager.Instance.SetGameObjectParameters(previewObject, isPlacingObject);
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
        //Debug.Log("Mouse delta = " + currentEvent.mousePosition.y);

        if (isPlacingObject && previewObject != null)
        {
            currentMousePositionOnY = currentEvent.mousePosition.y;

            currentMouseDeltaY = (currentMousePositionOnY - previousMousePositionOnY) * -1f;

            previousMousePositionOnY = currentMousePositionOnY;

            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            Selection.activeObject = null;

            Vector3 mouseWorldPosition = ray.origin;

            if (ray.direction.y != 0)
            {
                float distanceToGround = -ray.origin.y / ray.direction.y;
                Vector3 intersectionPoint = ray.origin + ray.direction * distanceToGround;


                if (currentEvent.shift)
                {
                    //Debug.Log("Mouse world position Y = " + mouseWorldPosition.y);

                    lockedX = previewObject.transform.position.x;
                    lockedZ = previewObject.transform.position.z;

                    if (currentMouseDeltaY != 0f)
                    {
                        //Debug.Log("Mouse delta Y = " + currentMouseDeltaY);

                        float previewObjectNewPositionOnY = previewObjectCurrentPositionOnY + currentMouseDeltaY;

                        previewObject.transform.position = new Vector3(lockedX, previewObjectNewPositionOnY, lockedZ);

                        previewObjectCurrentPositionOnY = previewObjectNewPositionOnY;
                    }


                }
                else
                {
                    previewObject.transform.position = new Vector3(intersectionPoint.x, previewObjectCurrentPositionOnY, intersectionPoint.z);
                }
            }

            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                if (ObjectParametersUI.Instance.SelectedObjectType == TypeOfObject.Primitive)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrimitiveType);
                }
                else if (ObjectParametersUI.Instance.SelectedObjectType == TypeOfObject.Prefab)
                {
                    ObjectPlacer.PlaceObject(previewObject, selectedPrefab);
                }

                DestroyImmediate(previewObject);
                isPlacingObject = false;
                ObjectParametersManager.Instance.ObjectPosition = Vector3.zero;
                Repaint();
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
            {
                DestroyImmediate(previewObject);
                isPlacingObject = false;
                ObjectParametersManager.Instance.ObjectPosition = Vector3.zero;
                Repaint();
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseMove)
            {
                //Debug.Log("Current mouse y delta = " + currentMouseDeltaY);

                ObjectParametersManager.Instance.ObjectPosition = previewObject.transform.position;

                Repaint();
            }

            sceneView.Repaint();
        }
    }
}
