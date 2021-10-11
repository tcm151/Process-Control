using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;

[InitializeOnLoad]
#endif
abstract public class MonoScriptableObject : ScriptableObject
{
    static MonoScriptableObject()
    {
        // throw new System.NotImplementedException();
    }

    abstract protected void OnBegin();
    virtual protected void OnEnd() { }

    #if UNITY_EDITOR
        protected void OnEnable() => EditorApplication.playModeStateChanged += OnPlayStateChange;
        protected void OnDisable() => EditorApplication.playModeStateChanged -= OnPlayStateChange;

        private void OnPlayStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode) OnBegin();
            else if (state == PlayModeStateChange.ExitingPlayMode) OnEnd();
        }
    #else
        protected void OnEnable() => OnBegin();
        protected void OnDisable() => OnEnd();
    #endif
}