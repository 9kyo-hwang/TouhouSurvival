using FMOD;
using FMOD.Studio;

namespace Unchord
{
    public sealed class SingleSoundChannel : SoundChannel
    {
        private string m_eventPath;
        private EventInstance m_soundEventInstance;

        public EventInstance ChangeSoundEvent(string _eventPath)
        {
            // UnityEngine.Debug.Assert(!base.IsPaused, "Cannot change sound event when sound channel paused.");
            UnityEngine.Debug.Assert(_eventPath != null, "Sound Event cannot be null.");

            PLAYBACK_STATE playbackState;
            m_soundEventInstance.getPlaybackState(out playbackState);

            // if (_eventPath.Equals(m_eventPath) && playbackState == PLAYBACK_STATE.PLAYING)
            if (_eventPath.Equals(m_eventPath))
                return m_soundEventInstance;

            if (m_soundEventInstance.isValid())
                m_soundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);

            m_eventPath = _eventPath;
            m_soundEventInstance = FMODUnity.RuntimeManager.CreateInstance(_eventPath);

            m_soundEventInstance.setVolume(base.Volume);
            m_soundEventInstance.start();
            return m_soundEventInstance;
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
                m_soundEventInstance.setVolume(_volume);
            }
        }

        protected override void Pause()
        {
            System.Diagnostics.Debug.Assert(_isPaused == false);

            if (!_isMuted)
            {
                _volumeBuffer = _volume;
                _volume = 0.0f;
                m_soundEventInstance.setPaused(true);
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
                m_soundEventInstance.setVolume(_volume);
                m_soundEventInstance.setPaused(false);
            }
        }

        protected override void Mute()
        {
            System.Diagnostics.Debug.Assert(_isMuted == false);

            if (!_isPaused)
            {
                _volumeBuffer = _volume;
                _volume = 0.0f;
                m_soundEventInstance.setVolume(_volume);
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
                m_soundEventInstance.setVolume(_volume);
            }

            _isMuted = false;
        }
    }
}