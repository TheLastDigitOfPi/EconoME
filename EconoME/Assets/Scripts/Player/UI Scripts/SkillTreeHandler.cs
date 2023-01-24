using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SkillTreeHandler : MonoBehaviour
{
    [SerializeField] SkillNodeHandler[] SkillNodes;
    public static SkillTreeHandler Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Attempted to make more than 1 instance of SkillTreeHandler");
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    public bool AreNodesUnlocked(List<int> RequiredNodes)
    {
        for (int i = 0; i < RequiredNodes.Count; i++)
        {
            try
            {
                if (SkillNodes[RequiredNodes[i]] == null) { return false; }
            }
            catch (Exception)
            {
                Debug.LogWarning("Tried to get node with ID that greater than array size");
                return false;
            }

        }
        return true;
    }

}
