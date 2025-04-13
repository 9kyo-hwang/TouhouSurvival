using System.Collections.Generic;

namespace Unchord
{
    public class CompositePhaseRuntime : PhaseRuntime<List<PhaseDataSO>>
    {
        private int _ptrPrev;
        private int _ptrRuntime;
        private IPhase[] _runtimes;

        public CompositePhaseRuntime(List<PhaseDataSO> phaseList)
        : base(phaseList)
        {
            int count = phaseList.Count;
            UnityEngine.Debug.Assert(count > 0);

            _ptrPrev = -1;
            _ptrRuntime = 0;
            _runtimes = new IPhase[count];

            for (int i = 0; i < count; ++i)
            {
                UnityEngine.Debug.Assert(phaseList[i] != null);
                _runtimes[i] = phaseList[i].CreateRuntime() as IPhase;
            }
        }

        public override RuntimeState Update()
        {
            if (_ptrRuntime == _runtimes.Length)
                return RuntimeState.Halt;
            else if (_ptrRuntime != _ptrPrev)
            {
                _ptrPrev = _ptrRuntime;
                _runtimes[_ptrRuntime].Start();
            }

            RuntimeState execResult = _runtimes[_ptrRuntime].Update();

            switch (execResult)
            {
                case RuntimeState.Continue:
                    // This case has intentionally no operation.
                    return execResult;

                case RuntimeState.Pass:
                    _runtimes[_ptrRuntime++].End();

                    if (_ptrRuntime < _runtimes.Length)
                        return RuntimeState.Continue;
                    else
                        return RuntimeState.Pass;

                case RuntimeState.Fail:
                    _runtimes[_ptrRuntime].End();
                    _ptrRuntime = _runtimes.Length;
                    return RuntimeState.Fail;

                default:
                    // game crash for unknown error.
                    UnityEngine.Debug.Assert(false);
                    return RuntimeState.Halt;
            }
        }

        public override void Pause()
        {
            _runtimes[_ptrRuntime].Pause();
        }

        public override void Resume()
        {
            _runtimes[_ptrRuntime].Resume();
        }

        public override void InterruptHalt()
        {
            _runtimes[_ptrRuntime].InterruptHalt();
        }
    }
}