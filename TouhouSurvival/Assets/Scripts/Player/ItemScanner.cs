using System;
using UnityEngine;

public class ItemScanner : MonoBehaviour
{
    [SerializeField] private float range;
    private RaycastHit2D[] _hits;
    private NewPlayer _player;

    private void Awake()
    {
        _player = gameObject.GetComponent<NewPlayer>();
    }

    private void FixedUpdate()
    {
        float scanRangeMultiplier = _player.GetComponent<NewPlayerAttributeSet>().GetAttributeValue(PlayerAttributeNames.ScanRangeMultiplier);
        _hits = Physics2D.CircleCastAll(_player.transform.position, range * scanRangeMultiplier, Vector2.zero);
        AddExperiences();
    }

    private void AddExperiences()
    {
        foreach (RaycastHit2D hit in _hits)
        {
            ExperienceDropObject experience = hit.collider.GetComponent<ExperienceDropObject>();
            if (experience)
            {
                _player.GetComponent<NewPlayerAttributeSet>().AddExperience(experience.amount);
                Destroy(experience.gameObject);
            }
        }
    }
}
