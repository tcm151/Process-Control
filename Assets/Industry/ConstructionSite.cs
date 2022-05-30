using System;
using System.Threading.Tasks;
using ProcessControl.Industry;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using UnityEngine;

public class ConstructionSite : MonoBehaviour, Buildable
{
    public bool Assembled => this.enabled;
    
    [SerializeField] private Recipe constructionRecipe;

    public Recipe recipe => constructionRecipe;

    public async Task Deliver(Stack stack, float deliveryTime)
    {
        await Alerp.Delay(deliveryTime);
    }

    public async Task Build(float buildTime)
    {
        await Alerp.Delay(buildTime);
        enabled = true;
    }

    public async Task Disassemble(float deconstructionTime)
    {
        await Alerp.Delay(deconstructionTime);
        Destroy(this);
    }
}