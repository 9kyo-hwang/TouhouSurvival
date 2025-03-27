using System;
using UnityEngine;

namespace Unchord
{
    public class HarudesuyoExplosionEffect : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Play()
        {
            _animator.SetTrigger("Explode");
        }

        // 폭발 애니메이션이 끝나면 호출되어 풀에 반환
        public void OnExplosionComplete()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}