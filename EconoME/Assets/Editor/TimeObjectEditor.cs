using UnityEditor;

[CustomEditor(typeof(TimeObject))]
public class TimeObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var timeObject = (TimeObject)target;
        timeObject.UpdateTotalSecondsPerDay();

    }

}
