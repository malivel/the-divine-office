using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct GravitySystem : ISystem
{
    const float Gravity = 9.81f;
    const float FloorY = 0.1f; // пол в y=0 плюс половина высоты куба

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, velocity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Velocity>>()
                          .WithAll<Weight>())
        {
            velocity.ValueRW.Value.y -= Gravity * dt;
            transform.ValueRW.Position += velocity.ValueRO.Value * dt;

            if (transform.ValueRO.Position.y < FloorY)
            {
                transform.ValueRW.Position.y = FloorY;
                velocity.ValueRW.Value = default;
            }
        }
    }
}