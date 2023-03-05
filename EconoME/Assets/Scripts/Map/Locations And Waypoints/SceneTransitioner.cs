using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A point on the map where the NPC or player can transition to a new scene (i.e. enter/exit a house, cave, world, etc)
/// </summary>
public class SceneTransitioner : MonoBehaviour
{
    [field: SerializeField] public WorldLocationData LocationToTransitionTo { get; private set; }
    [field: SerializeField] public WorldWayPoint TransitionerEntrance { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GlobalSceneManager.Instance.TransitionPlayerToLocation(LocationToTransitionTo);
        }

    }

    public SceneTransitionerData Initialize(WorldLocationData startingLocation)
    {
        var data = new SceneTransitionerData(startingLocation, this);
        GlobalLocationHandler.AddTransitioner(data);
        return data;
    }
}
