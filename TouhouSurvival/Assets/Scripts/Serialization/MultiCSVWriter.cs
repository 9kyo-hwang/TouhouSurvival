using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Unchord
{
    public class MultiCSVWriter
    {
        private Stream _stream;
        private StringBuilder _builder;
        private Dictionary<string, List<string>> _writtenTypes;

        public MultiCSVWriter(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException();
            }

            _stream = stream;
            _builder = new StringBuilder(1024);
            _writtenTypes = new Dictionary<string, List<string>>(16);
        }

        public void Close()
        {
            _stream.Close();
        }

        public bool TryWriteTable<T>(List<T> records, params string[] aliasesOrNull)
        {
            Type type = typeof(T);
            string[] headers;

            if (ContainsAnyAlias<T>(aliasesOrNull))
            {
                return false;
            }

            if (!TryGetHeaders<T>(out headers))
            {
                return false;
            }

            WriteAliasRecord<T>(aliasesOrNull);
            WriteHeaderRecord<T>(headers);
            WriteContentsRecord<T>(records, headers);
            _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));

            AddAllAlias<T>(aliasesOrNull);

            return true;
        }

        private bool ContainsAnyAlias<T>(params string[] aliasesOrNull)
        {
            Type type = typeof(T);

            if (!_writtenTypes.ContainsKey(type.Name))
            {
                return false;
            }

            if (aliasesOrNull == null || aliasesOrNull.Length == 0)
            {
                return _writtenTypes[type.Name].Contains(string.Empty);
            }

            for (int i = 0; i < aliasesOrNull.Length; ++i)
            {
                if (_writtenTypes[type.Name].Contains(aliasesOrNull[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddAllAlias<T>(params string[] aliasesOrNull)
        {
            Type type = typeof(T);

            if (!_writtenTypes.ContainsKey(type.Name))
            {
                _writtenTypes.Add(type.Name, new List<string>(16));
            }

            if (aliasesOrNull == null || aliasesOrNull.Length == 0)
            {
                _writtenTypes[type.Name].Add(string.Empty);
                return;
            }

            for (int i = 0; i < aliasesOrNull.Length; ++i)
            {
                _writtenTypes[type.Name].Add(aliasesOrNull[i]);
            }
        }

        private void WriteAliasRecord<T>(params string[] aliasesOrNull)
        {
            _stream.Write(Encoding.UTF8.GetBytes($"#{typeof(T).Name}"));

            int length = aliasesOrNull == null ? 0 : aliasesOrNull.Length;

            for (int i = 0; i < length; ++i)
            {
                _stream.Write(Encoding.UTF8.GetBytes($",{aliasesOrNull[i]}"));
            }

            _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));
        }

        private void WriteHeaderRecord<T>(string[] headers)
        {
            for (int i = 0; i < headers.Length; ++i)
            {
                if (i > 0)
                {
                    _stream.Write(Encoding.UTF8.GetBytes(","));
                }

                _stream.Write(Encoding.UTF8.GetBytes($"{headers[i]}"));
            }

            _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));
        }

        private void WriteContentsRecord<T>(List<T> records, string[] headers)
        {
            if (records.Count == 0)
            {
                _stream.Write(Encoding.UTF8.GetBytes("\u000D\u000A"));
                return;
            }

            Type type = typeof(T);

            for (int i = 0; i < records.Count; ++i)
            {
                _builder.Clear();

                for (int j = 0; j < headers.Length; ++j)
                {
                    _builder.Append($"{type.GetField(headers[j]).GetValue(records[i]).ToString()},");
                }

                _builder.Remove(_builder.Length - 1, 1);
                _builder.Append("\u000D\u000A");
                _stream.Write(Encoding.UTF8.GetBytes(_builder.ToString()));
            }
        }

        private bool TryGetHeaders<T>(out string[] headers)
        {
            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<string> fields = new List<string>(fieldInfos.Length);

            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                TypeCode tCode = Type.GetTypeCode(fieldInfos[i].FieldType);

                if (IsValidFieldType(tCode))
                {
                    fields.Add(fieldInfos[i].Name);
                }
            }

            if (fields.Count == 0)
            {
                headers = null;
                return false;
            }

            headers = fields.ToArray();
            return true;
        }

        private bool IsValidFieldType(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }
    }
}