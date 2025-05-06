using System;

namespace Unchord
{
    public class ExperienceChangedEventArgs : EventArgs
    {
        public float PreviousExperience { get; private set; }
        public float CurrentExperience { get; private set; }
        public float TotalExperience { get; private set; }

        public ExperienceChangedEventArgs(float previousExperience, float currentExperience, float totalExperience)
        {
            PreviousExperience = previousExperience;
            CurrentExperience = currentExperience;
            TotalExperience = totalExperience;
        }
    }
}