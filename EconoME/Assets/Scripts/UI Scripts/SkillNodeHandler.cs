using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillNodeHandler : MonoBehaviour
{
    [SerializeReference] public SkillNode node = new SkillNode();
    public void UnlockNode()
    {
        if (node.Unlock()) { AnimateNode(); }
    }

    void AnimateNode()
    {
    }


}

[Serializable]
public class SkillNode
{
    public int NodeID;
    public int price;
    public bool unlocked;
    public List<int> RequiredNodes = new List<int>();
    [SerializeField] IntVariable PlayerCurrency;
    public virtual bool Unlock()
    {
        if (unlocked) { return false; }
        if (PlayerCurrency.Value < price) { return false; }

        if (!SkillTreeHandler.Instance.AreNodesUnlocked(RequiredNodes)) { return false; }

        unlocked = true;
        return false;
    }
    public SkillNode() { }
    public SkillNode(SkillNode other)
    {
        NodeID = other.NodeID;
    }
    public enum SkillNodeType
    {
        ToolUpgrade,
        EconomyUpgrade,
        TileUpgrade
    }
    public SkillNodeType NodeType = 0;

}

#region Tool Upgrades

[Serializable]
public class ToolUpgradeNode : SkillNode
{

    public enum upgradeToolType
    {
        Pickaxe,
        Axe,
        Sword,
        Bow
    }
    public upgradeToolType upgradeTool;

    public ToolUpgradeNode(SkillNode other) : base(other)
    {

    }

    public override bool Unlock()
    {
        if (!base.Unlock()) { return false; }


        return true;
    }
}

[Serializable]
public class PickaxeUpgradeNode : ToolUpgradeNode
{
    [SerializeField] float swingDamageBoost;

    public PickaxeUpgradeNode(SkillNode other) : base(other)
    {
    }
}



#endregion

[Serializable]
public class EconomyUpgradeNode : SkillNode
{
    public enum economyUpgradeType
    {

    }
    economyUpgradeType upgradeType;

}
[Serializable]
public class TileUpgradeNode : SkillNode
{
    public enum tileUpgradeType
    {

    }
    tileUpgradeType upgradeType;
}

