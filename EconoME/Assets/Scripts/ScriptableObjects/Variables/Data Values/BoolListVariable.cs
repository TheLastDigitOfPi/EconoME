using UnityEngine;

[CreateAssetMenu(fileName = "New Vector3Int Variable", menuName = "ScriptableObjects/Variables/Data Values/BoolList")]
public class BoolListVariable : ScriptableObject
{
    [SerializeField] BoolVariable[] Value;

    public bool TrueValueExists()
    {
        for (int i = 0; i < Value.Length; i++)
        {
            if(Value[i].Value)
            {
                return true;
            }
        }
        return false;
    }
}