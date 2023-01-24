using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(NPCScheduleManager))]
[RequireComponent(typeof(NPCTravelingManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCDialogHandler))]
[RequireComponent(typeof(AIController))]
public class NPC : MonoBehaviour, IAmInteractable
{
    [field: SerializeField] public NPCBase NPCData { get; private set; }

    public NPCScheduleManager NPCScheduler { get; private set; }
    public NPCTravelingManager TravelingManager { get; private set; }
    public Animator Animator { get; private set; }
    public NPCDialogHandler DialogHandler { get; private set; }

    Guid[] currentActiveInteractions;
    [field: SerializeField] Collider2D RaycastCollider { get; set; }

    public event Action OnStartTalkToPlayer;
    public event Action OnEndTalkToPlayer;

    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider)
    {
        if (NPCData == null) { return false; }
        if (collider != RaycastCollider) { return false; }

        if(!DialogHandler.TryGetInteraction(NPCData, out Interaction[] interactionSet))
        {
            return false;
        }
        if (interactionSet == null) { return false; }
        currentActiveInteractions = new Guid[interactionSet.Length];
        for (int i = 0; i < interactionSet.Length; i++)
        {
            InteractionHandler.Instance.AddNew(interactionSet[i]);
            currentActiveInteractions[i] = interactionSet[i].ID;
        }
        interactionSet.Last().OnInteractionEnd += () => { OnEndTalkToPlayer?.Invoke(); };
        return true;
    }

    private void Awake()
    {
        NPCScheduler = GetComponent<NPCScheduleManager>();
        TravelingManager = GetComponent<NPCTravelingManager>();
        Animator = GetComponent<Animator>();
        DialogHandler = GetComponent<NPCDialogHandler>();
    }

    private void Start()
    {

        for (int i = 0; i < NPCData.InteractionSet.AllNPCInteractions.Length; i++)
        {
            for (int j = 0; j < NPCData.InteractionSet.AllNPCInteractions[i].Interactions.Length; j++)
            {
                if (NPCData.InteractionSet.AllNPCInteractions[i].Interactions[j] is TextPopup)
                {
                    (NPCData.InteractionSet.AllNPCInteractions[i].Interactions[j] as TextPopup).titleName = NPCData.NPCName;
                }
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //InteractionHandler.Instance.PlayerLeftNPC(currentActiveInteractions);
        }
    }
}

public class NPCDialogHandler : MonoBehaviour
{
    /*
     This class is possibly not required, as we may want to place the logic for determining the 
     
     
     */
    [SerializeField] NPCInteractionSet PossibleInteractions;

    public bool TryGetInteraction(NPCBase NPCData, out Interaction[] foundSet)
    {
        foundSet = null;
        return false;
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
