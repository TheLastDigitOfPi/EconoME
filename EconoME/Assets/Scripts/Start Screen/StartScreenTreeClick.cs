using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenTreeClick : MonoBehaviour
{
    bool _broken = false;
    bool _hittingTree = false;
    int _hits;
    SpriteRenderer _spriteRenderer;
    [SerializeField] ParticleSystem _treeHitParticles;
    [SerializeField] ParticleSystem _treeBreakParticles;
    [SerializeField] int hitsTillBreak = 5;
    [SerializeField] float _timeBeforeAppearing = 10;
    [SerializeField] float _hitDelay = 0.3f;
    [SerializeField] Vector3 _particleOffset;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (_broken || _hittingTree)
            return;
        StartCoroutine(HitTree());
    }

    IEnumerator HitTree()
    {
        _hittingTree = true;
        _hits++;
        var particle = Instantiate(_treeHitParticles, transform);
        particle.transform.position += _particleOffset;
        if (_hits >= hitsTillBreak)
        {
            _broken = true;
            StartCoroutine(WaitThenRespawn());
        }
        yield return new WaitForSeconds(_hitDelay);
        _hittingTree = false;
    }

    IEnumerator WaitThenRespawn()
    {
        _spriteRenderer.color = Color.clear;
        yield return new WaitForSeconds(_timeBeforeAppearing);
        var particle = Instantiate(_treeBreakParticles, transform);
        particle.transform.position += _particleOffset;
        _spriteRenderer.color = Color.white;
        _broken = false;
        _hits = 0;
    }

}
