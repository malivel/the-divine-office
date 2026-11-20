using Unity.Entities;
using UnityEngine;

public class RouteAuthoring : MonoBehaviour
{
    public float Speed = 2f;
    public Transform[] Points; // сюда перетащим Point1..Point4

    class Baker : Baker<RouteAuthoring>
    {
        public override void Bake(RouteAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Walker
            {
                Speed = authoring.Speed,
                TargetIndex = 0
            });

            var buffer = AddBuffer<Waypoint>(entity);
            foreach (var point in authoring.Points)
            {
                if (point == null) continue;
                DependsOn(point); // если подвинете точку, данные пересоберутся
                buffer.Add(new Waypoint { Position = point.position });
            }
        }
    }
}