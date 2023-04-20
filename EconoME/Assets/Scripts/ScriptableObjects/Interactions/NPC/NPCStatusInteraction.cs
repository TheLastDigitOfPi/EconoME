using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Interaction", menuName = "ScriptableObjects/Interactions/NPCs/NPC Interaction")]
public class NPCStatusInteraction : ScriptableObject
{
    [field: SerializeField] public InteractionSO[] Interactions { get; private set; }
    [field: SerializeField] public NPCRelationshipStatus StatusForInteraction { get; private set; }
    [field: SerializeField] public GameEvent GameEvent { get; private set; }
    public Guid ID { get; private set; } = Guid.NewGuid();
    [field: SerializeField] public bool CanRemoveAfterUse { get; private set; } = true;
    [SerializeField] string Id;

    private void OnValidate()
    {
        Id = ID.ToString();
    }
}

