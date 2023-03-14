using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenCarrotBoy : MonoBehaviour
{

    Animator _animator;
    bool _hiding = true;
    [SerializeField] float _timeBeforeHiding;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.CrossFade("Hiding", 0);
    }

    private void OnMouseDown()
    {
        if (!_hiding)
            return;

        _animator.CrossFade("Appear", 0);
        _hiding = false;
        StartCoroutine(WaitAndHide());
    }

    IEnumerator WaitAndHide()
    {
        yield return new WaitForSeconds(_timeBeforeHiding);
        _animator.CrossFade("Hide", 0);
        _hiding = true;
    }

}
