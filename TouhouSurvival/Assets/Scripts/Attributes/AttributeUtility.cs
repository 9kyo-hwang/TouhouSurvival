using System.IO;
using UnityEngine;

namespace Unchord
{
    public static class AttributeUtility
    {
        public static string[] ConvertXlsxToCsv(string xlsxAssetPathRelative)
        {
            Debug.Log($"[Debug] Input Relative Path: {xlsxAssetPathRelative}");
            string xlsxPath = Application.streamingAssetsPath + xlsxAssetPathRelative;
            Debug.Log($"[Debug] Full XLSL Path: {xlsxPath}");

            if(!File.Exists(xlsxPath))
            {
                Debug.LogError($"[Debug] XLSX File NOT FOUND at: {xlsxPath}");
                return new string[2];
            }

            string xlsxDir = Path.GetDirectoryName(xlsxPath);
            string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

            XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

            string[] arrCsvPath = new string[2];
            string csvBase = xlsxDir + $@"\{xlsxName}+base.csv";
            string csvMod = xlsxDir + $@"\{xlsxName}+modifiers.csv";

            arrCsvPath[0] = File.Exists(csvBase) ? csvBase : null;
            arrCsvPath[1] = File.Exists(csvMod) ? csvMod : null;

            return arrCsvPath;
        }
    }
}