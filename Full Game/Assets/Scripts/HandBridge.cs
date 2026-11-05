using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandBridge : MonoBehaviour
{
    public InputActionProperty gripAction; // кнопка хвата

    EntityManager em;
    Entity handEntity;

    void OnEnable()
    {
        gripAction.action.Enable();
    }

    void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        handEntity = em.CreateEntity(typeof(HandInput));
        em.SetComponentData(handEntity, new HandInput { Held = Entity.Null });
    }

    void Update()
    {
        var data = em.GetComponentData<HandInput>(handEntity);
        data.Position = transform.position + transform.forward * 0.6f;
        data.GripPressed = gripAction.action.IsPressed();
        em.SetComponentData(handEntity, data);
    }
}