using System.Collections;
using System.Collections.Generic;
using ProcessControl.Conveyors;
using UnityEngine;

namespace ProcessControl
{
    public class Machine : Node
    {
        override public void DepositResource(Resource resource) { }
        override public Resource WithdrawResource() => null;
    }
}
