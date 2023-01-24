using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A point on the map where the NPC or player can transition to a new scene (i.e. enter/exit a house, cave, world, etc)
/// </summary>
public class SceneTransitioner : MonoBehaviour
{
    /// <summary>
    /// The scene this transitioner transitions from
    /// </summary>
    public Scene SceneFrom { get { return gameObject.scene; } }
    /// <summary>
    /// The scene this transitioner transition to
    /// </summary>
    public Scene SceneTo { get { return LocationToTransitionTo.LocationScene; } }
    [field: SerializeField] public WorldLocationData LocationToTransitionTo { get; private set; }
    [field: SerializeField] public WorldWayPoint TransitionerEntrance { get; private set; }

    private void Start()
    {
        GlobalLocationHandler.AddTransitioner(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GlobalSceneManager.Instance.TransitionPlayerToLocation(LocationToTransitionTo);
        }

    }

}
