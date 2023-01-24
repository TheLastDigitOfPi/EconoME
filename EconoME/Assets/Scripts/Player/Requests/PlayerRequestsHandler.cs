using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// The Requests handler manages the requests the player has recieved from NPCs.
/// It will be accessed by various scripts such as one that will display the requests in the adventure book
/// </summary>
public class PlayerRequestsHandler : MonoBehaviour
{
    //Static
    public static PlayerRequestsHandler Instance;
    //Local fields
    List<NPCRequest> activeRequests;
    List<NPCRequest> completedRequests;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("More than 1 Player requests handler found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }
}

public class NPCRequest
{

}

