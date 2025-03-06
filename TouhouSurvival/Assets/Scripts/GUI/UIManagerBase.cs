using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public abstract class UIManagerBase<T_UIManager> : Singleton<T_UIManager>
    where T_UIManager : Component
    {
        private Dictionary<string, Component> _componentTable;

        protected virtual void Awake()
        {
            _componentTable = new Dictionary<string, Component>(8);
        }

        protected virtual void Start()
        {

        }

        protected Component GetComponentFromTable(string resourcePath, Transform parentOrNull, bool showOnInitialLoad)
        {
            return GetComponentFromTable<Component>(resourcePath, parentOrNull, showOnInitialLoad);
        }

        protected T_UnityComponent GetComponentFromTable<T_UnityComponent>(string resourcePath, Transform parentOrNull, bool showOnInitialLoad)
        where T_UnityComponent : Component
        {
            if (_componentTable.ContainsKey(resourcePath))
                return _componentTable[resourcePath] as T_UnityComponent;

            T_UnityComponent resource = Resources.Load<T_UnityComponent>(resourcePath);
            T_UnityComponent instance = GameObject.Instantiate(resource);

            if (parentOrNull == null)
                parentOrNull = this.transform;

            UnityEngine.Debug.Assert(parentOrNull != null);

            instance.transform.SetParent(parentOrNull, false);
            instance.gameObject.SetActive(showOnInitialLoad);
            _componentTable.Add(resourcePath, instance);

            return instance;
        }
    }
}