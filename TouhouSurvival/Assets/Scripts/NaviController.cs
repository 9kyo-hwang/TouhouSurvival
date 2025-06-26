using UnityEngine;

namespace Unchord
{
    public class NaviController : MonoBehaviour
    {
        [Header("기본 움직임 설정")]
        public float horizontalAmplitude = 2f;   // 좌우 진동 범위
        public float horizontalSpeed   = 1f;    // 좌우 진동 속도 (rad/sec)
        public float verticalAmplitude = 0.5f;  // 상하 sine 진동 범위
        public float verticalSpeed     = 2f;    // 상하 sine 진동 속도 (rad/sec)

        [Header("노이즈(랜덤) 설정")]
        public float noiseAmplitude   = 0.2f;   // 노이즈 진동 범위
        public float noiseFrequency   = 0.5f;   // 노이즈 변화 속도

        [Header("팔로우 스무딩")]
        public float followSmoothTime = 0.3f;  // 작을수록 빠르게, 클수록 더 크게 지연
        private Vector3 centerPosition;        // 실제 요정이 돌며 참조할 중심점
        private Vector3 followVelocity;        // SmoothDamp 용 내부 상태

        private float xPhase, yPhase, noiseSeed;

        private void Start()
        {
            // 시작 시 원위치 저장
            centerPosition = transform.parent.position;

            // sine 위상과 노이즈 시드 랜덤화
            xPhase    = Random.Range(0f, Mathf.PI * 2f);
            yPhase    = Random.Range(0f, Mathf.PI * 2f);
            noiseSeed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            // 1) 플레이어 위치로 부드럽게 이동 (지연 효과)
            centerPosition = Vector3.SmoothDamp(
                centerPosition,
                transform.parent.position,
                ref followVelocity,
                followSmoothTime
            );

            // 2) 사인 진동 계산
            float t = Time.time;
            float x = Mathf.Sin(t * horizontalSpeed + xPhase) * horizontalAmplitude;
            float y = Mathf.Sin(t * verticalSpeed   + yPhase) * verticalAmplitude;

            // 3) Perlin Noise 로 부드러운 랜덤 오프셋
            float nx = (Mathf.PerlinNoise(t * noiseFrequency, noiseSeed) - 0.5f) * 2f * noiseAmplitude;
            float ny = (Mathf.PerlinNoise(noiseSeed, t * noiseFrequency) - 0.5f) * 2f * noiseAmplitude;

            // 4) 최종 위치 적용
            transform.position = centerPosition + new Vector3(x + nx, y + ny, 0f);
        }
    }
}
