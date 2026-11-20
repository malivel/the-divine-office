using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(GravitySystem))]
public partial struct GrabSystem : ISystem
{
    const float GrabRadius = 6f;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HandInput>();
    }

    public void OnUpdate(ref SystemState state)
    {
        ref var hand = ref SystemAPI.GetSingletonRW<HandInput>().ValueRW;

        if (!hand.GripPressed)
        {
            hand.Held = Entity.Null;
            return;
        }

        if (hand.Held == Entity.Null)
        {
            float best = GrabRadius;
            float nearest = 9999f;
            int count = 0;

            foreach (var (transform, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                              .WithAll<Weight>()
                              .WithEntityAccess())
            {
                count++;
                float d = math.distance(transform.ValueRO.Position, hand.Position);
                nearest = math.min(nearest, d);
                if (d < best)
                {
                    best = d;
                    hand.Held = entity;
                }
            }

            if (hand.Held == Entity.Null)
            {
                UnityEngine.Debug.Log("Не схвачено. Кубиков: " + count +
                    ", до ближайшего: " + nearest + " м, рука: " + hand.Position);
            }
            else
            {
                UnityEngine.Debug.Log("Схвачен кубик");
            }
        }

        if (hand.Held != Entity.Null)
        {
            SystemAPI.GetComponentRW<LocalTransform>(hand.Held).ValueRW.Position = hand.Position;
            SystemAPI.GetComponentRW<Velocity>(hand.Held).ValueRW.Value = float3.zero;
        }
    }
}