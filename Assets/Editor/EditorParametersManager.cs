using UnityEditor;
using UnityEngine;

public static class EditorParametersManager
{
    public static Vector2 GetSceneViewSize()
    {
        if (SceneView.lastActiveSceneView == null)
        {
            Debug.LogWarning("No active scene view found!");
            return Vector2.zero;
        }

        Rect currentSceneView = SceneView.lastActiveSceneView.position;
        return new Vector2(currentSceneView.width, currentSceneView.height);
    }

    public static Vector3 GetScreenToSceneMouseDelta(Vector2 screenMouseDelta, Vector3 currentObjectPosition)
    {
        if (!SceneView.lastActiveSceneView || !SceneView.lastActiveSceneView.camera)
        {
            Debug.LogWarning("No active scene view or camera was found!");
            return Vector3.zero;
        }

        Camera editorCamera = SceneView.lastActiveSceneView.camera;

        float distanceFromCameraToObject = Vector3.Distance(editorCamera.transform.position, currentObjectPosition);

        Vector3 mouseStartingPoint = new Vector3(screenMouseDelta.x, screenMouseDelta.y, distanceFromCameraToObject);
        Vector3 worldMouseDelta = editorCamera.ScreenToWorldPoint(mouseStartingPoint) - editorCamera.ScreenToWorldPoint(Vector3.zero);

        return worldMouseDelta;
    }
}
