using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "New Test Message Enchant", menuName = "ScriptableObjects/Enchants/Custom/Effects/Test Message")]
public class TestEnchantInConsoleSO : ArmorEffectSO
{
    [SerializeField] string Message;

    public override bool TryGetEffect(EntityCombatController owner, out ArmorEffect effect)
    {
        effect = new TestEnchantInConsole(Message);
        return true;
    }
}

public class TestEnchantInConsole : ArmorEffect
{
    [SerializeField] string Message;

    public TestEnchantInConsole(string message)
    {
        Message = message;
    }

    public override void OnUnequip()
    {
        Debug.Log("Bye!");
    }

    public override void TriggerEffect()
    {
        Debug.Log(Message);
    }
}

