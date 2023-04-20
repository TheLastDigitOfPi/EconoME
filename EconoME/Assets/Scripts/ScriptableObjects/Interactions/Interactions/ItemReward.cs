using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Reward Interaction", menuName = "ScriptableObjects/Interactions/Interactions/Item Reward")]
public class ItemRewardSO : InteractionSO
{
    [SerializeField] DefinedScriptableItem[] rewards;
    public override Interaction GetInteraction()
    {
        return new ItemReward(ID, rewards);    
    }
}

public class ItemReward : Interaction
{
    DefinedScriptableItem[] rewards;
    public ItemReward(Guid interactionId, DefinedScriptableItem[] rewards) : base(interactionId)
    {
        this.rewards = rewards;
    }

    public override event Action OnInteractionEnd;

    public override void Activate(InteractionHandler handler)
    {
        throw new NotImplementedException();
    }
}

