using UnityEngine;

public class ShopRaycast : Raycastable
{
    [SerializeField] BoolVariable ShopUI;

    public override bool OnRaycastHit(PlayerController owner, Collider2D collider)
    {
        ShopUI.Value = !ShopUI.Value;
        return true;
    }
}