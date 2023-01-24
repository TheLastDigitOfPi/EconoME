using UnityEngine;

public class ShopRaycast : MonoBehaviour, IAmInteractable
{
    [SerializeField] BoolVariable ShopUI;

    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider)
    {
        ShopUI.Value = !ShopUI.Value;
        return true;
    }
}