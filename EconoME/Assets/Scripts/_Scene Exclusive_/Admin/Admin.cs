using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Admin
{
    internal class Admin : MonoBehaviour
    {
        [SerializeField] ItemBase ItemToSpawn;
        [SerializeField] int ItemToSpawnStacksize;
        [SerializeField] Vector3Variable PlayerPosition;

        [SerializeField] PlayerInput AdminInput;
        InputAction _keyboardC;
        InputAction _keyboardDot;
        [SerializeField] bool Enabled;

        private void Start()
        {
            _keyboardC = AdminInput.actions["KeyboardC"];
            _keyboardDot = AdminInput.actions["KeyboardDot"];
        }

        private void Update()
        {
            if (!Enabled) { return; }
            if (!_keyboardC.triggered) { return; }
            if (_keyboardDot.triggered)
                SpawnItem();


        }

        public void SpawnItem()
        {
            if (ItemToSpawn == null) { return; }
            GroundItemManager.Instance.SpawnItem(ItemToSpawn.CreateItem(ItemToSpawnStacksize), PlayerPosition.Value, out _);
        }

    }

}



