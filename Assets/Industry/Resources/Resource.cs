﻿using System;
using UnityEngine;


namespace ProcessControl.Machines
{
    public class Resource : MonoBehaviour
    {
        [Serializable] public class Data
        {
            public int ticks;
            public Color color = Color.grey;
            
            public Vector3 position;
        }

        new private SpriteRenderer renderer;

        public Data data;

        public void SetColor(Color newColor) => renderer.color = newColor;
        
        public void SetVisible(bool visible) => renderer.enabled = visible;

        private void Awake()
        {
            data.position = transform.position;
            renderer = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate() => transform.position = data.position;

        // public void OnDrawGizmos()
        // {
        //     Gizmos.color = data.color;
        //     Gizmos.DrawCube(data.position, 0.25f * Vector3.one);
        // }
    }
}