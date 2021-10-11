using System;
using ProcessControl.Construction;
using ProcessControl.UI;
using ProcessControl.Industry.Machines;
using ProcessControl.Procedural;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl
{
    public class MachineInventoryUI : UI_DraggableWindow
    {
        override public void GoBack() => Hide();

        [SerializeField] private Machine selectedMachine;

        [SerializeField] private TextMeshProUGUI inputCount;
        [SerializeField] private Image inputIcon;
        [SerializeField] private TextMeshProUGUI outputCount;
        [SerializeField] private Image outputIcon;

        private bool buildMode;

        override protected void Awake()
        {
            base.Awake();

            ConstructionManager.OnBuildModeChanged += (isEnabled) => buildMode = isEnabled;
            
            Hide();
        }

        public void UpdateInventory()
        {
            if (selectedMachine.machine.inputInventory.Count >= 1)
            {
                inputIcon.enabled = true;
                inputIcon.sprite = selectedMachine.machine.inputInventory[0].data.sprite;
                inputCount.text = selectedMachine.machine.inputInventory.Count.ToString();
            }
            else
            {
                inputIcon.enabled = false;
                inputCount.text = "";
            }

            if (selectedMachine.machine.outputInventory.Count >= 1)
            {
                outputIcon.enabled = true;
                outputIcon.sprite = selectedMachine.machine.outputInventory[0].data.sprite;
                outputCount.text = selectedMachine.machine.outputInventory.Count.ToString();
            }
            else
            {
                outputIcon.sprite = null;
                outputCount.text = "";
            }
        }

        private void Update()
        {
            if (buildMode) return;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var selectedCell = ProceduralGrid.GetCellUnderMouse();
                if (selectedCell is {occupied: true, node: Machine n})
                {
                    selectedMachine = n;
                    UpdateInventory();
                    selectedMachine.onInventoryModified += UpdateInventory;
                    Show();
                }
            }
        }
    }
}
