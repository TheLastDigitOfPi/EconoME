using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WorldItemHandler : MonoBehaviour
{
    public WorldItemInstance data = new WorldItemInstance();
    Image image;
    TextMeshProUGUI TMP;
    private void Awake()
    {
        TMP = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        image = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        if(data.itemPreset != null)
        {
            data.item = data.itemPreset.CreateItem();
        }

        updateImage();
        updateText();
    }

    public void updateImage()
    {
        if (data.item == null)
        {
            Debug.LogWarning("Tried to update world item image but the item data was never instantiated");
            return;
        }
        if (data.item.Icon != null)
        {
            image.sprite = data.item.Icon;
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            return;
        }

        image.sprite = null;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

    }

    public void updateText()
    {
        //updateUI of text element on ground object

        if (data.item == null)
        {
            Debug.LogWarning("Tried to update world item text but the item data was never instantiated");
            return;
        }
        if (data.item.Stacksize <= 1)
        {
            TMP.text = "";
            return;
        }
        TMP.text = data.item.Stacksize.ToString();
    }

    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }

    internal void setInstanceData(WorldItemInstance other)
    {
        data = other;
        updateText();
        updateImage();
    }
}

[Serializable]
public class WorldItemInstance
{
    [SerializeReference] public DefinedScriptableItem itemPreset;
    [SerializeReference] public Item item;
    public Vector3 WorldPos;
    

}

