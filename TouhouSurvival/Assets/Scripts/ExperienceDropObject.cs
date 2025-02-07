using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ExperienceDropObject : MonoBehaviour
{
    [Header("경험치 드랍 오브젝트 정보")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private float amount;
    [SerializeField] private float radius;
    
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = sprite;
    }

    private void FixedUpdate()
    {
        if (FindPlayerWithinRange(out GameObject player))
        {
            player.GetComponent<NewPlayer>().AttributeSet.AddExperience(amount);
            Destroy(gameObject);
        }
    }

    private bool FindPlayerWithinRange(out GameObject player)
    {
        player = null;
        Collider2D result = Physics2D.OverlapCircle(transform.position, radius);
        if (result.CompareTag("Player"))
        {
            player = result.gameObject;
            return true;
        }

        return false;
    }
}
