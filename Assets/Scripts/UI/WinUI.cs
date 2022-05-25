using Common;

namespace UI
{
    public class WinUI : UnitySingleton<WinUI>
    {
        public ToggleShowUI toggle;
        private void Awake()
        {
            toggle = gameObject.AddComponent<ToggleShowUI>();
        }
    }
}