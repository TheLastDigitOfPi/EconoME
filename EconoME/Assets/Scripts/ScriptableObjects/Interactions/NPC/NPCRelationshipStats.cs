using System;
using System.Collections.Generic;
using UnityEngine;

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

