using UnityEngine;

public class ObjectParametersManager
{
    private static ObjectParametersManager instance;

    public static ObjectParametersManager Instance 
    {
        get 
        {
            if (instance == null)
            {
                instance = new ObjectParametersManager();
            }
            return instance;
        } 
    }

    public string ObjectName { get => objectName; set => objectName = value; }
    public float ObjectScale { get => objectScale; set => objectScale = value; }
    public Vector3 ObjectPosition { get => objectPosition; set => objectPosition = value; }

    private string objectName = "New Object";
    private float objectScale = 1f;
    private Vector3 objectPosition = Vector3.zero;

    public void SetDefaultObjectParameters()
    {
        GUI.FocusControl(null);

        ObjectName = "New Object";
        ObjectScale = 1f;
        ObjectPosition = Vector3.zero;
    }

    public void SetGameObjectParameters(GameObject instance, bool isPlacingObject)
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
}
