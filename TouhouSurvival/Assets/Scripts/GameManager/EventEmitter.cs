using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unchord
{
    // TEST: Unchord.BlockingEventHandler 컴포넌트 테스트를 위한 코드입니다.
    // TODO: GameManager에서 Blocking Event를 처리하는 부분의 분리 여부를 결정 후 이 파일을 삭제합니다.
    public class EventEmitter : MonoBehaviour
    {
        /*
         * 테스트 수행 방법
         * 1. 테스트를 위한 새로운 Scene을 만듭니다.
         * 2. 새로운 GameObject를 생성하고 아래 컴포넌트를 부착합니다.
         *      - Unchord.BlockingEventHandler
         *      - Unchord.EventEmitter
         * 3. Play 모드에 진입합니다.
         * 4. 키보드의 F1 또는 F2 키를 누릅니다. 그러면 이벤트가 등록됩니다.
         *      - 이벤트가 등록되면 Occurred 메시지가 Console 창에 출력됩니다.
         * 5. 인스펙터 창에서 EventEmitter 컴포넌트의 b1 또는 b2 변수를 true로 설정합니다.
         *      - F1을 눌러 이벤트를 등록했다면 b1을 true로 설정하세요.
         *      - F2를 눌러 이벤트를 등록했다면 b2를 true로 설정하세요.
         * 6. 이벤트가 성공적으로 수행되었는지 확인합니다.
         *      - 올바르게 수행되었다면 OK 메시지가 Console 창에 출력됩니다.
         */

        private GameManager _gameManager;
        private BlockingEventHandler _blockingEventHandler;

        [Header("Event Control Flags")]
        public bool b1;
        public bool b2;

        [Header("Values for Debugging")]
        public float timer;

        private void Awake()
        {
            _gameManager = GameManager.Instance;
            _blockingEventHandler =  GetComponent<BlockingEventHandler>();

            _blockingEventHandler.onBlockingEventOccurred += OnBlockingEventOccurred;
            _blockingEventHandler.onBlockingEventHandled += OnBlockingEventHandled;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _blockingEventHandler.Publish(OnF1());
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _blockingEventHandler.Publish(OnF2());
            }

            timer += Time.deltaTime;
        }

        private void OnBlockingEventOccurred()
        {
            Time.timeScale = 0.0f;
        }

        private void OnBlockingEventHandled()
        {
            Time.timeScale = 1.0f;
        }

        private IEnumerator OnF1()
        {
            Debug.Log("F1 Occurred.");
            yield return new WaitUntil(() => b1 == true);
            b1 = false;
            Debug.Log("F1 OK.");
        }

        private IEnumerator OnF2()
        {
            Debug.Log("F2 Occurred.");
            yield return new WaitUntil(() => b2 == true);
            b2 = false;
            Debug.Log("F2 OK.");
        }
    }
}