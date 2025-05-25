using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unchord
{
    public class PlayerLevelSystem
    {
        private const string ExperienceTablePath = "/Players/ExperienceTable.CSV";

        public GameplayAttribute ExpGainIncreaseAttribute { get; set; }

        private float ExpIncrement => ExpGainIncreaseAttribute?.CurrentValue ?? 0.0f;
        
        private Dictionary<int, float> _experienceTable;    // level -> level + 1 필요 경험치
        
        public event EventHandler<LevelUpEventArgs> OnLevelUp;
        public event EventHandler<ExperienceChangedEventArgs> OnExperienceChanged;
        
        private int _level;
        private bool IsMaxLevel => _level > _experienceTable.Count;
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

                return _experienceTable[Level];
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
                    float required = _experienceTable[Level];
                    if (remain < required)
                    {
                        _experience = remain;
                        OnExperienceChanged?.Invoke(this, new ExperienceChangedEventArgs(prev, _experience, _experienceTable[Level]));
                        break;
                    }
                    
                    remain -= required;
                    _experience = remain;
                    OnExperienceChanged?.Invoke(this, new ExperienceChangedEventArgs(prev, _experience, _experienceTable[Level]));
                    prev = _experience;
                    
                    Level += 1;
                }
            }
        }

        public PlayerLevelSystem()
        {
            LoadExperienceTable();
        }

        // TODO: 이 곳에 Xlsx 파일을 Csv로 Convert하고 경험치 테이블을 얻는 코드를 작성합니다.
        //public static PlayerLevelSystem LoadFromFile(string xlsxAssetPathRelative) { }

        private void LoadExperienceTable()
        {
            _experienceTable = new Dictionary<int, float>();
            
            string path = Application.streamingAssetsPath + ExperienceTablePath;
            if (!File.Exists(path))
            {
                Debug.LogError($"Could not load experience table csv file from {path}");
                return;
            }
            
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new StreamReader(stream);
            while (reader.ReadLine() is { } line)
            {
                string[] tokens = line.Split(',');
                if (!string.IsNullOrEmpty(tokens[0]))
                {
                    _experienceTable.Add(int.Parse(tokens[0]), float.Parse(tokens[1]));
                }
            }
            reader.Close();
            
            // First Element: Level 1 / Experience 0
            Level = 1;
            Experience = 0;
        }
    }
}