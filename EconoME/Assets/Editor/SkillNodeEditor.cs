using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillNodeHandler)), CanEditMultipleObjects]
public class SkillNodeEditor : Editor
{
    int enumVal = 0;
    int ToolEnumVal = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SkillNodeHandler handler = (SkillNodeHandler)target;

        do
        {
            if (handler.node is ToolUpgradeNode)
            {
                ToolUpgradeNode toolNode = (ToolUpgradeNode)handler.node;
                ToolUpgradeNode.upgradeToolType TT = toolNode.upgradeTool;

                if ((int)TT == ToolEnumVal) { break; }
                switch (TT)
                {
                    case ToolUpgradeNode.upgradeToolType.Pickaxe:
                        Debug.Log("Chaning to pickaxe upgrade");
                        handler.node = new PickaxeUpgradeNode(handler.node);
                        break;
                    case ToolUpgradeNode.upgradeToolType.Axe:
                        handler.node = new ToolUpgradeNode(handler.node);
                        break;
                    case ToolUpgradeNode.upgradeToolType.Sword:
                        break;
                    case ToolUpgradeNode.upgradeToolType.Bow:
                        break;
                    default:
                        break;
                }
                ToolEnumVal = (int)TT;
                return;
            }
        } while (false);

        SkillNode.SkillNodeType NT = handler.node.NodeType;
        if ((int)NT == enumVal) { return; }
        switch (NT)
        {
            case SkillNode.SkillNodeType.ToolUpgrade:
                Debug.Log("Changing to tool upgrade");
                handler.node = new ToolUpgradeNode(handler.node);
                handler.node.price++;
                break;
            case SkillNode.SkillNodeType.EconomyUpgrade:
                Debug.Log("changed to economy upgrade");
                break;
            case SkillNode.SkillNodeType.TileUpgrade:
                break;
            default:
                break;
        }
        enumVal = (int)NT;

    }

}
