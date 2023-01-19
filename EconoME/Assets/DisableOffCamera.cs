using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOffCamera : MonoBehaviour
{

    List<Collider2D> _allColliders = new();


    private void Start()
    {
        var colliders = GetComponentsInChildren<Collider2D>();

        foreach (var collider in colliders)
            _allColliders.Add(collider);

        colliders = GetComponents<Collider2D>();

        foreach (var collider in colliders)
            _allColliders.Add(collider);

        foreach (var item in _allColliders)
        {
            item.enabled = false;
        }
    }

    private void OnBecameInvisible()
    {
#if UNITY_EDITOR
        if (Camera.current != null ? Camera.current.name == "SceneCamera" : false)
            return;
#endif
        foreach (var item in _allColliders)
        {
            
            item.enabled = false;
        }
    }

    private void OnBecameVisible()
    {
#if UNITY_EDITOR
        if (Camera.current != null ? Camera.current.name == "SceneCamera" : false)
            return;
#endif

        foreach (var item in _allColliders)
        {
            item.enabled = true;
        }
    }

}
