using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Object", menuName = "ScriptableObjects/Interactions/NPCs/NPC Data")]
[Serializable]
public class NPCObject : ScriptableObject
{
    public int ID;
    public string Name;
    public string Description;
    [SerializeField] NPCInteractionSet _interactionSet;
    [SerializeField] NPCStatusObject _defaultStatus;
    [SerializeField] NPCStatusObject[] AvailableStatus;
    public NPCStatusObject CurrentStatus {get {return GetCurrentStatus();}}
    public NPCStatusObject DefaultStatus { get { return _defaultStatus; } }
    public NPCInteractionSet InteractionSet { get { return _interactionSet; } }

    public NPCStatusObject GetCurrentStatus()
    {
        return _defaultStatus;
    }

}

