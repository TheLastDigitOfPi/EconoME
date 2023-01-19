using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using System;

[CreateAssetMenu(fileName = "New Float Variable", menuName = "ScriptableObjects/Variables/Data Values/Float")]
[System.Serializable]
public class FloatVariable : VariableBase<float>
{
}
