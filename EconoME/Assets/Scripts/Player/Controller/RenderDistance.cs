using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    // Render Distance Script. Should Enable/Disable Objects that are in view


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("PlacedTile"))
        {
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlacedTile"))
        {

            if (collision.transform.GetComponent<WorldTileHandler>().data.Shrink == 0)
            {
                collision.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlacedTile"))
        {
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

}
