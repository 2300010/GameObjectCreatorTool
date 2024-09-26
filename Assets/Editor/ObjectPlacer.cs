using UnityEngine;
using UnityEditor;


public class ObjectPlacer : MonoBehaviour
{

    // Store whether to show the Gizmo
    public bool showGizmo = true;

    private void Update()
    {
        // This logic might be different since we're in editor mode
        if (showGizmo)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.lastActiveSceneView.position.height - mousePosition.y;
            mousePosition.z = 0;
        }
    }
}
