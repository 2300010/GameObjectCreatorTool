using UnityEditor;
using UnityEngine;

public class UIController
{
    private static UIController instance;

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

    public TypeOfObject SelectedObjectType { get => selectedObjectType; set => selectedObjectType = value; }

    private TypeOfObject selectedObjectType = TypeOfObject.Primitive;
    private bool showObjectSettings = true;

    public void ShowObjectParametersUI()
    {
        showObjectSettings = EditorGUILayout.Foldout(showObjectSettings, "Object Settings", true);

        if (showObjectSettings)
        {
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);

            PropertyPanelManager.Instance.ObjectName = EditorGUILayout.TextField("Object Name", PropertyPanelManager.Instance.ObjectName);
            PropertyPanelManager.Instance.ObjectScale = EditorGUILayout.Slider("Object Scale", PropertyPanelManager.Instance.ObjectScale, 0.1f, 10f);
            PropertyPanelManager.Instance.ObjectPosition = EditorGUILayout.Vector3Field("Object Position", PropertyPanelManager.Instance.ObjectPosition);
            if (GUILayout.Button("Reset Settings To Default"))
            {
                PropertyPanelManager.Instance.SetDefaultObjectParameters();
            }

            EditorGUILayout.Space(15);
        }
    }

    public void ShowSelectedGameObjectTypeUI()
    {
        GUILayout.Label("Select Object Type", EditorStyles.boldLabel);

        SelectedObjectType = (TypeOfObject)EditorGUILayout.EnumPopup("Object Type", SelectedObjectType);
    }
}
