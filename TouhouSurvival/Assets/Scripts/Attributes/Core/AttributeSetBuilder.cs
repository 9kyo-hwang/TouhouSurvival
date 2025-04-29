using System.IO;
using UnityEngine;

namespace Unchord
{
    // TODO: 이 클래스를 삭제하면서 내부에 포함하고 있는 함수를 적절한 위치로 이동시켜야 합니다.
    //public class AttributeSetBuilder
    //{
    //    public static void LoadAttributes(AttributeSet attributeSet)
    //    {
    //        _LoadAttributes(attributeSet);
    //    }

    //    private static void _LoadAttributes(AttributeSet attribute)
    //    {
    //        string xlsxPath = Application.streamingAssetsPath + attribute.attributeAssetPath;
    //        string xlsxDir = Path.GetDirectoryName(xlsxPath);
    //        string xlsxName = Path.GetFileNameWithoutExtension(xlsxPath);

    //        XlsxToCsvConverter.Convert(xlsxDir, xlsxPath, xlsxName);

    //        using (FileStream fs = new FileStream(xlsxDir + $"\\{xlsxName}+attributes_base.csv", FileMode.Open, FileAccess.Read))
    //        using (StreamReader rd = new StreamReader(fs))
    //        {
    //            rd.ReadLine(); // NOTE: Ignore header line.

    //            while (!rd.EndOfStream)
    //            {
    //                string[] tokens = rd.ReadLine().Split(",");

    //                if (tokens[0].Equals(string.Empty))
    //                    continue;

    //                attribute.Attributes.Add(tokens[0], new GameplayAttribute(float.Parse(tokens[1])));
    //            }
    //        }

    //        using (FileStream fs = new FileStream(xlsxDir + $"\\{xlsxName}+attributes_growth.csv", FileMode.Open, FileAccess.Read))
    //        using (StreamReader rd = new StreamReader(fs))
    //        {
    //            rd.ReadLine(); // NOTE: Ignore header line.

    //            while (!rd.EndOfStream)
    //            {
    //                string[] tokens = rd.ReadLine().Split(",");

    //                if (tokens[0].Equals(string.Empty))
    //                    continue;

    //                LevelUpData levelUpData = new LevelUpData();
    //                levelUpData.attributeType = tokens[0];
    //                levelUpData.deltaValue = float.Parse(tokens[1]);
    //                levelUpData.expRequirement = float.Parse(tokens[3]);
    //                levelUpData.displayDescription = tokens[4];

    //                attribute.LevelUpData.Add(levelUpData);

    //                switch(tokens[2])
    //                {
    //                    case "Addition":
    //                    case "addition":
    //                    case "Add":
    //                    case "add":
    //                    case "+":
    //                        levelUpData.attributeOperation = AttributeOperation.Add;
    //                        break;

    //                    case "Multiply":
    //                    case "multiply":
    //                    case "Mul":
    //                    case "mul":
    //                    case "*":
    //                        levelUpData.attributeOperation = AttributeOperation.Multiply;
    //                        break;

    //                    case "Subtract":
    //                    case "subtract":
    //                    case "Sub":
    //                    case "sub":
    //                    case "-":
    //                        levelUpData.attributeOperation = AttributeOperation.Subtract;
    //                        break;

    //                    case "Divide":
    //                    case "divide":
    //                    case "Div":
    //                    case "div":
    //                    case "/":
    //                        levelUpData.attributeOperation = AttributeOperation.Divide;
    //                        break;
    //                }
    //            }
    //        }
    //    }
    //}
}