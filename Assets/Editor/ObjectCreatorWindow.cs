using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectCreatorWindow : EditorWindow
{
    private string objectName = "New Object";
    private float objectScale = 1f;
    private List<Object> assetsFound = new List<Object>();
    private int selectedIndex;

    private void OnEnable()
    {
        ObjectManager.FillAssetList();
        assetsFound = ObjectManager.Assets;
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
            sphere.name = objectName;
            sphere.transform.localScale = Vector3.one * objectScale;
        }
        
        if (assetsFound != null && assetsFound.Count > 0)
        {
            string[] assetNames = new string[assetsFound.Count];
            for (int i = 0; i < assetsFound.Count; i++)
            {
                assetNames[i] = assetsFound[i].name; // Get the name of each asset for the dropdown
            }

            // Create the dropdown menu
            selectedIndex = EditorGUILayout.Popup("Select Asset", selectedIndex, assetNames);

            // Optionally, you can display the selected asset details
            //if (GUILayout.Button("Show Selected Asset"))
            //{
            //    if (selectedIndex >= 0 && selectedIndex < assetsFound.Count)
            //    {
            //        Debug.Log($"Selected Asset: {assetsFound[selectedIndex].name}");
            //    }
            //}
        }
        else
        {
            EditorGUILayout.LabelField("No assets found.");
        }

        if(GUILayout.Button("Create Selected Prefab"))
        {
            GameObject prefab = (GameObject)assetsFound[selectedIndex];
            prefab.name = objectName;
            prefab.transform.localScale = Vector3.one * objectScale;

            Instantiate(prefab);
        }
    }
}
