namespace DOTSTemplate
{
    public static class ControlsTempAccess
    {
        private static Controls controls;

        public static Controls Controls => controls ??= new Controls();

        public static void Dispose()
        {
            controls?.Dispose();
        }
    }
}