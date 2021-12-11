using ProcessControl.UI;
using ProcessControl.Procedural;


namespace ProcessControl.User_Interface.Scripts
{
    public class LoadingWindow : UI_Window
    {
        override protected void Awake()
        {
            base.Awake();
            
            CellGrid.onStartWorldGeneration += Show;
            CellGrid.onFinishWorldGeneration += Hide;
        }
    }
}