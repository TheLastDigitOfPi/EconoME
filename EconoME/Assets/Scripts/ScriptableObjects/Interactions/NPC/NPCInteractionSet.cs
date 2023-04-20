using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Dataset", menuName = "ScriptableObjects/Interactions/NPCs/NPC Interaction Set")]
public class NPCInteractionSet : ScriptableObject
{
    public NPCStatusInteraction[] AllNPCInteractions;

    internal NPCStatusInteraction GetCurrentInteraction(NPCRelationshipStatus status, NPCRelationshipStatus NormalStatus, List<NPCStatusInteraction> completedInteractions)
    {
        var foundInteraction = AllNPCInteractions.FirstOrDefault(i => i.StatusForInteraction == status && !completedInteractions.Any(a => a.ID == i.ID));
        if (foundInteraction == null)
        {
            foundInteraction = AllNPCInteractions.FirstOrDefault(i => i.StatusForInteraction == NormalStatus && !completedInteractions.Any(a => a.ID == i.ID));
            if (foundInteraction == null)
            {
                return null;
            }
        }
        return foundInteraction;
    }
}

/* NPC Dialog Determiner
 * 
 * 
 * Maybe:
 * All NPCs derive off the same set of interactions (i.e. all angry will have the same set to choose from)
 * Then, each NPC will be able to override, add on, etc to the existing database to become more unique
 * 
 * 
 * 
 * 
 * No Recent Chat Base - "Hey, how's it going?"
 * Recent Chat Base - "What the frick you already talked to me"
 * 
 * Event Base - "Hey, want to do this thing for me?"
 * 
 * Current Action Base - "You mind? I'm on my way to the store"
 * 
 * 
 * Base What the frick you already talked to me
 * 
 * If the npc is busy - Walking, fighting, doesn't want to talk
 * Then play the current action base
 * 
 * If the Npc is not busy see if they have already been chat to for their current state and play Recent Chat base
 * 
 * If the npc is not busy and there is no recent chat, see if they have an event
 * 
 * If the npc doesn't have an event, play the No recent chat base
 * 
 * There should be a list of No Recent Chat Bases to choose from
 * 
 * There should be an event class that has a list of 
 * 
 * Each dialog path should have some list of recent chat bases
 * 
 * When does their current chat state change?
 * After x amount of time
 * Have player select from all available events
 * Event priority system where new ones override current ones
 * Each event expires after a certian amount of time
 * 
 * The current chat has a priority system
 * If some large event happens then the next chat will be about that
 * 
 * 
 * 
 * Ex: A zombie invasion occured
 * 
 * 
 * Major events change the responses people use
 * 
 * 
 * Player has to talk to NPC for quest
 * Player already has quest to talk to that NPC
 * An Event came up that the NPC wants to talk about
 * 
 * 
 * 
 */
