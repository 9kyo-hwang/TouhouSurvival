using System.IO;
using UnityEngine;

namespace Unchord
{
    public static class AttributeUtility
    {
        public static string[] ConvertXlsxToCsv(string xlsxAssetPathRelative)
        {
            string xlsxPath = Application.streamingAssetsPath + xlsxAssetPathRelative;
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