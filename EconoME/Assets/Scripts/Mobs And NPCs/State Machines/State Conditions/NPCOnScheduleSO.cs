using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




using UnityEngine;

[CreateAssetMenu(fileName = "New NPCOnSchedule", menuName = "ScriptableObjects/NPCs/AI/Conditions/NPCOnSchedule")]
public class NPCOnScheduleSO : AIStateConditionSO
{
    public override AIStateCondition GetCondition()
    {
        return new NPCOnSchedule();
    }
}

public class NPCOnSchedule : AIStateCondition
{
    
    public override bool CheckIfValid(AIController controller, out string Error)
    {
        Error = default;
        if(!controller.TryGetComponent(out NPC npc))
        {
            Error += "No NPC object to check schedule";
            return false;
        }

        var scheduler = npc.NPCScheduler;
        Condition = () => scheduler.CheckingSchedule;
        return true;
    }
}


