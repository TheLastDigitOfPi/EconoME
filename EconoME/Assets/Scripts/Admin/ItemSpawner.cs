using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ItemSpawner : MonoBehaviour
{
    [SerializeField] WorldItemHandler Prefab;
    [SerializeField] GameObject WorldItemParentObject;

    public static ItemSpawner Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnItem(Item item, Vector3 position)
    {
        var handler = Instantiate(Prefab, transform);
        handler.data.WorldPos = position;
        handler.data.item = item.Duplicate();
        handler.updateImage();
        handler.updateText();
        handler.transform.position = position;

    }

   
}

