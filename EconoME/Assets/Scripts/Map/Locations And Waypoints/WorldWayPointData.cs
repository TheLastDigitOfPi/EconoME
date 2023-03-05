using UnityEngine;

[System.Serializable]
public class WorldWayPointData
{
    /*
    Since the components are not gaurenteed to exist or be enabled, we must store the data for the component here at the start of the game
    We can either have a scriptable object hold the data or a class with constructors. Since this is data will be stored in another Scriptable object, just use a normal class.
    -- NOTE -- The data must be "copied" from the component since the component could be deleted /disabled at any time
    */

    //Helpers
    /// <summary>
    /// The Location that this waypoint is attached to. Represents global data such as who is there and what scene it is in
    /// </summary>
    public WorldLocationData Location { get; private set; }
    [field:SerializeField] public Vector3 WayPointWorldPosition { get; private set; }
    [field:SerializeField] public Bounds Bounds { get; private set; }

    public WorldWayPointData(WorldWayPoint waypoint, WorldLocationData location)
    {
        Location = location;
        Bounds = waypoint.WayPointCollider.bounds;
        WayPointWorldPosition = waypoint.transform.position;
    }

    public WorldWayPointData(WorldLocationData location, Vector3 Pos)
    {
        Location = location;
        WayPointWorldPosition = Pos;
    }
}
