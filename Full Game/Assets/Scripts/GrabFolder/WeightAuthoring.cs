using Unity.Entities;
using UnityEngine;

public class WeightAuthoring : MonoBehaviour
{
    public float Value = 1f;

    class Baker : Baker<WeightAuthoring>
    {
        public override void Bake(WeightAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Weight { Value = authoring.Value });
            AddComponent(entity, new Velocity());
        }
    }
}