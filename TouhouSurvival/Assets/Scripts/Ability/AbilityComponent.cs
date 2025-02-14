using UnityEngine;

namespace Unchord
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        public const string FLAG_SHOULD_DESTROY = "ShouldDestroy";



        }

        {

        }

        {
        }

        {


        }

        {

        }

        public int Level
        {
            get => _level;
            set => SetLevel(value);
        }

        public float NormalizedLevel => (float)_level / maxLevel;

        public int maxLevel = 1;

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

        private void SetLevel(int level)
        {
            Debug.Assert(level >= 0);

            int prevLevel = _level;
            int nextLevel = level;

            for (int i = _level + 1; i <= level; ++i)
            {
                // TODO: Write code for updating player's stat. (level increasing)
            }

            for (int i = _level; i > level; --i)
            {
                // TODO: Write code for updating player's stat. (level decreasing)
            }

            _level = level;

            if (prevLevel != nextLevel)
            {
                // TODO: Enable this comment after integrating region Ability Pool Management from this class to Player class.
                // _player.OnChangeAbilityLevel(this, prevLevel, nextLevel);
            }
        }
    }
}