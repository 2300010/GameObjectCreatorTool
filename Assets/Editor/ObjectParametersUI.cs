using UnityEditor;
using UnityEngine;

public class ObjectParametersUI
{
    private static ObjectParametersUI instance;
    public TypeOfObject SelectedObjectType { get => selectedObjectType; set => selectedObjectType = value; }

    private TypeOfObject selectedObjectType = TypeOfObject.Primitive;
    private bool showObjectSettings = true;

    GUILayoutOption[] resetBtnOptions = { GUILayout.MaxWidth(180) };
    GUIStyle resetStyle = GUIStyle.none;

    public static ObjectParametersUI Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new ObjectParametersUI();
            }
            return instance; 
        }
    }
    public void ShowObjectParametersUI()
    {
        showObjectSettings = EditorGUILayout.Foldout(showObjectSettings, "Object Settings", true);

        if (showObjectSettings)
        {
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            if (GUILayout.Button("Reset Settings To Default", resetBtnOptions))
            {
                ObjectParametersManager.Instance.SetDefaultObjectParameters();
            }

            GUILayout.Space(8f);

            ObjectParametersManager.Instance.ObjectName = EditorGUILayout.TextField("Name", ObjectParametersManager.Instance.ObjectName);
            ObjectParametersManager.Instance.ObjectScale = EditorGUILayout.Slider("Scale", ObjectParametersManager.Instance.ObjectScale, 0.1f, 10f);
            ObjectParametersManager.Instance.ObjectPosition = EditorGUILayout.Vector3Field("Position", ObjectParametersManager.Instance.ObjectPosition);

        }
    }

    public void ShowSelectedGameObjectTypeUI()
    {
        GUILayout.Label("Select Object Type", EditorStyles.boldLabel);

        SelectedObjectType = (TypeOfObject)EditorGUILayout.EnumPopup("Object Type", SelectedObjectType);
    }
}
