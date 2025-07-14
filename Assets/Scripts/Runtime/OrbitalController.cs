using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mastardy.Runtime
{
    public class OrbitalController : MonoBehaviour
    {
        private int inputMask;

        [SerializeField] private Vector2 orbitRotationSpeed = new(0.4f, 0.4f);
        [SerializeField] private Camera cam;
        
        private Vector3 orbitCenter;
        
        private void Start()
        {
            inputMask = ~LayerMask.GetMask("Ignore Raycast");
            InputManager.Instance.SubscribeLayerMask(inputMask);
        }

        private void Update()
        {
            if (!InputManager.Instance.Colliders.TryGetValue(inputMask, out var instanceCollider)) return;
            if (!instanceCollider.HasValue) return;

            var mouse = Mouse.current;
            var hit = instanceCollider.Value;

            if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame) orbitCenter = hit.point;

            if (mouse.scroll.value.y != 0)
            {
                cam.fieldOfView -= mouse.scroll.value.y;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 10f, 90f);
            }

            if (mouse.leftButton.isPressed)
            {
                var dif = hit.point - orbitCenter;
                dif.y = 0;

                transform.position -= dif;
            }
            else if (mouse.rightButton.isPressed)
            {
                var dif = mouse.delta.value * orbitRotationSpeed;

                var euler = transform.eulerAngles;
                var pos = transform.position;

                transform.RotateAround(orbitCenter, Vector3.up, dif.x);
                transform.RotateAround(orbitCenter, transform.right, -dif.y);

                if (transform.eulerAngles.x < 25)
                {
                    transform.RotateAround(orbitCenter, transform.right, dif.y);
                    transform.RotateAround(orbitCenter, transform.right, 25 - transform.eulerAngles.x);
                }
                else if (transform.eulerAngles.x > 90f || transform.eulerAngles.z > 1)
                {
                    transform.eulerAngles = euler;
                    transform.position = pos;

                    transform.RotateAround(orbitCenter, transform.right, 90 - transform.eulerAngles.x);
                }
            }
        }
    }
}