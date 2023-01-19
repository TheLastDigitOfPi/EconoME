using UnityEngine;
public partial class ArmorSriptableObject : ItemScriptableObject
{
    [SerializeField] ArmorType armorType;
    public override ItemType ItemType => armorType;
}

