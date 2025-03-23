using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Unchord
{
    public class XlsxToCsvConverter
    {
        private string m_destDirectory;
        private string m_srcXlsxFilePath;

        private int m_sheetCount;
        private string[] m_xmlSheetFilePaths;
        private List<string> m_sheetNames;

        private string[] m_sharedStrings;

        private int m_offsetRow;
        private int m_offsetColumn;

        private string[,] m_csvTable;
        private int m_spanRow;
        private int m_spanColumn;
        private int m_itRow; // NOTE: Iterator of row for m_csvTable.
        private int m_itColumn; // NOTE: Iterator of column for m_csvTable.
        private string m_itType; // NOTE: Iterator of value's data type.

        private XlsxToCsvConverter()
        {

        }

        public static XlsxToCsvConverter Convert(string _destDirectory, string _srcXlsxFilePath, string _csvAliasOrNull)
        {
            System.Diagnostics.Debug.Assert(_destDirectory != null);
            System.Diagnostics.Debug.Assert(_srcXlsxFilePath != null);

            XlsxToCsvConverter converter = new XlsxToCsvConverter();

            converter.m_destDirectory = _destDirectory;
            converter.m_srcXlsxFilePath = _srcXlsxFilePath;

            string extractDirectory = _destDirectory + @"\extract";
            ZipFile.ExtractToDirectory(_srcXlsxFilePath, extractDirectory, true);

            m_ParseSharedString(converter, extractDirectory);
            m_ParseWorkbook(converter, extractDirectory, _csvAliasOrNull);

            for (int i = 0; i < converter.m_sheetCount; ++i)
            {
                m_ConvertSheet(converter, converter.m_xmlSheetFilePaths[i]);
                string csvPathCreated = m_WriteToCsv(converter, converter.m_sheetNames[i]);
                Console.WriteLine("[xlsx2csv] File Created. {0}", csvPathCreated);
            }

            Directory.Delete(extractDirectory, true);
            return converter;
        }

        private static void m_ParseSharedString(XlsxToCsvConverter _converter, string _extractDirectory)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = false;

            FileStream fs = new FileStream(_extractDirectory + @"\xl\sharedStrings.xml", FileMode.Open, FileAccess.Read);
            XmlReader rd = XmlReader.Create(fs);

            int count = 0;
            int iterator = -1;

            while (rd.Read())
            {
                if (rd.NodeType != XmlNodeType.Element)
                    continue;

                switch (rd.Name)
                {
                    case "sst":
                        if (int.TryParse(rd.GetAttribute("count"), out count))
                            _converter.m_sharedStrings = new string[count];
                        else
                            _converter.m_sharedStrings = new string[0];
                        break;
                    case "t":
                        rd.Read();
                        _converter.m_sharedStrings[++iterator] = rd.Value;
                        break;
                    default:
                        break;
                }
            }

            rd.Close();
            fs.Close();
        }

        private static void m_ParseWorkbook(XlsxToCsvConverter _converter, string _extractDirectory, string _csvAliasOrNull)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = false;

            FileStream fs = new FileStream(_extractDirectory + @"\xl\workbook.xml", FileMode.Open, FileAccess.Read);
            XmlReader rd = XmlReader.Create(fs);

            _converter.m_xmlSheetFilePaths = Directory.GetFiles(_extractDirectory + @"\xl\worksheets");
            int sheetCount = _converter.m_xmlSheetFilePaths.Length;
            _converter.m_sheetCount = sheetCount;
            _converter.m_sheetNames = new List<string>(sheetCount);

            string csvAlias = _csvAliasOrNull == null ? string.Empty : _csvAliasOrNull + '+';

            while (rd.Read())
            {
                if (rd.NodeType == XmlNodeType.Element && rd.Name.Equals("sheet"))
                {
                    string sheetName = rd.GetAttribute("name");
                    string sheetId = rd.GetAttribute("sheetId");

                    _converter.m_sheetNames.Add(string.Format("{0}{1}", csvAlias, sheetName));
                }
            }

            rd.Close();
            fs.Close();
        }

        private static void m_ConvertSheet(XlsxToCsvConverter _converter, string _srcXmlSheetFilePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = false;

            FileStream xmlFile = new FileStream(_srcXmlSheetFilePath, FileMode.Open, FileAccess.Read);
            XmlReader xmlFileReader = XmlReader.Create(xmlFile);

            while (xmlFileReader.Read())
            {
                if (xmlFileReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlFileReader.Name)
                {
                    case "dimension":
                        string[] coordinates = xmlFileReader.GetAttribute("ref").Split(':');

                        m_ParseCoordinate(out _converter.m_offsetRow, out _converter.m_offsetColumn, coordinates[0]);

                        int lastRow = _converter.m_offsetRow;
                        int lastColumn = _converter.m_offsetColumn;

                        if (coordinates.Length == 2)
                            m_ParseCoordinate(out lastRow, out lastColumn, coordinates[1]);

                        _converter.m_spanRow = lastRow - _converter.m_offsetRow + 1;
                        _converter.m_spanColumn = lastColumn - _converter.m_offsetColumn + 1;
                        _converter.m_csvTable = new string[_converter.m_spanRow, _converter.m_spanColumn];
                        break;
                    case "c":
                        m_ParseCoordinate(out _converter.m_itRow, out _converter.m_itColumn, xmlFileReader.GetAttribute("r"));
                        _converter.m_itType = xmlFileReader.GetAttribute("t");
                        break;
                    case "v":
                        int rIndex = _converter.m_itRow - _converter.m_offsetRow;
                        int cIndex = _converter.m_itColumn - _converter.m_offsetColumn;

                        xmlFileReader.Read();

                        if (_converter.m_itType == null)
                            _converter.m_csvTable[rIndex, cIndex] = xmlFileReader.Value;
                        else if (_converter.m_itType.Equals("s"))
                            _converter.m_csvTable[rIndex, cIndex] = _converter.m_sharedStrings[int.Parse(xmlFileReader.Value)];
                        break;
                    default:
                        break;
                }
            }

            xmlFileReader.Close();
            xmlFile.Close();
        }

        private static string m_WriteToCsv(XlsxToCsvConverter _converter, string _sheetName)
        {
            string csvFilePath = _converter.m_destDirectory + @"\" + _sheetName + ".csv";
            FileStream csvFile = new FileStream(csvFilePath, FileMode.Create, FileAccess.Write);
            StreamWriter csvFileWriter = new StreamWriter(csvFile, Encoding.UTF8);

            for (int r = 0; r < _converter.m_spanRow; ++r)
            {
                for (int c = 0; c < _converter.m_spanColumn; ++c)
                {
                    if (_converter.m_csvTable[r, c] != null)
                        csvFileWriter.Write(_converter.m_csvTable[r, c]);

                    if (c < _converter.m_spanColumn - 1)
                        csvFileWriter.Write(',');
                    else if (r < _converter.m_spanRow - 1)
                        csvFileWriter.Write('\n');
                }
            }

            csvFileWriter.Close();
            csvFile.Close();

            return csvFilePath;
        }

        private static void m_ParseCoordinate(out int _row, out int _column, string _coordinate)
        {
            int column = 0;
            int row = 0;

            for (int i = 0; i < _coordinate.Length; ++i)
            {
                if (_coordinate[i] >= '0' && _coordinate[i] <= '9')
                {
                    row *= 10;
                    row += (int)(_coordinate[i] - '0');
                }
                else
                {
                    column *= 26;
                    column += (int)(_coordinate[i] - 'A');
                }
            }

            _row = row;
            _column = column;
        }
    }
}
