using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public const int TicksPerSecond = 64;
    
    public int speed = 16;
    
    public ConveyorBelt previous, next;

    private int ticks;
    private int numberOfItems;
    private int maxItems;

    private void Awake()
    {
        ticks = 0;
        numberOfItems = 0;
    }

    private void FixedUpdate()
    {
        ticks++;

        if (ticks >= 16)
        {
            PopLastItem();
        }

        if (numberOfItems < maxItems)
        {
            PullNextItem();
        }
    }


    public void PullNextItem()
    {
        
    }

    public void PopLastItem()
    {
        
    }
}

public class BeltItem
{
    
}