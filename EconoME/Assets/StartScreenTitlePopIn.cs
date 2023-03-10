using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartScreenTitlePopIn : MonoBehaviour
{

    [SerializeField] List<Transform> _letters = new();
    [SerializeField] float _timePerCharacter;
    [SerializeField] float _dropInTime;
    [SerializeField] float _bridgeDelayTime;
    [SerializeField] float _bridgeStartDelayTime;
    [SerializeField] float _buttonsMoveInTime;
    [SerializeField] List<Transform> _bridges;
    [SerializeField] Transform _buttons;
    // Start is called before the first frame update
    void Start()
    {
        var _startYPos = _letters[0].transform.position.y;
        _buttons.gameObject.SetActive(false);
        foreach (var letter in _letters)
        {
            letter.position += (Vector3.up * 20);
        }

        foreach (var bridge in _bridges)
        {
            bridge.gameObject.SetActive(false);
        }

        StartCoroutine(TextPopIn());
        StartCoroutine(BridgePopIn());

        IEnumerator BridgePopIn()
        {
            yield return new WaitForSeconds(_bridgeStartDelayTime);
            foreach (var bridge in _bridges)
            {
                var bridgeEndYPos = bridge.position.y;
                bridge.position += (Vector3.up * 20);
                bridge.gameObject.SetActive(true);
                bridge.DOMoveY(bridgeEndYPos, _dropInTime).SetEase(Ease.InOutSine);

                yield return new WaitForSeconds(_bridgeDelayTime + _timePerCharacter);
            }
            _buttons.gameObject.SetActive(true);
            var finalPos = _buttons.position.y;
            _buttons.position += Vector3.down * 700;
            _buttons.DOMoveY(finalPos, _buttonsMoveInTime).SetEase(Ease.InOutSine);
        }

        IEnumerator TextPopIn()
        {

            foreach (var letter in _letters)
            {
                //Drop Letter
                letter.DOMoveY(_startYPos, _dropInTime).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(_timePerCharacter);

            }
        }
    }

}
