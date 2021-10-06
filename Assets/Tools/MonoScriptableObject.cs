using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
 
[InitializeOnLoad]
abstract public class MonoScriptableObject : ScriptableObject
{
    static MonoScriptableObject()
    {
        // throw new System.NotImplementedException();
    }

    abstract protected void Awake();
    virtual protected void OnDestroy() { }

    #if UNITY_EDITOR
        protected void OnEnable() => EditorApplication.playModeStateChanged += OnPlayStateChange;
        protected void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayStateChange;

        private void OnPlayStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode) Awake();
            else if (state == PlayModeStateChange.ExitingPlayMode) OnDestroy();
        }
    #else
        protected void OnEnable() => Awake();
        protected void OnDisable() => OnDestroy();
    #endif
}