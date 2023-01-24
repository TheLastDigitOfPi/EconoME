using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 This class origianlly was used to animate the player model in the inventory
but has since been abandoned. 
 
 
 
 */
public class UIAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer SR;
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = SR.sprite;
    }
    private void FixedUpdate()
    {
        if (image.sprite != SR.sprite)
        {
            image.sprite = SR.sprite;
        }

    }
    
    
}
