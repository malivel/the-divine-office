using Unity.Entities;
using Unity.Mathematics;

// одна точка маршрута
[InternalBufferCapacity(8)]
public struct Waypoint : IBufferElementData
{
    public float3 Position;
}

// данные ходока
public struct Walker : IComponentData
{
    public float Speed;     // скорость, метров в секунду
    public int TargetIndex; // к какой точке по счёту идёт сейчас
}