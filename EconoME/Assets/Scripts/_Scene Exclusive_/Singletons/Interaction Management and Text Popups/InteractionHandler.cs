using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;
using System.Text;

public class InteractionHandler : MonoBehaviour
{

    //Static
    public static InteractionHandler Instance;

    //Local
    Queue<Interaction> _queue { get; set; } = new();
    bool CurrentInteractionActive { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of Interaction Handler in scene");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        //If our current interaction is still active, do nothing
        if (CurrentInteractionActive)
        {
            return;
        }

        if (!_queue.TryDequeue(out Interaction nextInteraction))
            return;
        CurrentInteractionActive = true;
        nextInteraction.Activate(this);
        nextInteraction.OnInteractionEnd += InteractionComplete;
    }

    private void InteractionComplete()
    {
        CurrentInteractionActive = false;
    }

    public bool AddNew(InteractionSO newInteraction, out Interaction interactionMade)
    {
        Interaction[] interactions = _queue.ToArray();
        interactionMade = null;
        //Make sure interaction isn't already queued up
        for (int i = 0; i < interactions.Length; i++)
        {
            if (interactions[i].IsEqualTo(newInteraction)) { return false; }
        }
        interactionMade = newInteraction.GetInteraction();
        _queue.Enqueue(interactionMade);
        return true;
    }

    #region AdminTesting
    [Space(10)]
    [Header("Admin Testing")]
    [SerializeField] InteractionSO TestInteraction;

    public void TestTheInteraction()
    {
        _queue.Enqueue(TestInteraction.GetInteraction());
    }

    #endregion
}



[Serializable]
public class Option
{
    public string optionText;
    public UnityAction onSelect;
    public Option(string optionText)
    {
        this.optionText = optionText;
    }

}