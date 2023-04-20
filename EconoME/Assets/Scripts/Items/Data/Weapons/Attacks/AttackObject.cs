using UnityEngine;
public abstract class AttackObject : MonoBehaviour
{
    public abstract void Initialize(Weapon attackWeapon, EntityCombatController attacker, Vector2 direction);
}
