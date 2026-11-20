using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct WalkSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, walker, points) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Walker>, DynamicBuffer<Waypoint>>())
        {
            if (points.Length == 0) continue;

            float3 target = points[walker.ValueRO.TargetIndex].Position;
            float3 toTarget = target - transform.ValueRO.Position;
            float distance = math.length(toTarget);
            float step = walker.ValueRO.Speed * dt;

            if (distance <= step)
            {
                // дошли: встаём ровно в точку и выбираем следующую
                transform.ValueRW.Position = target;
                walker.ValueRW.TargetIndex = (walker.ValueRO.TargetIndex + 1) % points.Length;
            }
            else
            {
                // идём к точке на один шаг
                transform.ValueRW.Position += toTarget / distance * step;
            }
        }
    }
}