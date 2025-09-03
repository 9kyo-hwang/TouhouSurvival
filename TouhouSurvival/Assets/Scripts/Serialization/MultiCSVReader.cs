using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Unchord
{
    // TODO: 코드 기본 로직 완성 후 예외 처리 문제를 고민할 것.
    public class MultiCSVReader : IDisposable
    {
        private const int DEFAULT_BYTE_BUFFER_SIZE = 1024;

        private Stream _stream;
        private StringBuilder _builder;
        private Dictionary<string, Dictionary<string, long>> _aliasMap;

        private byte[] _byteBuffer;
        private int _byteIdx;
        private int _byteLen;

        public MultiCSVReader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException();
            }

            _stream = stream;

            _byteBuffer = new byte[DEFAULT_BYTE_BUFFER_SIZE];
            _builder = new StringBuilder(DEFAULT_BYTE_BUFFER_SIZE);

            _byteIdx = -1;
            _byteLen = 0;

            TryCreateAliasMap(out _aliasMap);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                    _stream = null;
                }
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        private bool TryCreateAliasMap(out Dictionary<string, Dictionary<string, long>> map)
        {
            string[] aliases;
            Dictionary<string, Dictionary<string, long>> aliasMap = new Dictionary<string, Dictionary<string, long>>(16);
            map = aliasMap;

            _stream.Position = 0;

            while (ReadBuffer() > 0)
            {
                if (!EqualsNextChar('#', false))
                {
                    ++_byteIdx;
                    continue;
                }

                long position = _stream.Position - _byteLen + _byteIdx + 1;

                if (!TryParseAliasRecord(out aliases))
                {
                    _stream.Position = 0;
                    _byteIdx = -1;
                    _byteLen = 0;
                    return false;
                }

                if (!aliasMap.ContainsKey(aliases[0]))
                {
                    aliasMap.Add(aliases[0], new Dictionary<string, long>(16));
                    aliasMap[aliases[0]].Add(string.Empty, -position - 1);
                }

                if (aliases.Length == 1)
                {
                    if (aliasMap[aliases[0]][string.Empty] >= 0)
                    {
                        _stream.Position = 0;
                        _byteIdx = -1;
                        _byteLen = 0;
                        return false;
                    }

                    aliasMap[aliases[0]][string.Empty] = position;
                }

                for (int i = 1; i < aliases.Length; ++i)
                {
                    aliasMap[aliases[0]].Add(aliases[i], position);
                }
            }

            _stream.Position = 0;
            _byteIdx = -1;
            _byteLen = 0;

            return true;
        }

        private bool ContainsAlias<T>(string[] aliases, string aliasOrNull)
        {
            Type type = typeof(T);

            if (aliasOrNull == null || aliasOrNull.Equals(string.Empty))
            {
                return true;
            }

            if (!_aliasMap.ContainsKey(type.Name))
            {
                return false;
            }

            for (int i = 0; i < aliases.Length; ++i)
            {
                if (aliases[i].Equals(aliasOrNull))
                {
                    return true;
                }
            }

            return false;
        }

        private long GetTableIndex<T>(string aliasOrNull)
        {
            Type type = typeof(T);

            if (aliasOrNull == null)
            {
                aliasOrNull = string.Empty;
            }

            if (!_aliasMap.ContainsKey(type.Name))
            {
                return -1;
            }

            if (!_aliasMap[type.Name].ContainsKey(aliasOrNull))
            {
                return -1;
            }

            long position = _aliasMap[type.Name][aliasOrNull];

            if (position < 0)
            {
                return -(position + 1);
            }
            else
            {
                return position;
            }
        }

        public bool TryParseTable<T>(out List<T> records, string aliasOrNull = null)
        {
            long tableIndex = GetTableIndex<T>(aliasOrNull);

            if (tableIndex < 0)
            {
                records = null;
                return false;
            }

            _stream.Position = tableIndex;
            _byteIdx = -1;
            _byteLen = 0;

            string[] aliases;
            List<T> list = new List<T>(0);

            if (!TryParseAliasRecord(out aliases))
            {
                records = null;
                return false;
            }
            if (!ContainsAlias<T>(aliases, aliasOrNull))
            {
                records = null;
                return false;
            }
            if (!TryParseContents<T>(out list))
            {
                records = null;
                return false;
            }
            if (!TryParseEndOfLine())
            {
                records = null;
                return false;
            }

            records = list;
            return true;
        }

        private bool TryParseAliasRecord(out string[] aliases)
        {
            string[] headers;

            if (!EqualsNextChar('#', true))
            {
                aliases = null;
                return false;
            }
            else if (!TryParseHeaderRecord(out headers))
            {
                aliases = null;
                return false;
            }
            else if (!TryParseEndOfLine())
            {
                aliases = null;
                return false;
            }

            aliases = headers;
            return true;
        }

        private bool TryParseContents<T>(out List<T> contents)
        {
            List<T> list = new List<T>(0);

            string[] headers;
            string[] data;

            if (!TryParseHeaderRecord(out headers))
            {
                contents = null;
                return false;
            }
            if (!TryParseEndOfLine())
            {
                contents = null;
                return false;
            }

            while (!EqualsNextChar('\r') && !EqualsNextChar('\n') && ReadBuffer() > 0)
            {
                if (!TryParseDataRecord(out data))
                {
                    contents = null;
                    return false;
                }

                T instance;

                if (!TryCreateInstance<T>(out instance, headers, data))
                {
                    contents = null;
                    return false;
                }

                list.Add(instance);

                if (!TryParseEndOfLine())
                {
                    contents = null;
                    return false;
                }
            }

            contents = list;
            return true;
        }

        private bool TryParseHeaderRecord(out string[] headers)
        {
            _builder.Clear();

            if (ReadBuffer() == 0)
            {
                headers = null;
                return false;
            }

            // TODO: 이 곳을 단순 한 줄 읽기가 아닌 string과 comma를 파싱하는 로직으로 변경해야 합니다.
            while (ReadBuffer() > 0)
            {
                if (_byteBuffer[_byteIdx + 1] == '\r' || _byteBuffer[_byteIdx + 1] == '\n')
                {
                    break;
                }

                ++_byteIdx;
            }

            PushToBuilder();

            headers = _builder.ToString().Split(',');
            return true;
        }

        private bool TryParseDataRecord(out string[] data)
        {
            _builder.Clear();

            if (ReadBuffer() == 0)
            {
                data = null;
                return false;
            }

            // TODO: 이 곳을 단순 한 줄 읽기가 아닌 string과 comma를 파싱하는 로직으로 변경해야 합니다.
            while (ReadBuffer() > 0)
            {
                if (_byteBuffer[_byteIdx + 1] == '\r' || _byteBuffer[_byteIdx + 1] == '\n')
                {
                    break;
                }

                ++_byteIdx;
            }

            PushToBuilder();

            data = _builder.ToString().Split(',');
            return true;
        }

        private bool EqualsNextChar(char c, bool flushWhenEquals = false)
        {
            if (ReadBuffer() == 0)
            {
                // end of file reached.
                return false;
            }

            if (_byteBuffer[_byteIdx + 1] != c)
            {
                return false;
            }

            if (flushWhenEquals)
            {
                ++_byteIdx;
                FlushBuffer();
            }

            return true;
        }

        private bool TryParseEndOfLine()
        {
            if (ReadBuffer() == 0)
            {
                // end of file
                return true;
            }
            else if (EqualsNextChar('\n', true))
            {
                return true;
            }
            else if (EqualsNextChar('\r', true))
            {
                _ = EqualsNextChar('\n', true);
                return true;
            }
            else
            {
                return false;
            }
        }

        private int ReadBuffer()
        {
            if (_byteIdx + 1 < _byteLen)
            {
                return _byteLen - _byteIdx - 1;
            }

            if (_stream.Position == _stream.Length)
            {
                return 0;
            }

            PushToBuilder();

            int readLen = _stream.Read(_byteBuffer, _byteLen, _byteBuffer.Length - _byteLen);

            if (readLen == 0)
            {
                // end of file reached.
                _byteIdx = -1;
                return 0;
            }

            _byteLen += readLen;

            return _byteLen;
        }

        private void FlushBuffer()
        {
            int distance = _byteIdx + 1;

            if (distance <= 0)
                return;

            for (int i = distance; i < _byteLen; ++i)
            {
                _byteBuffer[i - distance] = _byteBuffer[i];
            }

            _byteIdx = -1;
            _byteLen -= distance;
        }

        private void PushToBuilder()
        {
            int i = 0;

            // utf-8 decoding algorithm.
            while (i <= _byteIdx)
            {
                int length = 0;
                uint concatBytes = 0;

                if (_byteBuffer[i] >= 0xF8)
                {
                    // decoding error.
                    break;
                }
                else if (_byteBuffer[i] >= 0xF0)
                {
                    concatBytes += (uint)(_byteBuffer[i] & 0x07);
                    length = 4;
                }
                else if (_byteBuffer[i] >= 0xE0)
                {
                    concatBytes += (uint)(_byteBuffer[i] & 0x0F);
                    length = 3;
                }
                else if (_byteBuffer[i] >= 0xC0)
                {
                    concatBytes += (uint)(_byteBuffer[i] & 0x1F);
                    length = 2;
                }
                else
                {
                    concatBytes += (uint)(_byteBuffer[i] & 0x7F);
                    length = 1;
                }

                if (i + length - 1 > _byteIdx)
                {
                    // decoding successfully finished.
                    break;
                }

                for (int j = i + 1; j < i + length; ++j)
                {
                    if ((_byteBuffer[j] & 0xC0) != 0x80)
                    {
                        // decoding error.
                        break;
                    }

                    concatBytes <<= 6;
                    concatBytes += (uint)(_byteBuffer[j] & 0x3F);
                }

                _builder.Append((char)(concatBytes & 0xFFFF));

                if (concatBytes > 0xFFFF)
                {
                    _builder.Append((char)(concatBytes >> 16));
                }

                i += length;
            }

            int distance = i;

            for (int j = i; j < _byteLen; ++j)
            {
                _byteBuffer[j - distance] = _byteBuffer[j];
            }

            _byteLen -= distance;
            _byteIdx = -1;
        }

        private bool TryCreateInstance<T>(out T instance, string[] headers, string[] data)
        {
            T inst = Activator.CreateInstance<T>();

            for (int i = 0; i < Math.Min(headers.Length, data.Length); ++i)
            {
                string field = headers[i];
                string value = data[i];

                if (!TrySetField<T>(ref inst, field, value))
                {
                    instance = default;
                    return false;
                }
            }

            instance = inst;
            return true;
        }

        private bool TrySetField<T>(ref T instance, string field, string value)
        {
            Type type = typeof(T);
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;

            FieldInfo fieldInfo = type.GetField(field, flag);

            if (fieldInfo == null)
            {
                return true;
            }

            TypeCode tCode = Type.GetTypeCode(fieldInfo.FieldType);
            object parsedValue;

            switch (tCode)
            {
                case TypeCode.String:
                    parsedValue = value;
                    break;
                case TypeCode.Boolean:
                    parsedValue = bool.TryParse(value, out var b) ? b : default;
                    break;
                case TypeCode.Char:
                    parsedValue = char.TryParse(value, out var c) ? c : default;
                    break;
                case TypeCode.SByte:
                    parsedValue = sbyte.TryParse(value, out var sby) ? sby : default;
                    break;
                case TypeCode.Byte:
                    parsedValue = byte.TryParse(value, out var by) ? by : default;
                    break;
                case TypeCode.Int16:
                    parsedValue = short.TryParse(value, out var sht) ? sht : default;
                    break;
                case TypeCode.UInt16:
                    parsedValue = ushort.TryParse(value, out var usht) ? usht : default;
                    break;
                case TypeCode.Int32:
                    parsedValue = int.TryParse(value, out var it) ? it : default;
                    break;
                case TypeCode.UInt32:
                    parsedValue = uint.TryParse(value, out var uit) ? uit : default;
                    break;
                case TypeCode.Int64:
                    parsedValue = long.TryParse(value, out var lg) ? lg : default;
                    break;
                case TypeCode.UInt64:
                    parsedValue = ulong.TryParse(value, out var ulg) ? ulg : default;
                    break;
                case TypeCode.Single:
                    parsedValue = float.TryParse(value, out var flt) ? flt : default;
                    break;
                case TypeCode.Double:
                    parsedValue = double.TryParse(value, out var db) ? db : default;
                    break;
                default:
                    parsedValue = default;
                    break;
            }

            bool result = parsedValue != null;
            object boxedInstance = instance;
            fieldInfo.SetValue(boxedInstance, result ? parsedValue : default);
            instance = (T)boxedInstance;
            return result;
        }
    }
}