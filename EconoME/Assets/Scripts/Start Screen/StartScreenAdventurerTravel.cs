using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class StartScreenAdventurerTravel : MonoBehaviour
{


    [SerializeField] List<Transform> _waypoints;
    Animator _animator;
    NavMeshAgent _pathfinder;
    SpriteRenderer _spriteRenderer;
    int _currentWaypoint;
    // Start is called before the first frame update
    void Start()
    {
        _pathfinder = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(DropIn());


    }

    IEnumerator DropIn()
    {
        _pathfinder.isStopped = true;
        _pathfinder.enabled = false;
        var startPos = transform.position.y;
        transform.position += (Vector3.up * 20);
        yield return new WaitForSeconds(2f);
        transform.DOMoveY(startPos, 2f).SetEase(Ease.InOutSine).onComplete += () =>
        {
            _pathfinder.enabled = true;
            StartCoroutine(FindNewPath());
        };
    }

    private void FixedUpdate()
    {
        if (!_pathfinder.enabled || _pathfinder.isStopped)
            return;
        if (_pathfinder.remainingDistance < _pathfinder.stoppingDistance + 0.05f)
        {
            StartCoroutine(FindNewPath());
        }
    }

    IEnumerator FindNewPath()
    {
        _pathfinder.isStopped = true;
        _animator.CrossFade("Idle", 0);
        yield return new WaitForSeconds(Random.Range(3, 8));
        _animator.CrossFade("Movement", 0);
        int lastWaypoint = _currentWaypoint;
        _currentWaypoint = -1;
        Vector3 newPos = Vector3.zero;
        do
        {
            if (_waypoints.Count < 2)
                break;
            while (_currentWaypoint == -1)
            {
                int randIndex = Random.Range(0, _waypoints.Count);
                if (randIndex == lastWaypoint)
                    continue;
                newPos = _waypoints[randIndex].transform.position;
                _currentWaypoint = randIndex;
                break;
            }
            _pathfinder.isStopped = false;
            _pathfinder.SetDestination(newPos);
            if (newPos.x > transform.position.x)
            {
                _spriteRenderer.flipX = false;
                break;
            }
            _spriteRenderer.flipX = transform;
        } while (false);

    }


}
