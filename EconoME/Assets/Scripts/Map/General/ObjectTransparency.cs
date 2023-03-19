using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransparency : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            SpriteRenderer _renderer;
            if (collision.gameObject.TryGetComponent(out _renderer))
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.6f);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            SpriteRenderer _renderer;
            if (collision.gameObject.TryGetComponent(out _renderer))
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.6f);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            SpriteRenderer _renderer;
            if (collision.gameObject.TryGetComponent(out _renderer))
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 1);
            }
        }
    }
}
