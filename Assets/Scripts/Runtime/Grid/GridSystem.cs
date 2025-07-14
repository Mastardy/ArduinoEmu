using System;
using Mastardy.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mastardy.Runtime
{
    public class GridSystem : MonoBehaviour
    {
        private static readonly int baseColor = Shader.PropertyToID("_BaseColor");
        private int gridMask;

        [SerializeField] private GameObject gridPreviewPrefab;

        [SerializeField] private Vector3 tileSize;
        private Vector3 tileOffset => tileSize / 2;

        [SerializeField] private Color freeColor = Color.green;
        [SerializeField] private Color occupiedColor = Color.red;

        private GameObject gridPreviewInstance;

        private void Start()
        {
            gridMask = LayerMask.GetMask("Grid");
            
            gridPreviewInstance = Instantiate(gridPreviewPrefab);
            gridPreviewInstance.SetActive(false);

            InputManager.Instance.SubscribeLayerMask(gridMask);
        }

        private void Update()
        {
            gridPreviewInstance.SetActive(false);

            if (!InputManager.Instance.Colliders.TryGetValue(gridMask, out var instanceCollider)) return;
            if (!instanceCollider.HasValue) return;
            var hit = instanceCollider.Value;

            var hitPoint = hit.point;

            var gridPreview = gridPreviewInstance.transform;
            gridPreview.localScale = tileSize;

            var gridPos = Vector3.zero;
            gridPos.x = Mathf.Floor(hitPoint.x / tileSize.x) * tileSize.x + tileOffset.x;
            gridPos.y = 0;
            gridPos.z = Mathf.Floor(hitPoint.z / tileSize.z) * tileSize.z + tileOffset.z;

            gridPreviewInstance.SetActive(true);
            gridPreview.position = gridPos;
            var gridPreviewRenderer = gridPreviewInstance.GetComponent<Renderer>();

            var mpb = new MaterialPropertyBlock();
            gridPreviewRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(baseColor, Collides(gridPreview) ? occupiedColor : freeColor);
            gridPreviewRenderer.SetPropertyBlock(mpb);
        }

        private bool Collides(Transform obj)
        {
            var collisions = new Collider[10];
            var size = Physics.OverlapBoxNonAlloc(obj.position, obj.localScale / 2, collisions, obj.rotation);

            for (var i = 0; i < collisions.Length; i++)
            {
                var col = collisions[i];
                if (!col) continue;
                if (col.gameObject == gridPreviewInstance) continue;
                if (col.gameObject.layer == LayerMask.NameToLayer("Grid")) continue;
                return true;
            }

            return false;
        }
    }
}