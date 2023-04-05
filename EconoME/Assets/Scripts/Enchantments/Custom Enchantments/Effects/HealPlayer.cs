using UnityEngine;

public class HealPlayer : ArmorEffect
{
    [SerializeField] int HealAmount;

    public override void OnUnequip()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerEffect()
    {

    }
}


