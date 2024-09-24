using UnityEditor;
using UnityEngine;

public class ObjectCreatorWindow : EditorWindow
{
    private string objectName = "New Object";
    private float objectScale = 1f;

    private GUIContent assetsDropDown = new GUIContent("Select Asset");

    private void OnEnable()
    {
        ObjectManager.FillAssetList();
    }

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
            sphere.name = "New Sphere";
            sphere.transform.localScale = Vector3.one * objectScale;
        }

        if (EditorGUILayout.DropdownButton(assetsDropDown, FocusType.Keyboard))
        {
            //EditorGUILayout.BeginScrollView(Vector2.zero);
        }
    }
}
