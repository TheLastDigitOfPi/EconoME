using UnityEngine;
using System;

[Serializable]
public class ItemIcon
{
    [field:SerializeField] public Sprite Icon {get; private set;}
    [field:SerializeField] public Color IconColor {get; private set;} = Color.white;
}