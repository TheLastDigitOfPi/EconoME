using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class WorldLocation : MonoBehaviour
{
    [SerializeField] WorldLocationData location;
    [field:Tooltip("Transitioner for where the player/npc will spawn when entering the location from another scene")]
    [field: SerializeField] public SceneTransitioner Optional_Transitioner { get; private set; }
    private void Start()
    {
        var waypoints = GetComponentsInChildren<WorldWayPoint>();
        var waypointData = waypoints.Select((w) => w.WaypointData(location)).ToArray();

        location.Initialize(this, waypointData);
    }
}