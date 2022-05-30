using ProcessControl.UI;
using ProcessControl.Procedural;


namespace ProcessControl.User_Interface.Scripts
{
    public class LoadingWindow : UI_Window
    {
        protected override void Awake()
        {
            base.Awake();
            
            CellGrid.onStartWorldGeneration += Show;
            CellGrid.onFinishWorldGeneration += Hide;
        }
    }
}