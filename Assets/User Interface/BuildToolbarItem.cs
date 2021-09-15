using System;
using ProcessControl.Building;
using ProcessControl.Conveyors;
using UnityEngine;
using UnityEngine.UI;


namespace ProcessControl.UI
{
    public class BuildToolbarItem : MonoBehaviour
    {
        public Node buildItem;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => BuildManager.SetBuildItem(buildItem));
        }
    }
}
