using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unchord
{
    public abstract class AttributeSet : MonoBehaviour
    {
        public string attributeAssetPath;

        public Dictionary<string, GameplayAttribute> Attributes { get; private set; }

        public GameplayAttribute this[string attributeType]
        {
            get => Attributes[attributeType];
            set => Attributes[attributeType] = value;
        }

        protected virtual void Awake()
        {
            Attributes = new Dictionary<string, GameplayAttribute>();

            this.Attributes = LoadAttributes(attributeAssetPath);
        }

        protected virtual void Start()
        {

        }

        private Dictionary<string, GameplayAttribute> LoadAttributes(string xlsxFilePath)
        {
            Dictionary<string, GameplayAttribute> attributes = new Dictionary<string, GameplayAttribute>();

            string xlsxPath = Application.streamingAssetsPath + xlsxFilePath;
            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

            using (FileStream fs = new FileStream(xlsxDir + $"\\{xlsxName}+attributes_base.csv", FileMode.Open, FileAccess.Read))
            using (StreamReader rd = new StreamReader(fs))
            {
                rd.ReadLine(); // NOTE: Ignore header line.

                while (!rd.EndOfStream)
                {
                    string[] tokens = rd.ReadLine().Split(",");

                    if (tokens[0].Equals(string.Empty))
                        continue;

                    attributes.Add(tokens[0], new GameplayAttribute(float.Parse(tokens[1])));
                }
            }

            return attributes;
        }
    }
}