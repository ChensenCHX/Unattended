using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.InGameUI
{
    public class ChangeWindowOrder : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;

        private readonly List<RaycastResult> raycastResults = new();
        private void Update()
        {
            // change render order
            
            if (Input.GetMouseButtonDown(0))
            {
                var pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
                raycastResults.Clear();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                foreach (var result in raycastResults)
                {
                    var current = result.gameObject.transform;
                    while (current != null && current.parent != transform) current = current.parent;
                    
                    if (current == null || current.parent != transform) continue;
                    current.SetAsLastSibling(); break;
                }
            }
        }
    }
}
