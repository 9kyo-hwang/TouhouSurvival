namespace Unchord
{
    public class SoundManager : Singleton<SoundManager>
    {
        public SingleSoundChannel BGM { get; private set; }
        public MultipleSoundChannel SFX { get; private set; }

        private void Awake()
        {
            BGM = new SingleSoundChannel();
            SFX = new MultipleSoundChannel();
        }

        private void Update()
        {
            SFX.OnUpdate();
        }
    }
}