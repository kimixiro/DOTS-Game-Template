using System;

namespace DOTSTemplate
{
    public class InputServices : IInputServices, IDisposable
    {
        private readonly Controls _controls;

        public Controls.DebugActions Debug => _controls.Debug;
        public Controls.PlayerActions Player => _controls.Player;
        public Controls.UIActions UI => _controls.UI;


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