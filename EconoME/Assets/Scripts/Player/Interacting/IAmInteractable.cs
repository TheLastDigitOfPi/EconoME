using UnityEngine;
public interface IAmInteractable
{
    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider);
}