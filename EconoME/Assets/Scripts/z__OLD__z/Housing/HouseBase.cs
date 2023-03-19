using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Housing Data", menuName = "ScriptableObjects/NPCs/Housing/House Data")]
public class HouseBase : ScriptableObject
{
    public Guid HouseID = Guid.NewGuid();
    [SerializeField] string _houseID;
    [field: SerializeField] public int SceneNum { get; private set; }
    private void OnValidate()
    {
        _houseID = HouseID.ToString();
    }

    public Action OnEnter;
    public Action OnExit;
}

