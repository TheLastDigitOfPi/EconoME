using System;
using UnityEngine;

[CreateAssetMenu(fileName ="New Receive Item Interaction", menuName = "ScriptableObjects/Interactions/Receive Item")]
public class ReceiveItemSO : InteractionSO
{
    [SerializeField] DefinedScriptableItem item;

    public override Interaction GetInteraction()
    {
        return new ReceiveItem(ID, item);
    }
}
public class ReceiveItem : Interaction
{
    public override event Action OnInteractionEnd;
    [SerializeField] DefinedScriptableItem item;

    public ReceiveItem(Guid interactionId, DefinedScriptableItem item) : base(interactionId)
    {
        this.item = item;
    }

    public override void Activate(InteractionHandler handler)
    {
        ReceiveItemManager.ReceiveItem(item);
        ReceiveItemManager.Instance.OnItemReceived += ItemReceived;
    }

    private void ItemReceived()
    {
        OnInteractionEnd?.Invoke();
        ReceiveItemManager.Instance.OnItemReceived -= ItemReceived;

    }
}