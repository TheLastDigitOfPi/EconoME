using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Interaction", menuName = "ScriptableObjects/Interactions/NPCs/NPC Interaction")]
public class NPCStatusInteraction : ScriptableObject
{
    [field: SerializeField] public Interaction[] Interactions {get; private set;}
    [field: SerializeField] public NPCRelationshipStatus StatusForInteraction {get; private set;}
    [field:SerializeField] public GameEvent GameEvent {get; private set;}
}

