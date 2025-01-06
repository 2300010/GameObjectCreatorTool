using UnityEditor;
using UnityEngine;

public class UIController
{
    private static UIController instance;
    public TypeOfObject SelectedObjectType { get => selectedObjectType; set => selectedObjectType = value; }

    private TypeOfObject selectedObjectType = TypeOfObject.Primitive;
    private bool showObjectSettings = true;

    GUILayoutOption[] resetBtnOptions = { GUILayout.MaxWidth(180) };
    GUIStyle resetStyle = GUIStyle.none;

    public static UIController Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new UIController();
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
                PropertyPanelManager.Instance.SetDefaultObjectParameters();
            }

            GUILayout.Space(8f);

            PropertyPanelManager.Instance.ObjectName = EditorGUILayout.TextField("Name", PropertyPanelManager.Instance.ObjectName);
            PropertyPanelManager.Instance.ObjectScale = EditorGUILayout.Slider("Scale", PropertyPanelManager.Instance.ObjectScale, 0.1f, 10f);
            PropertyPanelManager.Instance.ObjectPosition = EditorGUILayout.Vector3Field("Position", PropertyPanelManager.Instance.ObjectPosition);

        }
    }

    public void ShowSelectedGameObjectTypeUI()
    {
        GUILayout.Label("Select Object Type", EditorStyles.boldLabel);

        SelectedObjectType = (TypeOfObject)EditorGUILayout.EnumPopup("Object Type", SelectedObjectType);
    }
}
