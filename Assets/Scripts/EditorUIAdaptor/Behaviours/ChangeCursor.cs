using Riten.Native.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorUIAdaptor.Behaviours
{
    public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public NTCursors cursorTypeInThisDomain;
        
        public void OnPointerEnter(PointerEventData eventData)  => CursorStack.Push(cursorTypeInThisDomain);
        public void OnPointerExit(PointerEventData eventData)   => CursorStack.Pop();
    }
}
