using UnityEngine;

namespace Unchord
{
    public class HarudesuyoAttributeType
    {
        public const string Cooldown = "Cooldown";              // 쿨타임
        public const string Radius = "Radius";                  // 적 타게팅 범위
        public const string TargetCount = "TargetCount";        // 생성할 폭탄 개수
        public const string BombSpawnDelay = "BombSpawnDelay";  // 폭탄 생성 간격
        public const string ExplosionDelay = "ExplodeDelay";    // 폭탄 생성 후 폭발까지 걸리는 시간
        public const string ExplosionRadius = "ExplodeRadius";  // 폭탄 폭발 범위
        public const string Damage = "Damage";                  // 폭탄 폭발 데미지
    }
}
