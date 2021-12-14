using System;
using System.Threading.Tasks;
using ProcessControl.Industry;
using ProcessControl.Jobs;
using ProcessControl.Tools;
using UnityEngine;

public class ConstructionSite : MonoBehaviour, Buildable
{
    [SerializeField] private Recipe constructionRecipe;

    public Recipe recipe => constructionRecipe;

    public Task Deliver(Stack stack)
    {
        return Task.CompletedTask;
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