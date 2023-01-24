using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Represents a Location on the map. This location can have multiple waypoints that NPC's can chose from.
/// </summary>
[CreateAssetMenu(fileName = "New World Location", menuName = "ScriptableObjects/Map/Location")]
public class WorldLocationData : ScriptableObject
{
    public Scene LocationScene { get { return _scene != null ? _scene : SceneManager.GetActiveScene(); } set { _scene = value; } }
    Scene _scene;

    [field: SerializeField] public int SceneIndex { get; private set; } = 0;
    List<NPC> NpcsAtLocation { get; set; } = new();
    [field: SerializeField] public List<WorldWayPointData> LocationWayPoints { get; private set; } = new();
    WorldWayPointData _transitionWaypoint;

    public void Initialize(WorldLocation location, WorldWayPointData[] waypoints)
    {
        LocationWayPoints.Clear();
        LocationWayPoints.AddRange(waypoints);
        LocationScene = location.gameObject.scene;
        SceneIndex = LocationScene.buildIndex;
        SceneTransitioner sceneTransitioner = location.Optional_transitioner;
        if (sceneTransitioner != null)
        {
            _transitionWaypoint = sceneTransitioner.TransitionerEntrance.WaypointData(this);
        }
    }
    public WorldWayPointData GetWayPoint()
    {
        return LocationWayPoints.Count > 0 ? LocationWayPoints[0] : null;
    }

    public bool TryGetTransitionWaypoint(out WorldWayPointData waypoint)
    {
        waypoint = _transitionWaypoint;
        if (_transitionWaypoint == null)
            return false;
        return true;
    }
}
