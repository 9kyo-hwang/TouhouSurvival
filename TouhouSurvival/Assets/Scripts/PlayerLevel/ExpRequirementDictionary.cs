using System;
using System.Collections.Generic;

namespace Unchord
{
    /// <summary>
    /// 현재 레벨에서 다음 레벨로 올라갈 때 필요한 경험치 요구량 사전입니다.
    /// </summary>
    public class ExpRequirementDictionary : Dictionary<int, float>
    {
        public GameplayAttribute ExpGainIncreaseAttribute { get; set; }

        private float ExpIncrement => ExpGainIncreaseAttribute?.CurrentValue ?? 0.0f;
        
        public event EventHandler<LevelUpEventArgs> OnLevelUp;
        public event EventHandler<ExperienceChangedEventArgs> OnExperienceChanged;
        
        private int _level;
        private bool IsMaxLevel => _level > base.Count;
        public int Level
        {
            get => _level;
            private set
            {
                if (value > 0 && value != _level && !IsMaxLevel)
                {
                    int prevLevel = _level;
                    _level = value;
                    OnLevelUp?.Invoke(this, new LevelUpEventArgs(prevLevel, _level));
                }
            }
        }
        
        private float _experience;

        public float TotalExperienceForCurrentLevel
        {
            get
            {
                if (Level == 0 || IsMaxLevel)
                {
                    return 1.0f;
                }

                return base[Level];
            }
        }
        
        public float Experience
        {
            get => _experience;
            set
            {
                float remain = value * (1.0f + ExpIncrement);
                float prev = _experience;
                while (!IsMaxLevel && remain >= 0)
                {
                    float required = base[Level];
                    if (remain < required)
                    {
                        _experience = remain;
                        OnExperienceChanged?.Invoke(this, new ExperienceChangedEventArgs(prev, _experience, base[Level]));
                        break;
                    }
                    
                    remain -= required;
                    _experience = remain;
                    OnExperienceChanged?.Invoke(this, new ExperienceChangedEventArgs(prev, _experience, base[Level]));
                    prev = _experience;
                    
                    Level += 1;
                }
            }
        }

        private ExpRequirementDictionary()
        : base(capacity: 32)
        {
            Level = 1;
            Experience = 0;
        }

        public ExpRequirementDictionary(MultiCSVReader reader, string aliasOrNull = null)
        : this()
        {
            List<SerializedLevelUpExp> expReqs;

            if (!reader.TryParseTable<SerializedLevelUpExp>(out expReqs, aliasOrNull))
            {
                UnityEngine.Debug.Assert(false, "Parsing SerializedLevelUpExp type failed.");
                return;
            }

            for (int i = 0; i < expReqs.Count; ++i)
            {
                UnityEngine.Debug.Assert(!base.ContainsKey(expReqs[i].currentLevel));
                UnityEngine.Debug.Assert(expReqs[i].expRequirement >= 0);

                base.Add(expReqs[i].currentLevel, expReqs[i].expRequirement);
            }
        }
    }
}