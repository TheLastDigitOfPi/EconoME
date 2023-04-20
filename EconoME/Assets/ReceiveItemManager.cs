using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveItemManager : MonoBehaviour
{ 
    Queue<Item> itemsQueue = new Queue<Item>();
    public static ReceiveItemManager Instance;

    public event Action OnItemReceived;
    [SerializeField] ItemDisplayer _itemDisplayerPrefab;
    bool _canDisplayNextItem = true;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 instance of Recieve Item manager found");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    public static void ReceiveItem(DefinedScriptableItem item)
    {
        Instance.itemsQueue.Enqueue(item.CreateItem());
    }

    private void Update()
    {
        if(itemsQueue.Count <= 0 || !_canDisplayNextItem)
            return;

        _canDisplayNextItem = false;
        var displayer = Instantiate(_itemDisplayerPrefab);
        displayer.Initialize(itemsQueue.Dequeue());
        WorldTimeManager.Instance.StopTime();
        displayer.OnComplete += FinishedGettingItem;
    }

    private void FinishedGettingItem()
    {
        _canDisplayNextItem = true;
        OnItemReceived?.Invoke();
        WorldTimeManager.Instance.ResumeTime();
    }
}
