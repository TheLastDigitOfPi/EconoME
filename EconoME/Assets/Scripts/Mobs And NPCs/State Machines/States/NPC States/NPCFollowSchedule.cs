using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
This will be the pathing that the NPCs will attempt to follow

It should take in the NPC's current status and determine where it needs to go

NPC's will travel between waypoints. We should check when the NPC makes it to the waypoint to end the state or continue to the next waypoint

An NPC should have a final destination. The destination can possibly have a set of waypoints that should be met along the way
Maybe we define paths from destinations to each other and have them move along that path

NPCs will have a separate state where they are set to wander. Wandering should involve randomly moving to a new area, or attempting to visit common wandering destinations (Maybe NPCs will want to visit the town fountain or go wander a park.

It may be a good idea to create a seperate component on the NPC that handles most of the wandering logic. This state should just tell that component that we want to travel.
*/



/*

The NPC AI Controller is going to primarly do a few things, depnding on how much AI we want to give them

1 - The NPC will have a schedule that they desire to complete throughout each day. The NPC will go into the 'Schedule' state for the majority of their time

2 - If we want to add aditional AI to the NPCs, we can do so. For example, if I want the NPC to accompany the player for some time, we can create a travel with player AI State. The NPC will leave the Schedule state and do what we design it to.

The following are current AI options that we might want to implement

-Schedule (base)

-Follow player / travel with player. The NPC will travel close to the player and attempt to either mine resources for them, play them music, annoy them, fight mobs for them, heal them, etc.

-The NPCs may want to be able to defend themselves. Currently they are immortal god like beings, but if we want to change that we can. This AI will start up when a hostile mob is near them or attacking them. They will then go into an "attacking" state until the mob dies, the NPC dies, or it gets too far away

-Maybe we want the NPC's to be able to randomly bump into each other and show icons above their heads like they are talking.

Either way, the NPC should go back to it's schedule state when none of these other states are active.
 
*/

public class NPCFollowSchedule : AIState
{
    
    NPCScheduleHandler _scheduleHandler;
    NPC _npc;
    public NPCFollowSchedule(){ }
    public NPCFollowSchedule(AIController controller, NPCFollowSchedule other) : base(controller, other.AICondition)
    {
        if(!controller.TryGetComponent(out _npc))
        {
            FailedStateRequirements(this, "Unable to find NPC Component");
        }
        _scheduleHandler = _npc.NPCScheduler;
    }

    public override void OnEnter()
    {
        _scheduleHandler.ResumeSchedule();
    }

    public override void OnExit()
    {
        _scheduleHandler.StopSchedule();
    }

    public override void Tick()
    {
        _scheduleHandler.ScheduleTick();
    }
}

