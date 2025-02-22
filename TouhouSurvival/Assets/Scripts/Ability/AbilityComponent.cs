using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        public const string FLAG_SHOULD_DESTROY = "ShouldDestroy";

        public int Level
        {
            get => _level;
            set => SetLevel(value);
        }

        public float NormalizedLevel => (float)_level / maxLevel;

        public int maxLevel = 1;
        public Sprite icon;

        protected Player _player { get; private set; }
        private int _level;
        
        protected virtual void Awake()
        {

        }

        protected virtual void Update()
        {

        }

        public void Subscribe(Player player)
        {
            _player = player;
        }

        public int SortSiblingIndex()
        {
            Transform parent = transform.parent;
            int i = transform.GetSiblingIndex();

            while (i > 0)
            {
                --i;
                AbilityComponent temp = parent.GetChild(i).GetComponent<AbilityComponent>();

                if (temp.gameObject.activeSelf == true)
                {
                    i++;
                    break;
                }
            }

            transform.SetSiblingIndex(i);
            return i;
        }

        private void SetLevel(int level)
        {
            Debug.Assert(level >= 0);

            for (int i = _level + 1; i <= level; ++i)
            {
                _player.OnChangeAbilityLevel(this, i - 1, i);
            }

            for (int i = _level; i > level; --i)
            {
                _player.OnChangeAbilityLevel(this, i, i - 1);
            }

            _level = level;
        }
    }
}