using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class GroundItemManager : MonoBehaviour
{
    [SerializeField] WorldItemHandler Prefab;
    [SerializeField] Transform WorldItemParentObject;

    [SerializeField] List<WorldItemHandler> GroundItems = new();
    public static GroundItemManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public bool SpawnItem(Item item, Vector3 position, out WorldItemHandler handler)
    {
        handler = Instantiate(Prefab, WorldItemParentObject);
        handler.CreateItem(item, position);
        handler.transform.position = position;
        GroundItems.Add(handler);
        return true;
    }

    public void ItemRemoved(WorldItemHandler itemRemoved)
    {
        GroundItems.Remove(itemRemoved);
        Destroy(itemRemoved.gameObject);
    }

   
}

