using UnityEngine;

public class SquirrelMob : Mob
{
    [ContextMenu("Kill IT!")]
    public void KillMob()
    {
        Health = 0;
    }
    public override void OnDeath()
    {
        Debug.Log("Squirrel is dead");
    }
}
