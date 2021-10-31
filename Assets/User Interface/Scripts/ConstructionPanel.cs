using ProcessControl.Construction;

namespace ProcessControl.UI
{
    public class ConstructionPanel : UI_Window
    {
        override public void GoBack() => Hide();

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