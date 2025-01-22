using FMOD;
using FMOD.Studio;

namespace Unchord
{
    public sealed class MultipleSoundChannel : SoundChannel
    {
        public int Count => m_count;

        private SoundNode m_root;
        private int m_count = 0;

        public MultipleSoundChannel()
        {
            m_root = new SoundNode();
            m_root.prev = m_root;
            m_root.next = m_root;
        }

        public EventInstance AddSoundEvent(string _eventPath)
        {
            // UnityEngine.Debug.Assert(!base.IsPaused, "Cannot add sound event when sound channel paused.");
            UnityEngine.Debug.Assert(_eventPath != null, "Sound Event cannot be null.");

            SoundNode node = new SoundNode();
            node.instance = FMODUnity.RuntimeManager.CreateInstance(_eventPath);

            // NOTE: 초기화 코드
            node.instance.setVolume(base.Volume);
            node.instance.start();

            node.prev = m_root.prev;
            node.next = m_root;
            m_root.prev.next = node;
            m_root.prev = node;
            ++m_count;

            return node.instance;
        }

        public void OnUpdate()
        {
            if (base.IsPaused)
                return;

            PLAYBACK_STATE playbackState;
            SoundNode ptr = m_root.next;

            while(ptr != m_root)
            {
                SoundNode next = ptr.next;

                if (ptr.instance.getPlaybackState(out playbackState) == RESULT.OK &&
                    playbackState == PLAYBACK_STATE.STOPPED
                    )
                {
                    ptr.prev.next = ptr.next;
                    ptr.next.prev = ptr.prev;
                    ptr.prev = null;
                    ptr.next = null;
                    --m_count;
                }

                ptr = next;
            }
        }

        protected override void SetVolume(float volume)
        {
            if (_isPaused || _isMuted)
            {
                _volumeBuffer = volume;
            }
            else
            {
                _volume = volume;
                SetVolumeUnsafe(volume);
            }
        }

        protected override void Pause()
        {
            System.Diagnostics.Debug.Assert(_isPaused == false);

            if (!_isMuted)
            {
                _volumeBuffer = _volume;
                _volume = 0.0f;
                SetPauseUnsafe(true);
            }

            _isPaused = true;
        }

        protected override void Unpause()
        {
            System.Diagnostics.Debug.Assert(_isPaused == true);

            _isPaused = false;

            if (!_isMuted)
            {
                _volume = _volumeBuffer;
                _volumeBuffer = 0.0f;
                SetVolumeUnsafe(_volume);
                SetPauseUnsafe(false);
            }
        }

        protected override void Mute()
        {
            System.Diagnostics.Debug.Assert(_isMuted == false);

            if (!_isPaused)
            {
                _volumeBuffer = _volume;
                _volume = 0.0f;
                SetVolumeUnsafe(_volume);
            }

            _isMuted = true;
        }

        protected override void Unmute()
        {
            System.Diagnostics.Debug.Assert(_isMuted == true);

            if (!_isPaused)
            {
                _volume = _volumeBuffer;
                _volumeBuffer = 0.0f;
                SetVolumeUnsafe(_volume);
            }

            _isMuted = false;
        }

        private void SetVolumeUnsafe(float volume)
        {
            SoundNode ptr = m_root.next;

            while (ptr != m_root)
            {
                ptr.instance.setVolume(volume);
                ptr = ptr.next;
            }
        }

        private void SetPauseUnsafe(bool isPaused)
        {
            SoundNode ptr = m_root.next;

            while (ptr != m_root)
            {
                ptr.instance.setPaused(isPaused);
                ptr = ptr.next;
            }
        }
    }
}