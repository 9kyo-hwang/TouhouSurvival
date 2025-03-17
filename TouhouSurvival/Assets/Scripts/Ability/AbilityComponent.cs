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

        protected Player _player { get; private set; }

        protected virtual void Awake()
        {
            Attributes = GetComponent<AttributeSet>();
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

        // TODO: 매 자식 컴포넌트에서 이를 재정의해야 하는데, 코드 중복을 제거할 수 있는 방안을 찾아야 합니다.
        protected virtual void OnChangeAbilityLevel(int prevLevel, int nextLevel)
        {

        }
    }
}