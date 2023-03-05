using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCSchedule), true), CanEditMultipleObjects]
public class NPCScheduleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        NPCSchedule _interaction = (NPCSchedule)target;
        if (GUILayout.Button("Organize And Validate Schedules"))
        {
            _interaction.SortList();
            _interaction.ValidateSchedules();
        }
    }
}
