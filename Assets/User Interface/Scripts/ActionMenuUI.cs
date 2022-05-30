using System;
using ProcessControl.Controls;


namespace ProcessControl.UI
{
    public class ActionMenuUI : UI_Panel
    {
        protected override void Awake()
        {
            base.Awake();

            ObjectSelector.onCloseActionMenu += () =>
            {
                Hide();
            };
            
            ObjectSelector.onOpenActionMenu += (position) =>
            {
                transform.position = position;
                Show();
            };
        }
    }
}
