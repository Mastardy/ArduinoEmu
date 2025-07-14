using System.Collections.Generic;
using UnityEngine;
using Mastardy.Runtime.Utils;
using UnityEngine.InputSystem;

namespace Mastardy.Runtime
{
    public class InputManager : MonoSingleton<InputManager>
    {
        [SerializeField] private Camera cam;

        private HashSet<int> subscribedLayerMasks = new();
        public Dictionary<int, RaycastHit?> Colliders { get; } = new();

        private void Update()
        {
            var worldRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            foreach (var layerMask in subscribedLayerMasks)
            {
                var hasHit = Physics.Raycast(worldRay.origin, worldRay.direction, out var hit, 100f, layerMask);

                Colliders[layerMask] = hasHit ? hit : null;
            }
        }

        public void SubscribeLayerMask(LayerMask layerMask)
        {
            if (!subscribedLayerMasks.Add(layerMask)) return;

            Colliders[layerMask] = null;
        }
    }
}