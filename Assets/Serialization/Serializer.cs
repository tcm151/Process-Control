using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProcessControl.Industry;
using ProcessControl.Procedural;
using ProcessControl.Tools;
using UnityEngine;

namespace ProcessControl.Serialization
{
    public class Serializer : Service
    {
        private ConstructionManager constructionManager;
        
        private readonly List<Serializeable> thingsToSerialize = new List<Serializeable>();

        private void Start()
        {
            constructionManager = ServiceManager.Current.RequestService<ConstructionManager>();
        }

        public void Register(Serializeable serializeable)
        {
            if (thingsToSerialize.Contains(serializeable))
            {
                Debug.Log("Serializing object more than once.");
                return;
            }
            thingsToSerialize.Add(serializeable);            
        }

        public void SerializeToJson()
        {
            var smelter = ItemFactory.GetSchematic("Smelter");
            constructionManager.PlaceNode(smelter, CellGrid.GetCellAtCoordinates(Vector2Int.zero), false);
        }
    }
}
