using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(GravitySystem))]
public partial struct GrabSystem : ISystem
{
    const float GrabRadius = 2.5f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HandInput>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ref var hand = ref SystemAPI.GetSingletonRW<HandInput>().ValueRW;

        // кнопка отпущена: бросаем то, что держали
        if (!hand.GripPressed)
        {
            hand.Held = Entity.Null;
            return;
        }

        // кнопка зажата, но руки пусты: ищем ближайший кубик
        if (hand.Held == Entity.Null)
        {
            float best = GrabRadius;
            foreach (var (transform, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                              .WithAll<Weight>()
                              .WithEntityAccess())
            {
                float d = math.distance(transform.ValueRO.Position, hand.Position);
                if (d < best)
                {
                    best = d;
                    hand.Held = entity;
                }
            }
        }

        // что-то держим: переносим кубик к руке
        if (hand.Held != Entity.Null)
        {
            SystemAPI.GetComponentRW<LocalTransform>(hand.Held).ValueRW.Position = hand.Position;
            SystemAPI.GetComponentRW<Velocity>(hand.Held).ValueRW.Value = float3.zero;
        }
    }
}