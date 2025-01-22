namespace Unchord
{
    public abstract class SoundChannel
    {
        public float Volume
        {
            get => _volume;
            set => SetVolume(value);
        }

        public float BufferedVolume => _volumeBuffer;

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (value == _isMuted)
                    return;
                else if (value)
                    Mute();
                else
                    Unmute();
            }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (value == _isPaused)
                    return;
                else if (value)
                    Pause();
                else
                    Unpause();
            }
        }

        protected float _volume;
        protected float _volumeBuffer;
        protected bool _isMuted;
        protected bool _isPaused;
        protected bool _volumeBufferFlag;

        public SoundChannel()
        {
            _volume = 0.5f;
        }

        protected abstract void SetVolume(float volume);
        protected abstract void Pause();
        protected abstract void Unpause();
        protected abstract void Mute();
        protected abstract void Unmute();
    }
}