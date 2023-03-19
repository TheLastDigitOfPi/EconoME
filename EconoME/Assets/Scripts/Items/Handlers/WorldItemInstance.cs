using UnityEngine;
using System;

[Serializable]
public class WorldItemInstance
{
    [SerializeReference] public DefinedScriptableItem itemPreset;
    [SerializeReference] public Item item;
    public Vector3 WorldPos;
}

