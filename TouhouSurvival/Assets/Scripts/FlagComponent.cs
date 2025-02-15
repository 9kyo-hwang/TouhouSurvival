using System;
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
            set => Set(key, value, true);
        }

        private Dictionary<string, bool> _flagTable;
        private Dictionary<string, Action<FlagComponent>> _tEventHandlerTable;
        private Dictionary<string, Action<FlagComponent>> _fEventHandlerTable;

        public bool Get(string key)
        {
            CreateFlagTable();

            // NOTE: flag의 초기 상태는 항상 false로 정의함.
            AppendKey(key, false);

            return _flagTable[key];
        }

        public void Set(string key, bool value)
        {
            Set(key, value, true);
        }

        public void SetWithoutEvent(string key, bool value)
        {
            Set(key, value, false);
        }

        public void SetFlagTrue(string key)
        {
            SetFlagTrue(key, true);
        }

        public void SetFlagTrueWithoutEvent(string key)
        {
            SetFlagTrue(key, false);
        }

        public void SetFlagFalse(string key)
        {
            SetFlagFalse(key, true);
        }

        public void SetFlagFalseWithoutEvent(string key)
        {
            SetFlagFalse(key, false);
        }

        public void AddEventTrue(string key, Action<FlagComponent> eventHandler)
        {
            CreateEventHandlerTableT();
            AppendKeyT(key);
            _tEventHandlerTable[key] += eventHandler;
        }

        public void RemoveEventTrue(string key, Action<FlagComponent> eventHandler)
        {
            CreateEventHandlerTableT();
            AppendKeyT(key);
            _tEventHandlerTable[key] -= eventHandler;
        }

        public void AddEventFalse(string key, Action<FlagComponent> eventHandler)
        {
            CreateEventHandlerTableF();
            AppendKeyF(key);
            _fEventHandlerTable[key] += eventHandler;
        }

        public void RemoveEventFalse(string key, Action<FlagComponent> eventHandler)
        {
            CreateEventHandlerTableF();
            AppendKeyF(key);
            _fEventHandlerTable[key] -= eventHandler;
        }

        private void Set(string key, bool value, bool useEvent)
        {
            CreateFlagTable();

            if (AppendKey(key, value) || _flagTable[key] == value)
                return;

            _flagTable[key] = value;

            if (!useEvent)
                return;
            else if (value)
                PublishEventT(key);
            else
                PublishEventF(key);
        }

        private void SetFlagTrue(string key, bool useEvent)
        {
            CreateFlagTable();

            if (!AppendKey(key, true) && _flagTable[key])
                return;

            _flagTable[key] = true;

            if (useEvent)
                PublishEventT(key);
        }

        private void SetFlagFalse(string key, bool useEvent)
        {
            CreateFlagTable();

            if (AppendKey(key, false) || !_flagTable[key])
                return;

            _flagTable[key] = false;

            if (useEvent)
                PublishEventF(key);
        }

        private void PublishEventT(string key)
        {
            CreateEventHandlerTableT();
            AppendKeyT(key);
            _tEventHandlerTable[key]?.Invoke(this);
        }

        private void PublishEventF(string key)
        {
            CreateEventHandlerTableF();
            AppendKeyF(key);
            _fEventHandlerTable[key]?.Invoke(this);
        }

        private bool CreateFlagTable()
        {
            if (_flagTable != null)
                return false;

            _flagTable = new Dictionary<string, bool>(1);
            return true;
        }

        private bool AppendKey(string key, bool value)
        {
            if (_flagTable.ContainsKey(key))
                return false;
            
            _flagTable.Add(key, value);
            return true;
        }

        private bool CreateEventHandlerTableF()
        {
            if (_fEventHandlerTable != null)
                return false;

            _fEventHandlerTable = new Dictionary<string, Action<FlagComponent>>(1);
            return true;
        }

        private bool AppendKeyF(string key)
        {
            if (_fEventHandlerTable.ContainsKey(key))
                return false;

            _fEventHandlerTable.Add(key, null);
            return true;
        }

        private bool CreateEventHandlerTableT()
        {
            if (_tEventHandlerTable != null)
                return false;

            _tEventHandlerTable = new Dictionary<string, Action<FlagComponent>>(1);
            return true;
        }

        private bool AppendKeyT(string key)
        {
            if (_tEventHandlerTable.ContainsKey(key))
                return false;

            _tEventHandlerTable.Add(key, null);
            return true;
        }
    }
}