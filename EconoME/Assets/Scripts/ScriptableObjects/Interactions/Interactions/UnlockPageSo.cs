using System;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName ="New Unlock Page Interaction", menuName = "ScriptableObjects/Interactions/Unlock Page")]
public class UnlockPageSo : InteractionSO
{
    
    [SerializeField] UnlockableInventory PageToUnlock;

    public override Interaction GetInteraction()
    {
        return new UnlockPage(ID, PageToUnlock);
    }
}

public class UnlockPage : Interaction
{
    public override event Action OnInteractionEnd;
    [SerializeField] UnlockableInventory PageToUnlock;

    public UnlockPage(Guid interactionId, UnlockableInventory inventory) : base(interactionId)
    {
        PageToUnlock = inventory;
    }

    public override async void Activate(InteractionHandler handler)
    {
        PlayerBookHandler.Instance.UnlockPage(PageToUnlock);
        await Task.Delay(5000);
        PlayerBookHandler.Instance.CloseBook();
        await Task.Delay(800);
        OnInteractionEnd?.Invoke();
    }

}