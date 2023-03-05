using System;
using UnityEngine;
[Serializable]
public class SceneTransitionerData
{
    [field:SerializeField] public WorldLocationData LocationToTransitionTo { get; private set;}
    [field:SerializeField] public WorldLocationData LocationToTransitionFrom { get; private set; }
    [field:SerializeField] public WorldWayPointData TransitionerEntrance { get; private set; }
    public int TransitionFromSceneIndex { get { return LocationToTransitionFrom.SceneIndex; } }
    public int TransitionToSceneIndex { get { return LocationToTransitionTo.SceneIndex; } }

    public SceneTransitionerData(WorldLocationData fromLocaiton, SceneTransitioner transitioner)
    {
        LocationToTransitionTo = transitioner.LocationToTransitionTo;
        LocationToTransitionFrom = fromLocaiton;
        TransitionerEntrance = new WorldWayPointData(transitioner.TransitionerEntrance, fromLocaiton);
    }
}