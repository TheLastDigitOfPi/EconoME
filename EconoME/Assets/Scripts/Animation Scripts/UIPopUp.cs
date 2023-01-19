using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/*
 Pops up UI "tabs" like the X button to close the UI when the player hovers over it
 Slowly migrating most UI's to be in the book, but script may be usefull in future
 
 TODO - If using in future replace custom movement with DOTween movement
 
 */
public class UIPopUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool moving;
    [SerializeField]
    float TimetoMove;
    [SerializeField]
    float MoveAmount;
    RectTransform RT;
    float moved = 0;
    bool movingUp;
    public int Direction = 0;

    
    private void Awake()
    {
        RT = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
       
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(MoveUiUp());
        IEnumerator MoveUiUp()
        {
            movingUp = true;

            while (moved < MoveAmount)
            {
                if (movingUp)
                {
                    if (Direction == 0)
                    {
                        RT.position += new Vector3(0, MoveAmount / TimetoMove * 0.15f, 0);
                        moved += MoveAmount / TimetoMove;
                        yield return new WaitForSeconds(0.09f);

                    }
                    else if (Direction == 1)
                    {
                        RT.position += new Vector3(MoveAmount / TimetoMove * 0.15f, 0, 0);
                        moved += MoveAmount / TimetoMove;
                        yield return new WaitForSeconds(0.09f);
                    }

                }
                else
                {
                    break;
                }

            }

            movingUp = false;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        if (moved < 20)
        {
            return;
        }
        StartCoroutine(MoveUiUp());
        IEnumerator MoveUiUp()
        {
            movingUp = false;

            while (moved > 0)
            {
                if (!movingUp)
                {
                    if (Direction == 0)
                    {
                        RT.position -= new Vector3(0, MoveAmount / TimetoMove * 0.15f, 0);
                        moved -= MoveAmount / TimetoMove;
                        yield return new WaitForSeconds(0.09f);
                    }
                    else if (Direction == 1)
                    {
                        RT.position -= new Vector3(MoveAmount / TimetoMove * 0.15f, 0, 0);
                        moved -= MoveAmount / TimetoMove;
                        yield return new WaitForSeconds(0.09f);
                    }

                }
                else
                {
                    break;
                }

            }
        }
    }

    public void ResetPos()
    {
        movingUp = false;
        while (moved > 0)
        {
            if (Direction == 0)
            {
                RT.position -= new Vector3(0, MoveAmount / TimetoMove * 0.15f, 0);
                moved -= MoveAmount / TimetoMove;
            }
            else if (Direction == 1)
            {
                RT.position -= new Vector3(MoveAmount / TimetoMove * 0.15f, 0, 0);
                moved -= MoveAmount / TimetoMove;
            }

        }
    }

   
}
