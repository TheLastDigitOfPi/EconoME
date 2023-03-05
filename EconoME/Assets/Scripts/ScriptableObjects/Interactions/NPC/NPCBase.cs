using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Object", menuName = "ScriptableObjects/NPCs/NPC Data")]
public class NPCBase : ScriptableObject
{
    [field: SerializeField] public int NPCID { get; private set; }
    [field: SerializeField] public string NPCName { get; private set; }

    [field: SerializeField][field: Multiline] public string Description { get; private set; }

    public NPCRelationshipStatus CurrentStatus { get { return _relationshipDeterminer.CalculateStatus(); } }

    [SerializeField] NPCRelationshipStats _relationshipDeterminer;
    [field: SerializeField] public NPCInteractionSet InteractionSet { get; private set; }
    
    [field:SerializeField] public WorldLocationData location {get; private set;}
}

public enum NPCRelationshipStatus
{
    MortalEnemies,
    Enemies,
    Annoyed,
    Neutral,
    Friends,
    BestFriends,
    Lovers
}

public enum NPCRealtionshipOperation
{
    SamllDislike,
    SmallRequestFail,
    SmallRequestSuccess,
    MediumRequestFail,
    MediumRequestSuccess,
    LargeRequestFail,
    LargeRequestSuccess,
}

