using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct WalkSystem : ISystem
{
    const float YieldRadius = 1.5f; // на каком расстоянии от главного куба стоим
    const float LookAhead = 1.5f;   // насколько далеко вперёд смотрим

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        var query = SystemAPI.QueryBuilder().WithAll<Walker, LocalTransform>().Build();
        var entities = query.ToEntityArray(Allocator.Temp);
        var positions = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);

        foreach (var (transform, walker, points, self) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Walker>, DynamicBuffer<Waypoint>>()
                          .WithEntityAccess())
        {
            if (points.Length == 0) continue;

            // куда идём и какая точка находится впереди нас
            float3 target = points[walker.ValueRO.TargetIndex].Position;
            float3 toTarget = target - transform.ValueRO.Position;
            float distance = math.length(toTarget);

            float3 ahead = transform.ValueRO.Position;
            if (distance > 0.0001f)
                ahead += toTarget / distance * math.min(LookAhead, distance);

            // есть ли главнее куб рядом со мной или рядом с точкой впереди?
            bool mustWait = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index >= self.Index) continue;

                float3 other = positions[i].Position;
                if (math.distance(other, transform.ValueRO.Position) < YieldRadius ||
                    math.distance(other, ahead) < YieldRadius)
                {
                    mustWait = true;
                    break;
                }
            }
            if (mustWait) continue;

            // двигаемся к цели
            float step = walker.ValueRO.Speed * dt;
            if (distance <= step)
            {
                transform.ValueRW.Position = target;
                walker.ValueRW.TargetIndex = (walker.ValueRO.TargetIndex + 1) % points.Length;
            }
            else
            {
                transform.ValueRW.Position += toTarget / distance * step;
            }
        }
    }
}