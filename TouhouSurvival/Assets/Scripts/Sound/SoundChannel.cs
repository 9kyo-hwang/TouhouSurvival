namespace Unchord
{
    public abstract class SoundChannel
    {
        public float Volume
        {
            get => m_volume;
            protected set
            {
                m_volume = value;

                if(!this.m_isMuted)
                    m_volumeBuffer = value;
            }
        }

        public float BufferedVolume => m_volumeBuffer;

        protected float VolumeBuffer { get; set; } = 0.5f;
        public bool IsPaused { get; protected set; } = false;
        public bool IsMuted
        {
            get => m_isMuted;
            set => this.SetMute(value);
        }

        private bool m_isMuted;
        private float m_volume;
        private float m_volumeBuffer;

        public abstract void ChangeVolume(float _volume);
        public abstract void SetPause(bool _isPaused);

        public SoundChannel()
        {
            this.Volume = 0.5f;
        }

        public void SetMute(bool _isMuted)
        {
            if (m_isMuted = _isMuted)
                this.ChangeVolume(0.0f);
            else
                this.ChangeVolume(this.m_volumeBuffer);
        }
    }
}