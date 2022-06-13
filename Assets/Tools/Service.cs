using UnityEngine;


namespace ProcessControl.Tools
{
    abstract public class Service : MonoBehaviour
    {
        abstract public void Initialize();
        
        virtual protected void Awake() => ServiceManager.RegisterService(this);
        virtual protected void OnDestroy() => ServiceManager.RemoveService(this);
    }
}