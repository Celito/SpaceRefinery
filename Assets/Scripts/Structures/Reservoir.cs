using UnityEngine;
using System.Collections;

public class Reservoir : Structure
{
    protected override void VirtualStart()
    {
        base.VirtualStart();
        Opened = true;
        Capacity = 700.0;
    }
}
