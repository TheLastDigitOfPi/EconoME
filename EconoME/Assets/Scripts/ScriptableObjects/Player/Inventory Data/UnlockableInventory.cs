using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unlockable Inventory", menuName = "ScriptableObjects/Player/Inventory/UnlockedInventory")]
public class UnlockableInventory : ScriptableObject
{
    [field: SerializeField] public bool IsUnlocked { get; private set; }
    public event Action<UnlockableInventory> OnUnlock;
    public TabSide TabSide;
    [field: SerializeField] public Sprite TabSprite {get; private set;}
    public BookPage ConnectedPage;

    [ContextMenu("Unlock Inventory")]
    public void UnlockInventory()
    {
        IsUnlocked = true;
        OnUnlock?.Invoke(this);
    }
}

public enum TabSide
{
    Left,
    Right
}

