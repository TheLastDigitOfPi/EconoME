using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Object", menuName = "ScriptableObjects/Interactions/NPCs/NPC Data")]
[Serializable]
public class NPCBase : ScriptableObject
{
    [field: SerializeField] public int NPCID { get; private set; }
    [field: SerializeField] public string NPCName { get; private set; }

    [field: SerializeField][field: Multiline] public string Description { get; private set; }

    public NPCRelationshipStatus CurrentStatus { get { return _relationshipDeterminer.CalculateStatus(); } }

    [SerializeField] NPCRelationshipStats _relationshipDeterminer;
    [field: SerializeField] public NPCInteractionSet InteractionSet { get; private set; }



    [Serializable]
    public class NPCRelationshipStats
    {
        [SerializeField] List<RelationshipOption> RelationshipOptions = new();
        [SerializeField] List<RelationshipOption> InvalidOptions = new();

        [Tooltip("The current percentage range from base 0. Will be checked by options")]
        [SerializeField] float _currentValue;

        public NPCRelationshipStatus DoStatusChange(float changeAmount)
        {
            _currentValue -= changeAmount;
            return CalculateStatus();
        }

        public NPCRelationshipStatus CalculateStatus()
        {
            foreach (var option in RelationshipOptions)
            {
                if (option.NoMaxValue && _currentValue >= option.percentRange.x)
                    return option.Status;

                if (option.NoMaxValue && _currentValue <= option.percentRange.x)
                    return option.Status;

                if (_currentValue.isBetweenInclusive(option.percentRange.x, option.percentRange.y))
                    return option.Status;
            }
            return NPCRelationshipStatus.Neutral;
        }

        bool ValidateOptions()
        {
            bool foundInvalidOption = false;
            List<RelationshipOption> tempOptions = new();
            //Copy over list to temp list
            foreach (var option in RelationshipOptions)
            {
                tempOptions.Add(option);
            }
            //Check list for any options that contain the same status, remove duplicates
            foreach (var option in tempOptions)
            {
                foreach (var otherOption in tempOptions)
                {
                    if (option == otherOption)
                        continue;

                    if (option.Status == otherOption.Status)
                    {
                        InvalidOptions.Add(otherOption);
                        RelationshipOptions.Remove(otherOption);
                        tempOptions.Remove(otherOption);
                        foundInvalidOption = true;
                        continue;
                    }
                }

            }
            //Check list for any options that overlap their range for the status, remove offenders
            foreach (var option in tempOptions)
            {
                foreach (var otherOption in tempOptions)
                {
                    if (otherOption == option)
                        continue;

                    bool invalidX = otherOption.percentRange.x.isBetweenInclusive(option.percentRange.x, option.percentRange.y);
                    bool invalidY = otherOption.percentRange.y.isBetweenInclusive(option.percentRange.x, option.percentRange.y);
                    if (invalidX || invalidY)
                    {
                        InvalidOptions.Add(otherOption);
                        RelationshipOptions.Remove(otherOption);
                        tempOptions.Remove(otherOption);
                        foundInvalidOption = true;
                        continue;
                    }
                }
            }

            return !foundInvalidOption;
        }
    }

    public class RelationshipOption
    {
        [field: SerializeField] public NPCRelationshipStatus Status { get; private set; }
        [Tooltip("The range in percent decimal that this status will be a part of. X represnets min value and Y represents max value Set No Start/End Value to true if on one of the ends (min status, max status)")]
        [field: SerializeField] public Vector2 percentRange { get; private set; }
        [field: SerializeField] public bool NoMaxValue { get; private set; }
        [field: SerializeField] public bool NoMinValue { get; private set; }

    }

}

public enum NPCRelationshipStatus
{
    MortalEnemies,
    Enemies,
    Annoyed,
    Neutral,
    Friends,
    BestFriends,
    Lovers
}

public enum NPCRealtionshipOperation
{
    SamllDislike,
    SmallRequestFail,
    SmallRequestSuccess,
    MediumRequestFail,
    MediumRequestSuccess,
    LargeRequestFail,
    LargeRequestSuccess,
}


/// <summary>
/// The NPC Dialog Atlas will contain all the possible Dialogs that the NPC will be able to speak. It will also be able to take in the NPC's status and determine what
/// </summary>
public class NPCDialogAtlas : ScriptableObject
{

}

