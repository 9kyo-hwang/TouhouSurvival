using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unchord
{
    [System.Serializable]
    public class ShopSaveData
    {
        public int gold;
        public int investedPoints;
        public int exchangedPoints;

        [System.Serializable]
        public class ItemLevelData
        {
            public string attributeType;
            public int level;

            public ItemLevelData(string attributeType, int level)
            {
                this.attributeType = attributeType;
                this.level = level;
            }
        }

        public List<ItemLevelData> itemLevels = new List<ItemLevelData>();

        public void Update(string attributeType, int level)
        {
            foreach(ItemLevelData itemLevel in itemLevels)
            {
                if(itemLevel.attributeType == attributeType)
                {
                    itemLevel.level = level;
                    return;
                }
            }

            itemLevels.Add(new ItemLevelData(attributeType, level));
        }

        public bool TryGetItemLevel(string attributeType, out int level)
        {
            foreach(ItemLevelData itemLevel in itemLevels)
            {
                if(itemLevel.attributeType == attributeType)
                {
                    level = itemLevel.level;
                    return true;
                }
            }

            level = 0;
            return false;
        }

        public void ClearItemLevels()
        {
            itemLevels.Clear();
        }
    }

    public class GameData
    {
        private const int GAME_DATA_FILE_VERSION = 0;

        public static GameData Instance
        {
            get
            {
                lock (s_lock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new GameData();
                        s_instance.Load();
                    }

                    return s_instance;
                }
            }
        }

        private static GameData s_instance;
        private static object s_lock = new object();

        #region Save Fields
        // TODO: 저장하고 싶은 데이터를 Save Fields Region에 public으로 선언합니다.
        public int gameDataFileVersion = GameData.GAME_DATA_FILE_VERSION;

        public float totalAbsolutePlaytime;
        public float totalElapsedPlaytime;
        public int totalGamePlayCount;
        public int totalGamePlaySuccessCount;
        public int totalGamePlayFailureCount;

        public int totalKillCount;

        public ShopSaveData shopSaveData = new ShopSaveData();
        public int Gold
        {
            get => shopSaveData.gold;
            set => shopSaveData.gold = value;
        }

        #endregion

        public static string GetPersistentDataFilePath()
        {
            return Application.persistentDataPath + "/touhousaves.json";
        }

        public void Load()
        {
            string path = GameData.GetPersistentDataFilePath();

            if (!File.Exists(path))
            {
                string json = JsonUtility.ToJson(this);
                FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                StreamWriter wr = new StreamWriter(fs);
                wr.Write(json);
                wr.Close();
                fs.Close();
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader rd = new StreamReader(fs);
                string json = rd.ReadToEnd();
                rd.Close();
                fs.Close();
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }

        public void Save()
        {
            string path = GameData.GetPersistentDataFilePath();

            string json = JsonUtility.ToJson(this);
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter wr = new StreamWriter(fs);
            wr.Write(json);
            wr.Close();
            fs.Close();
        }
    }
}