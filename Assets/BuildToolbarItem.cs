using System;
using ProcessControl.Building;
using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public MonoBehaviour buildItem;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => BuildManager.SetBuildItem(buildItem));
        }
    }
}
