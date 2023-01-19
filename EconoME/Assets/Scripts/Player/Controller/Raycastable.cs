using UnityEngine;

public abstract class Raycastable : MonoBehaviour
{
    public abstract bool OnRaycastHit(PlayerController owner, Collider2D collider);

}
