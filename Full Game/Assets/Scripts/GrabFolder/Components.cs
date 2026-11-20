using Unity.Entities;
using Unity.Mathematics;

public struct Weight : IComponentData
{
    public float Value;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}

public struct HandInput : IComponentData
{
    public float3 Position;   // где рука
    public bool GripPressed;  // зажата ли кнопка
    public Entity Held;       // какой кубик сейчас в руке
    //public float3 Offset;     // где кубик относительно руки в момент хвата
}