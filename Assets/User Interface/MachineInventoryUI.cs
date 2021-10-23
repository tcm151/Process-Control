using System;
using System.Collections.Generic;
using System.Linq;
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

        public Transform inputSlotsContainer;
        public Transform outputSlotsContainer;
        
        [SerializeField] private InventorySlotUI inventorySlotPrefab;

        [SerializeField] private TMP_Dropdown recipeDropdown;

        private bool buildMode;
        private InventorySlotUI[] inputSlots;
        private InventorySlotUI[] outputSlots;
        
        
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

            inputSlots = inputSlotsContainer.GetComponentsInChildren<InventorySlotUI>();
            outputSlots = outputSlotsContainer.GetComponentsInChildren<InventorySlotUI>();
        }
        
        

        private void UpdateInventory()
        {
            var inputItems = selectedMachine.machine.inputInventory.GetItems();
            for (int i = 0; i < inputSlots.Length; i++)
            {
                inputSlots[i].Set((i < inputItems.Count) ? inputItems[i] : new KeyValuePair<Item, int>(null, 0));
            }

            var outputItems = selectedMachine.machine.outputInventory.GetItems();
            for (int i = 0; i < outputSlots.Length; i++)
            {
                outputSlots[i].Set((i < outputItems.Count) ? outputItems[i] : new KeyValuePair<Item, int>(null, 0));
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
                    selectedMachine.machine.inputInventory.onModified += UpdateInventory;
                    selectedMachine.machine.outputInventory.onModified += UpdateInventory;
                    
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
