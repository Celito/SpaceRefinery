public class OldReservoir : OldStructure
{
    protected override void VirtualStart()
    {
        base.VirtualStart();
        Opened = true;
        Capacity = 700.0;
        MaxInputFlow = 40.0;
    }
}
