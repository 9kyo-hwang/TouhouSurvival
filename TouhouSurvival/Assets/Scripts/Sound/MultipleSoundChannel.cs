using FMOD;
using FMOD.Studio;

namespace Unchord
{
    public sealed class MultipleSoundChannel : SoundChannel
    {
        public int Count => m_count;

        private Node m_root;
        private int m_count = 0;

        private class Node
        {
            public Node prev;
            public Node next;
            public EventInstance instance;
        }

        public MultipleSoundChannel()
        {
            m_root = new Node();
            m_root.prev = m_root;
            m_root.next = m_root;
        }

        public override void ChangeVolume(float _volume)
        {
            UnityEngine.Debug.Assert(_volume >= 0.0f && _volume <= 1.0f, "Volume should be between 0 and 1");

            Node ptr = m_root.next;

            while (ptr != m_root)
            {
                ptr.instance.setVolume(_volume);
                ptr = ptr.next;
            }

            base.Volume = _volume;
        }

        public override void SetPause(bool _isPaused)
        {
            Node ptr = m_root.next;

            while(ptr != m_root)
            {
                ptr.instance.setPaused(_isPaused);
                ptr = ptr.next;
            }

            base.IsPaused = _isPaused;
        }

        public EventInstance AddSoundEvent(string _eventPath)
        {
            UnityEngine.Debug.Assert(!base.IsPaused, "Cannot add sound event when sound channel paused.");
            UnityEngine.Debug.Assert(_eventPath != null, "Sound Event cannot be null.");

            Node node = new Node();
            node.instance = FMODUnity.RuntimeManager.CreateInstance(_eventPath);
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
            Node ptr = m_root.next;

            while(ptr != m_root)
            {
                Node next = ptr.next;

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
    }
}