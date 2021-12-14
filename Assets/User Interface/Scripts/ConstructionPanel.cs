using ProcessControl.Industry;


namespace ProcessControl.UI
{
    public class ConstructionPanel : UI_Panel
    {
        override protected void Awake()
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