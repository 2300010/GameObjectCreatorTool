using UnityEditor;

public class TimeTracker
{
    private static TimeTracker instance;

    public static TimeTracker Instance 
    {  
        get 
        {
            if (instance == null)
            {
                instance = new TimeTracker();
            }
            return instance; 
        }
    }

    public float EditorDeltaTime { get => editorDeltaTime; }

    private double lastTimeSinceStartup;
    private float editorDeltaTime;

    public void StartTimeTracker()
    {
        lastTimeSinceStartup = EditorApplication.timeSinceStartup;
    }

    public void CalculateEditorDeltaTime()
    {
        double currentTime = EditorApplication.timeSinceStartup;
        editorDeltaTime = (float)(currentTime - lastTimeSinceStartup);
        lastTimeSinceStartup = currentTime;
    }
}
