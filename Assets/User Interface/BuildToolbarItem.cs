using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Building;
using ProcessControl.Machines;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public Entity buildItem;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                BuildManager.SetBuildItem(buildItem);
                BuildManager.SetConveyorMode(buildItem is Conveyor);
            });
        }
    }
}
