using FMOD;
using FMOD.Studio;

namespace Unchord
{
    public sealed class SingleSoundChannel : SoundChannel
    {
        private string m_eventPath;
        private EventInstance m_soundEventInstance;

        public override void ChangeVolume(float _volume)
        {
            UnityEngine.Debug.Assert(_volume >= 0.0f && _volume <= 1.0f, "Volume should be between 0 and 1");

            m_soundEventInstance.setVolume(_volume);
            base.Volume = _volume;
        }

        public override void SetPause(bool _isPaused)
        {
            m_soundEventInstance.setPaused(_isPaused);
            base.IsPaused = _isPaused;
        }

        public EventInstance ChangeSoundEvent(string _eventPath)
        {
            UnityEngine.Debug.Assert(!base.IsPaused, "Cannot change sound event when sound channel paused.");
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

        public void Play()
        {
            m_soundEventInstance.start();
        }

        public void Stop()
        {
            m_soundEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}