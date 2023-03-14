using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartScreenDragonPeek : MonoBehaviour
{

    Animator _animator;
    SpriteRenderer _spriteRenderer;
    [SerializeField] Transform _offScreenPos;
    [SerializeField] Transform _onScreenPos;
    [SerializeField] float _timePerScreenPeek;
    [SerializeField] float _timeOnScreen;
    [SerializeField] float _transitionTime;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(PeekOnScreen());
        IEnumerator PeekOnScreen()
        {
            while (true)
            {
                yield return new WaitForSeconds(_timePerScreenPeek);
                _animator.CrossFade("Walk", 0);
                _spriteRenderer.flipX = true;
                transform.DOMove(_onScreenPos.position, _transitionTime).onComplete += () => { _animator.CrossFade("Idle", 0); };
                yield return new WaitForSeconds(_timeOnScreen);
                yield return new WaitForSeconds(_transitionTime);
                _animator.CrossFade("Walk", 0);
                _spriteRenderer.flipX = false;
                transform.DOMove(_offScreenPos.position, _transitionTime).onComplete += () => { _animator.CrossFade("Idle", 0); };
            }

        }
    }
}
