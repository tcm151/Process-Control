using ProcessControl.Construction;

namespace ProcessControl.UI
{
    public class BuildPanel : UI_Panel
    {
        override protected void Awake()
        {
            base.Awake();
            
            BuildManager.OnBuildModeChanged += TogglePanel;
        }

        private void TogglePanel(bool truth)
        {
            if (truth) Show();
            else Hide();
        }
    }
}