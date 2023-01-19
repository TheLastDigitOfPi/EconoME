using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Admin
{
    internal class Admin : MonoBehaviour
    {
        [SerializeField] ItemScriptableObject ItemToSpawn;
        [SerializeField] int ItemToSpawnStacksize;
        [SerializeField] Vector3Variable PlayerPosition;

        [SerializeField] bool Enabled;
        private void Update()
        {
            if (!Enabled) { return; }
            if (!Input.GetKey(KeyCode.Period)) { return; }

            if (Input.GetKeyDown(KeyCode.C))
            {
                SpawnItem();
            }

        }

        public void SpawnItem()
        {
            if (ItemToSpawn == null) { return; }
            ItemSpawner.Instance.SpawnItem(new Item(ItemToSpawn, ItemToSpawnStacksize), PlayerPosition.Value);
        }

    }
}
