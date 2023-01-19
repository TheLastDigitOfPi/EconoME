using UnityEngine;


[CreateAssetMenu(fileName ="New Type Container", menuName = "ScriptableObjects/Economy/Items/BaseItems/_Enums/Item Type Container")]
public class ItemTypeContainer : ScriptableObject
{
    [SerializeField] ItemType[] _types;
    public ItemType[] Types {get{return _types;}}
}