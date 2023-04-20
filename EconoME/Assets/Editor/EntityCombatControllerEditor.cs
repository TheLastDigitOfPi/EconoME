using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityCombatController), true)]
public class EntityCombatControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var controller = (EntityCombatController)target;
        if (GUILayout.Button("Test Hit Trigger"))
        {
            controller.TryReceiveAttack(null, out var report);
        }
    }

}