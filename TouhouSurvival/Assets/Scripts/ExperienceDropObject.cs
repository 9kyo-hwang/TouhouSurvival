using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ExperienceDropObject : MonoBehaviour
{
    [Header("경험치 드랍 오브젝트 정보")] public Sprite sprite;
    public float amount;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.sprite = sprite;
    }
}
