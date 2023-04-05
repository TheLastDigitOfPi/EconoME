public interface IAmASlot
{
    public void UpdateSlot();
    public bool GrabItem(out Item ItemGrabbed);
    /// <summary>
    /// Attempts to add item to the slot
    /// </summary>
    /// <param name="ItemAdded">The Item to be added</param>
    /// <param name="PartialAddition">Returns true if some of the item was able to be added</param>
    /// <returns>Returns true if all items were able to be added</returns>
    public bool AddItem(Item ItemAdded, out bool PartialAddition);
    /// <summary>
    /// Removes an item from the slot
    /// </summary>
    /// <returns>Returns true if the slot allows the item to be removed</returns>
    public bool RemoveItem();
    /// <summary>
    /// Attempts to swap items and output the item in this slot. Outs null if failed
    /// </summary>
    /// <param name="ItemToSwap">The item to be placed in the slot</param>
    /// <param name="SwappedItem">The original item in the slot if successful, otherwise returns null</param>
    /// <returns>Returns true if able to swap item</returns>
    public bool SwapItem(Item ItemToSwap, out Item SwappedItem);
}
