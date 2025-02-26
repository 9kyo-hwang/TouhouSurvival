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

        public int MaxLevel => maxLevel;
        public float NormalizedLevel => (float)_level / maxLevel;

        public Sprite DisplayIcon => displayIcon;
        public string DisplayName => displayName;
        public string DisplayDescription => displayDescription;

        [SerializeField]
        private int maxLevel = 1;
        
        [Header("Displays on GUI")]
        [SerializeField]
        private Sprite displayIcon;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private string displayDescription;

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