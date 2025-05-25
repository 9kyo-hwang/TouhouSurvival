using System;
using UnityEngine;

namespace Unchord
{
    public class ItemScanner : MonoBehaviour
    {
        [SerializeField] private float range;
        private RaycastHit2D[] _hits;
        private Player _player;

        private void Awake()
        {
            _player = gameObject.GetComponent<Player>();
        }

        private void FixedUpdate()
        {
            //float scanRangeMultiplier = _player.GetComponent<PlayerAttributeSet>().GetCurrentValue(PlayerAttributeType.ScanRangeMultiplier);
            _hits = Physics2D.CircleCastAll(_player.transform.position, range, Vector2.zero);
            AddExperiences();
        }

        private void AddExperiences()
        {
            foreach (RaycastHit2D hit in _hits)
            {
                ExperienceDropObject experience = hit.collider.GetComponent<ExperienceDropObject>();
                if (experience)
                {
                    // _player.GetComponent<PlayerAttributeSet>().AddExperience(experience.amount);
                    _player.LevelSystem.Experience += experience.amount;
                    Destroy(experience.gameObject);
                }
            }
        }
    }
}
