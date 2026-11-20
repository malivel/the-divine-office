using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandBridge : MonoBehaviour
{
    public InputActionProperty gripAction; // кнопка хвата

    EntityManager em;
    Entity handEntity;
    bool lastGrip;

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
        bool grip = gripAction.action.IsPressed();

        // временная проверка: хват на клавише F
        if (Keyboard.current != null && Keyboard.current.fKey.isPressed)
            grip = true;

        if (grip != lastGrip)
        {
            Debug.Log("Grip = " + grip);
            lastGrip = grip;
        }

        var data = em.GetComponentData<HandInput>(handEntity);
        data.Position = transform.position + transform.forward * 0.6f;
        data.GripPressed = grip;
        em.SetComponentData(handEntity, data);
    }
}