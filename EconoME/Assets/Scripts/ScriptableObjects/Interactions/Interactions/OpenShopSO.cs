using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Open Shop Interaction", menuName = "ScriptableObjects/Interactions/Interactions/Open Shop Interaction")]
public class OpenShopSO : InteractionSO
{
    [SerializeField] BoolVariable ShopToggler;

    public override Interaction GetInteraction()
    {
        return new OpenShop(ID, ShopToggler);
    }
}

public class OpenShop : Interaction
{
    [SerializeField] BoolVariable ShopToggler;

    public OpenShop(Guid interactionId,BoolVariable shopToggler) : base(interactionId)
    {
        ShopToggler = shopToggler;
    }

    public override event Action OnInteractionEnd;

    public override void Activate(InteractionHandler handler)
    {
        if(ShopToggler == null){return;}
        ShopToggler.Value = true;
    }
}