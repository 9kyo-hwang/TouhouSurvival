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

        [Header("Displays on GUI")]
        [SerializeField]
        private Sprite displayIcon;

        [SerializeField]
        private string displayName;

        [SerializeField]
        private string displayDescription;

        protected Player Player { get; private set; }
        public int CurrentLevel { get; private set; } = 0;

        public virtual int MaxLevel { get; } = 1;
        
        protected virtual void Awake()
        {
            Player = GetComponentInParent<Player>();
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

        public virtual void LevelUp()
        {
            CurrentLevel++;
            this.gameObject.SetActive(true);
        }

        public virtual string GetModifierDescription(int level)
        {
            return string.Empty;
        }
    }
}