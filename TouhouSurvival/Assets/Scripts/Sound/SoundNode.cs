using FMOD.Studio;

namespace Unchord
{
    public class SoundNode
    {
        public SoundNode prev;
        public SoundNode next;

        public EventInstance instance;
        public bool isStarted;
    }
}