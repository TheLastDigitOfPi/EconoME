using UnityEngine;
using System.Linq;

public class WorldLocation : MonoBehaviour
{
    [SerializeField] WorldLocationData location;
    [field:Tooltip("Transitioner for where the player/npc will spawn when entering the location from another scene")]
    [field: SerializeField] public SceneTransitioner Optional_transitioner { get; private set; }
    private void Start()
    {
        var waypoints = GetComponentsInChildren<WorldWayPoint>();
        var waypointData = waypoints.Select((w) => w.WaypointData(location)).ToArray();

        location.Initialize(this, waypointData);
    }
}