using UnityEngine;
using UnityEngine.UI;
using ProcessControl.Graphs;
using ProcessControl.Building;
using ProcessControl.Machines;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public Machine machine;
        public Conveyor conveyor;
        public bool isConveyor;
        
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            button.onClick.AddListener(() =>
            {
                BuildManager.SetMachine(machine);
                BuildManager.SetConveyor(conveyor);
                BuildManager.SetConveyorMode(isConveyor);
            });
        }
    }
}
