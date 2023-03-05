using System;
using UnityEngine;

[Serializable]
 public class RelationshipOption
    {
        [field: SerializeField] public NPCRelationshipStatus Status { get; private set; }
        [Tooltip("The range in percent decimal that this status will be a part of. X represnets min value and Y represents max value Set No Start/End Value to true if on one of the ends (min status, max status)")]
        [field: SerializeField] public Vector2 percentRange { get; private set; }
        [field: SerializeField] public bool NoMaxValue { get; private set; }
        [field: SerializeField] public bool NoMinValue { get; private set; }

    }

