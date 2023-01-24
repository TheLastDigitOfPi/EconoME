using UnityEngine;
//Calls to waypointdata to set on start/awake or whatever

[RequireComponent(typeof(Collider2D))]
public class WorldWayPoint : MonoBehaviour
{
    /// <summary>
    /// The Location that this waypoint is attached to. Represents global data such as who is there and what scene it is in
    /// </summary>
    public Collider2D WayPointCollider { get; private set; }
    public WorldWayPointData WaypointData(WorldLocationData location)
    {
        return new WorldWayPointData(this, location);
    }

    private void Awake()
    {
        WayPointCollider = GetComponent<Collider2D>();
    }
}
