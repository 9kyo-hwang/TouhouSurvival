using System;

namespace Unchord
{
    public class LevelUpEventArgs : EventArgs
    {
        public int PreviousLevel { get; private set; }
        public int CurrentLevel { get; private set; }

        public LevelUpEventArgs(int previousLevel, int currentLevel)
        {
            PreviousLevel = previousLevel;
            CurrentLevel = currentLevel;
        }
    }
}