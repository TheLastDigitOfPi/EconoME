using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(NPCScheduleHandler))]
[RequireComponent(typeof(NPCTravelingManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCDialogHandler))]
[RequireComponent(typeof(AIController))]
public class NPC : MonoBehaviour, IAmInteractable
{

    //Events
    public event Action OnStartTalkToPlayer;
    public event Action OnEndTalkToPlayer;
    //Public fields
    [field: SerializeField] public NPCBase NPCData { get; private set; }
    public NPCScheduleHandler NPCScheduler { get; private set; }
    public NPCTravelingManager TravelingManager { get; private set; }
    public Animator Animator { get; private set; }
    public NPCDialogHandler DialogHandler { get; private set; }
    //Local fields

    NPCStatusInteraction currentActiveInteraction;
    [field: SerializeField] Collider2D RaycastCollider { get; set; }


    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider)
    {
        if (NPCData == null) return false;
        if (collider != RaycastCollider) return false;
        if (ChatBoxManager.Instance.ChatBoxActive) return false;
        if (currentActiveInteraction != null) return false;

        if (!DialogHandler.TryGetInteraction(NPCData, out var interactionData))
        {
            return false;
        }
        
        var interactionSet = interactionData.Interactions;
        if (interactionSet == null) { return false; }
        List<Interaction> interactionsMade = new();
        for (int i = 0; i < interactionSet.Length; i++)
        {
            InteractionHandler.Instance.AddNew(interactionSet[i], out var interaction);
            interactionsMade.Add(interaction);
        }
        if (interactionsMade.Count == 0)
            return false;

        
        //PlayerBookHandler.Instance.CloseBook();
        currentActiveInteraction = interactionData;
        interactionsMade[interactionsMade.Count - 1].OnInteractionEnd += () => { TryRemoveInteractions(); OnEndTalkToPlayer?.Invoke(); };
        OnStartTalkToPlayer?.Invoke();
        return true;
    }

    private void TryRemoveInteractions()
    {
        DialogHandler.TryRemoveInteraction(currentActiveInteraction);
        currentActiveInteraction = null;
    }

    private void Awake()
    {
        NPCScheduler = GetComponent<NPCScheduleHandler>();
        TravelingManager = GetComponent<NPCTravelingManager>();
        Animator = GetComponent<Animator>();
        DialogHandler = GetComponent<NPCDialogHandler>();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //InteractionHandler.Instance.PlayerLeftNPC(currentActiveInteractions);
        }
    }
}

/* NPC Dialog Determiner
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
