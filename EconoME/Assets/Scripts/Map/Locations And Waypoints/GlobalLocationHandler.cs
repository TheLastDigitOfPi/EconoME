using System.Collections.Generic;
using UnityEngine;

public class GlobalLocationHandler : MonoBehaviour
{
    //Static instance
    public static GlobalLocationHandler Instance;

    //public fields
    [field: SerializeField] public List<SceneTransitionerData> GlobalTransitions { get; private set; } = new();
    [field: SerializeField] public List<WorldLocationData> WorldLocations { get; private set; } = new();

    //Admin Testing
    [SerializeField] WorldLocationData TestGoToLocaiton;
    [SerializeField] WorldLocationData PlayerLocation;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 global location handler found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public static void AddTransitioner(SceneTransitionerData transitioner)
    {
        Instance.GlobalTransitions.Add(transitioner);
    }

    [ContextMenu("Test Location Distance Finder")]
    public void TestDistanceFinder()
    {
        Debug.Log("Starting Test Distance Finder");
        if (TestGoToLocaiton == null)
        {
            Debug.Log("Did not set a location to go to");
            return;
        }
        if (PlayerLocation == null)
        {
            Debug.Log("Did not set a player location");
            return;
        }

        if (!TestGoToLocaiton.TryGetTransitionWaypoint(out var goToWaypoint))
        {
            Debug.Log("Could not get a waypoint from the location, does it have a transitioner?");
            return;
        }

        Debug.Log("Attempting to get distances from player to the desired location");

        List<float> distances = CalculateDistanceToLocation(PlayerLocation, PlayerMovementController.Instance.PlayerPosition.Value, goToWaypoint, out List<int> ScenesGoneThrough);

        string distancesLog = "Distances Found: ";
        string scenesLog = "Scenes Found: ";
        foreach (var distance in distances)
        {
            distancesLog += $"{distance}, ";
        }
        foreach (var sceneNum in ScenesGoneThrough)
        {
            scenesLog += $"{sceneNum}, ";
        }
        Debug.Log(scenesLog);
        Debug.Log(distancesLog);
    }

    public List<float> CalculateDistanceToLocation(WorldLocationData startingLocation, Vector3 startingPosition, WorldWayPointData destination, out List<int> ScenesToGoThrough)
    {
        int finalSceneIndex = destination.Location.SceneIndex;
        
        ScenesToGoThrough = new();

        /* 
            The information I need to know:
            I need to know where I currently am (scene and position)
            I need to know the location that I am going to,
            I need to know the final waypoint I am going to
        */

        /* 
         * Start at a spot
         * Search all the spots connections. If any are our final scene, we have made it
         * If not, mark the scene as checked then go in all the spots connections and repeat last step
         * 
         * 
        */


        bool foundDistance = false;
        List<int> scenesChecked = new();
        List<float> sceneDistances = new();
        //Make temp waypoint data for our current position
        WorldWayPointData startingWaypoint = new(startingLocation, startingPosition);
        recursiveDistanceFinder(startingWaypoint, scenesChecked, sceneDistances);
        ScenesToGoThrough = scenesChecked;
        return sceneDistances;
        bool recursiveDistanceFinder(WorldWayPointData currentSpot, List<int> listOfScenesChecked, List<float> distanceToEachScene)
        {
            
            int currentSceneIndex = currentSpot.Location.SceneIndex;
            List<int> scenesCheckedCopy = new();
            scenesCheckedCopy.AddRange(listOfScenesChecked);
            List<float> distanceTrackerCopy = new();
            distanceTrackerCopy.AddRange(distanceToEachScene);

            //Add the scene we are currently checking to our checked scenes
            scenesCheckedCopy.Add(currentSpot.Location.SceneIndex);

            //If we are in the same scene, just calculate the distance between the current pos and waypoint
            if (currentSceneIndex == finalSceneIndex)
            {
                distanceTrackerCopy.Add(Vector2.Distance(destination.WayPointWorldPosition, startingPosition));
                sceneDistances = distanceTrackerCopy;
                scenesChecked = scenesCheckedCopy;
                return false;
            }

            //Get all connections on this scene
            var possibleConnections = GlobalTransitions.FindAll(t => t.TransitionFromSceneIndex == currentSpot.Location.SceneIndex);
            //Go through each connection to get its distance and if it connects to our desired scene
            foreach (var connection in possibleConnections)
            {
                if (foundDistance)
                {
                    if (scenesCheckedCopy.Count >= scenesCheckedCopy.Count)
                        return true;
                }
                //If we have already checked the location this connection goes to, go to the next one
                if (scenesCheckedCopy.Contains(connection.LocationToTransitionTo.SceneIndex))
                    continue;

                float connectionDistance = Mathf.Abs(Vector2.Distance(connection.TransitionerEntrance.WayPointWorldPosition, currentSpot.WayPointWorldPosition));
                distanceTrackerCopy.Add(connectionDistance);
                //if the connection is found return true
                if (connection.TransitionToSceneIndex == finalSceneIndex)
                {
                    foundDistance = true;
                    sceneDistances = distanceTrackerCopy;
                    scenesChecked = scenesCheckedCopy;
                    return true;
                }

                //Get new starting spot
                if (!connection.LocationToTransitionTo.TryGetTransitionWaypoint(out var newSpot))
                    return false;

                recursiveDistanceFinder(newSpot, scenesCheckedCopy, distanceTrackerCopy);
                distanceTrackerCopy.Remove(connectionDistance);
            }
            return false;


        }
    }
}
