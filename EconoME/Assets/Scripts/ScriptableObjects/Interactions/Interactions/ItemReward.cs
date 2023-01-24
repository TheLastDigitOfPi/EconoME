using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Reward Interaction", menuName = "ScriptableObjects/Interactions/Interactions/Item Reward")]
public class ItemReward : Interaction
{
    [SerializeField] DefinedScriptableItem[] rewards;

    public override event Action OnInteractionEnd;

    public override void Activate(InteractionHandler handler)
    {
        throw new NotImplementedException();
    }
}

