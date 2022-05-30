using ProcessControl.Industry;


namespace ProcessControl.UI
{
    public class ConstructionPanel : UI_Panel
    {
        protected override void Awake()
        {
            base.Awake();
            ConstructionManager.OnBuildModeChanged += TogglePanel;
        }

        private void TogglePanel(bool truth)
        {
            if (truth) Show();
            else Hide();
        }
    }
}