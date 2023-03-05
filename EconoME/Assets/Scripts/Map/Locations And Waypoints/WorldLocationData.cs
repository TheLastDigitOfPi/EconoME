using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Represents a Location on the map. This location can have multiple waypoints that NPC's can chose from.
/// </summary>
[CreateAssetMenu(fileName = "New World Location", menuName = "ScriptableObjects/Map/Location")]
public class WorldLocationData : ScriptableObject
{
    /// <summary>
    /// The scene this transitioner transition to
    /// </summary>
    public WorldLocationData LocationConnectedTo { get; private set; }
    [field: SerializeField] public int SceneIndex { get; private set; } = 0;
    List<NPC> NpcsAtLocation { get; set; } = new();
    [field: SerializeField] public List<WorldWayPointData> LocationWayPoints { get; private set; } = new();
    SceneTransitionerData _transitionWaypoint;


    public void Initialize(WorldLocation location, WorldWayPointData[] waypoints)
    {
        LocationWayPoints.Clear();
        LocationWayPoints.AddRange(waypoints);
        SceneIndex = location.gameObject.scene.buildIndex;
        if (location.Optional_Transitioner != null)
        {
            LocationConnectedTo = location.Optional_Transitioner.LocationToTransitionTo;
            _transitionWaypoint = location.Optional_Transitioner.Initialize(this);
        }
    }
    public WorldWayPointData GetWayPoint()
    {
        return LocationWayPoints.Count > 0 ? LocationWayPoints[0] : null;
    }

    public bool TryGetTransitionWaypoint(out WorldWayPointData waypoint)
    {
        waypoint = _transitionWaypoint?.TransitionerEntrance;
        return _transitionWaypoint != null;
    }
}
