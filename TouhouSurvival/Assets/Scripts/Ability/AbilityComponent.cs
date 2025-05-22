using System;
using UnityEngine;

namespace Unchord
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        public const string FLAG_SHOULD_DESTROY = "ShouldDestroy";

        public Sprite DisplayIcon => displayIcon;
        public string DisplayName => displayName;
        public string DisplayDescription => displayDescription;

        public AttributeSet Attributes { get; private set; }

        [Header("Displays on GUI")]
        [SerializeField]
        private Sprite displayIcon;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private string displayDescription;

        protected Player Player { get; private set; }
        private int _level;
        public int CurrentLevel
        {
            get => _level;
            private set
            {
                //value = Mathf.Clamp(value, 0, MaxLevel);

                if (value != _level)
                {
                    int prevLevel = _level;
                    _level = value;
                    //OnLevelUp?.Invoke(this, new LevelUpEventArgs(prevLevel, _level));
                }
            }
        }
        public int MaxLevel => Attributes.MaxLevel;
        //public event EventHandler<LevelUpEventArgs> OnLevelUp;

        protected virtual void Awake()
        {
            Attributes = GetComponent<AttributeSet>();
            //Attributes.Initialize(OnLevelUp);
        }

        protected virtual void Start()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void Update()
        {

        }

        public void Subscribe(Player player)
        {
            Player = player;
        }

        public void Enable()
        {
            _level = 1; // OnLevelUp이 발동되지 않도록. 필요 시 CurrentLevel로 변경
            this.gameObject.SetActive(true);
        }

        public void LevelUp()
        {
            CurrentLevel++;
            Attributes.ApplyModifiersSelf(CurrentLevel);
            this.gameObject.SetActive(true);
        }
    }
}