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

        [SerializeField] private TMP_Dropdown recipeDropdown;

        private bool buildMode;

        override protected void Awake()
        {
            base.Awake();
            ConstructionManager.OnBuildModeChanged += (isEnabled) => buildMode = isEnabled;
            Hide();

            recipeDropdown.onValueChanged.AddListener(value =>
            {
                var machine = selectedMachine.machine;
                machine.currentRecipe = machine.recipes[value];
            });
        }
        
        

        private void UpdateInventory()
        {
            // Debug.Log("UPDATING INVENTORY!>>>");
            
            if (selectedMachine.machine.inputInventoryTest.Count >= 1)
            {
                inputIcon.enabled = true;
                inputIcon.sprite = selectedMachine.machine.inputInventoryTest.FirstItem.sprite;
                inputCount.text = selectedMachine.machine.inputInventoryTest.Count.ToString();
            }
            else
            {
                inputIcon.enabled = false;
                inputCount.text = "";
            }

            if (selectedMachine.machine.outputInventoryTest.Count >= 1)
            {
                outputIcon.enabled = true;
                outputIcon.sprite = selectedMachine.machine.outputInventoryTest.FirstItem.sprite;
                outputCount.text = selectedMachine.machine.outputInventoryTest.Count.ToString();
            }
            else
            {
                outputIcon.enabled = false;
                outputCount.text = "";
            }
        }

        private void Update()
        {
            if (buildMode) return;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var selectedCell = TileGrid.GetCellUnderMouse();
                if (selectedCell is {occupied: true, node: Machine machine})
                {
                    selectedMachine = machine;
                    selectedMachine.machine.inputInventoryTest.onModified += UpdateInventory;
                    selectedMachine.machine.outputInventoryTest.onModified += UpdateInventory;
                    
                    recipeDropdown.ClearOptions();
                    var dropdownOptions = selectedMachine.machine.recipes.ConvertAll(r => r.name);
                    recipeDropdown.AddOptions(dropdownOptions);
                    
                    recipeDropdown.transform.parent.gameObject.SetActive(selectedMachine.machine.recipes.Count >= 1);

                    recipeDropdown.value = selectedMachine.machine.recipes.IndexOf(selectedMachine.machine.currentRecipe);
                    
                    UpdateInventory();
                    Show();
                }
            }
        }
    }
}
