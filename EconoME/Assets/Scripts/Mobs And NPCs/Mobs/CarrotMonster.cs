using UnityEngine;

public class CarrotMonster : Mob
{
    [ContextMenu("Kill IT!")]
    public void KillMob()
    {
        Health = 0;
    }
    public override void OnDeath()
    {
        Debug.Log("Carrot Monster is dead");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            IsAngry = true;
        }
    }
}
