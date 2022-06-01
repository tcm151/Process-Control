using UnityEngine;


namespace ProcessControl.Tools
{
    public class Service : MonoBehaviour
    {
        virtual protected void Awake() => ServiceManager.RegisterService(this);
        virtual protected void OnDestroy() => ServiceManager.RemoveService(this);
    }
}