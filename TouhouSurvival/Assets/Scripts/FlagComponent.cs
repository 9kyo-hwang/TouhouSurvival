using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    [DisallowMultipleComponent]
    public class FlagComponent : MonoBehaviour
    {
        public bool this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        private Dictionary<string, bool> _flagTable;

        public bool Get(string key)
        {
            CreateTable();
            AppendKey(key, false);
            return _flagTable[key];
        }

        public void Set(string key, bool value)
        {
            CreateTable();
            AppendKey(key, value);
            _flagTable[key] = value;
        }

        public void SetFlagTrue(string key)
        {
            CreateTable();
            AppendKey(key, true);
            _flagTable[key] = true;
        }

        public void SetFlagFalse(string key)
        {
            CreateTable();
            AppendKey(key, false);
            _flagTable[key] = false;
        }

        private void CreateTable()
        {
            if (_flagTable == null)
                _flagTable = new Dictionary<string, bool>(1);
        }

        private void AppendKey(string key, bool value)
        {
            if (!_flagTable.ContainsKey(key))
                _flagTable.Add(key, value);
        }
    }
}