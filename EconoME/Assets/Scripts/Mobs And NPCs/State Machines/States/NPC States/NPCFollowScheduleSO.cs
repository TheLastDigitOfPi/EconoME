using UnityEngine;

[CreateAssetMenu(fileName = "NPC Schedule", menuName ="ScriptableObjects/NPCs/AI/States/NPC Follow Schedule")]
public class NPCFollowScheduleSO : AIStateSO
{
    [SerializeReference] NPCFollowSchedule TravelSettings = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new NPCFollowSchedule(controller, TravelSettings);
        return state.PassedValidation;
    }
}

