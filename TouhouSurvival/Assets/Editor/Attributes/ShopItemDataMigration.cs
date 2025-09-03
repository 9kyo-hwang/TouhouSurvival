using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unchord.Editor
{   
    public class ShopItemDataMigration
    {
        private const string shelfFileRelativePath = "/Shop/Shelves/shelf_0.multicsv";

        [MenuItem("Touhou/Data Migration/Run Shop Item Migration")]
        public static void Migrate()
        {
            string shelfFilePath = Application.streamingAssetsPath + shelfFileRelativePath;
            List<SerializedShopItem> items;

            try
            {
                using (FileStream stream = new FileStream(shelfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (MultiCSVReader reader = new MultiCSVReader(stream))
                {
                    if (!reader.TryParseTable(out items, "ShopItemShelf"))
                    {
                        Debug.LogError($"Failed to parse ShopItemShelf from {shelfFilePath}");
                        return;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.LogError($"Shelf file not found: {shelfFilePath}");
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured while reading shelf file {shelfFilePath}: {e.Message}");
                return;
            }

            foreach(var item in items)
            {
                List<SerializedGameplayAttributeModifier> modifiers = ParseModifiersFromFile(item.itemPath);
                if(modifiers == null)
                {
                    Debug.LogWarning($"Could not parse modifiers for '{item.itemName}'. Skipping asset creation.");
                    continue;
                }

                string assetPath = $"Assets/Resources/GUIs/ScriptableObjects/{item.itemName}.asset";
                ShopItemDataSO so = ScriptableObject.CreateInstance<ShopItemDataSO>();

                so.attributeType = item.itemName;
                so.alias = item.itemName;
                so.title = item.itemName;
                so.modifiers = modifiers;

                AssetDatabase.CreateAsset(so, assetPath);
                Debug.Log($"Created ShopItemDataSO at: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Data migration complete!");
        }

        private static List<SerializedGameplayAttributeModifier> ParseModifiersFromFile(string itemRelativePath)
        {
            string filePath = Application.streamingAssetsPath + itemRelativePath;
            if(!File.Exists(filePath))
            {
                Debug.LogError($"Modifier file not found at: {filePath}");
                return null;
            }

            List<SerializedGameplayAttributeModifier> modifiers = null;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (MultiCSVReader reader = new MultiCSVReader(stream))
                {
                    if (!reader.TryParseTable(out modifiers, "ModifierTable"))
                    {
                        Debug.LogWarning($"Failed to parse ModifierTable from: {filePath}");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured while parsing modifier table from {filePath}: {e.Message}");
                return null;
            }

            return modifiers;
        }
    }
}