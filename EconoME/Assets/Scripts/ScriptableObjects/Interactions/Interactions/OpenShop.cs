using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Open Shop Interaction", menuName = "ScriptableObjects/Interactions/Interactions/Open Shop Interaction")]
public class OpenShop : Interaction
{
    [SerializeField] BoolVariable ShopToggler;

    public override event Action OnInteractionEnd;

    public override void Activate(InteractionHandler handler)
    {
        if(ShopToggler == null){return;}
        ShopToggler.Value = true;
    }
}

