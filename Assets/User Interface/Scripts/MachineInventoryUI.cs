using ProcessControl.Graphs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProcessControl.UI;
using ProcessControl.Industry;
using ProcessControl.Procedural;


namespace ProcessControl.UI
{
    public class MachineInventoryUI : UI_DraggablePanel
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TMP_Dropdown recipeDropdown;
        
        [SerializeReference] private Node selectedNode;
        [SerializeReference] private Machine selectedMachine;
        [SerializeReference] public Transform inputSlotsContainer;
        [SerializeReference] public Transform outputSlotsContainer;

        private bool buildMode;
        private InventorySlotUI[] ioInputSlots;
        private InventorySlotUI[] ioOutputSlots;
        // private InventorySlotUI[] inventorySlots;
        
        protected override void Awake()
        {
            base.Awake();
            ConstructionManager.OnBuildModeChanged += (isEnabled) => buildMode = isEnabled;
            Hide();

            recipeDropdown.onValueChanged.AddListener(value => selectedMachine.currentRecipe = selectedMachine.recipes[value]);

            ioInputSlots = inputSlotsContainer.GetComponentsInChildren<InventorySlotUI>();
            ioOutputSlots = outputSlotsContainer.GetComponentsInChildren<InventorySlotUI>();
        }
        
        private void Update()
        {
            if (buildMode) return;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var selectedCell = CellGrid.GetCellUnderMouse();
                if (selectedCell is {occupied: true, node: Machine machine})
                {
                    selectedMachine = machine;
                    selectedMachine.inputInventory.onModified += UpdateInventory;
                    selectedMachine.outputInventory.onModified += UpdateInventory;
                    
                    title.text = selectedMachine.name;
                    
                    recipeDropdown.ClearOptions();
                    var dropdownOptions = selectedMachine.recipes.ConvertAll(r => r.name);
                    recipeDropdown.AddOptions(dropdownOptions);
                    
                    recipeDropdown.transform.parent.gameObject.SetActive(selectedMachine.recipes.Count >= 1);
                    recipeDropdown.value = selectedMachine.recipes.IndexOf(selectedMachine.currentRecipe);
                    
                    UpdateInventory();
                    Show();
                }
            }
        }
        
        private void UpdateInventory()
        {
            //- update IO
            var inputItems = selectedMachine.inputInventory.GetItems();
            for (int i = 0; i < ioInputSlots.Length; i++)
            {
                ioInputSlots[i].Set((i < inputItems.Count) ? inputItems[i] : null);
            }

            var outputItems = selectedMachine.outputInventory.GetItems();
            for (int i = 0; i < ioOutputSlots.Length; i++)
            {
                ioOutputSlots[i].Set((i < outputItems.Count) ? outputItems[i] : null);
            }
            
            //- update containers
            
        }

    }
}
