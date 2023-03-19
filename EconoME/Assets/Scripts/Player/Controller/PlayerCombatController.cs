using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    //Statics
    public static PlayerCombatController Instance { get; private set; }

    //Public fields
    [field: SerializeField] public float PlayerCurrentHealth { get; private set; }
    public float PlayerMaxHealth;

    //Events
    public event Action OnPlayerHit;
    public event Action OnPlayerDeath;
    public event Action OnPlayerFullyHealed;
    public event Action OnPlayerPassiveHeal;
    public event Action OnPlayerAttack;
    public event Action OnPlayerCastSpell;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 player combat controller found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Attempt to hit the player with the given attack
    /// </summary>
    /// <param name="attack"></param>
    /// <returns>Returns true if attack connects</returns>
    public bool HitPlayer(DamageInstance attack)
    {
        return false;
    }
}
