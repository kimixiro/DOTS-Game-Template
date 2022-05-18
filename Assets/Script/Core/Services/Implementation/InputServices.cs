using System;

namespace DOTSTemplate
{
    public class InputServices : IInputServices, IDisposable
    {
        private readonly Controls _controls;

        public Controls.PlayerActions playerActions => _controls.Player;
        public Controls.DebugActions debugActions => _controls.Debug;

        public InputServices()
        {
            _controls = new Controls();
            _controls.Enable();
        }

        public void Dispose()
        {
            _controls?.Dispose();
        }

       
    }
}