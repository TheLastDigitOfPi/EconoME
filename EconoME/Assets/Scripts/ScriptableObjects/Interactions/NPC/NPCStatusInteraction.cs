using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Interaction", menuName = "ScriptableObjects/Interactions/NPCs/NPC Interaction")]
public class NPCStatusInteraction : ScriptableObject
{
    [SerializeField] Interaction[] _interactions;
    [SerializeField] NPCStatusObject _statusForInteraction;
    [SerializeField] GameEvent gameEvent;
    public Interaction[] Interactions { get { return _interactions; } }
    public NPCStatusObject StatusForInteraction { get { return _statusForInteraction; } }
    public GameEvent GameEvent { get { return gameEvent; } }
}

