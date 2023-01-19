using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data Object", menuName = "ScriptableObjects/Player/Player Data")]
public class PlayerDataObject : ScriptableObject
{
    public SaveableData data = new();
}

