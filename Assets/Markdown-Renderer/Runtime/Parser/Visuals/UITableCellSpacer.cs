using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MarkdownToUnity.Runtime
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class UITableCellSpacer : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector2 lastSize;

        public List<List<LayoutElement>> tableCellElements;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            lastSize = rectTransform.rect.size;
        }


        private void OnRectTransformDimensionsChange()
        {
            Vector2 newSize = rectTransform.rect.size;

            if (newSize != lastSize)
            {
                lastSize = newSize;
                OnSizeChanged(newSize);
            }
        }

        private void OnSizeChanged(Vector2 newSize)
        {
            Calculate();
        }

       public void Calculate()
        {
            if (tableCellElements == null || tableCellElements.Count == 0)
                return;

            int rowCount = tableCellElements.Count;
            int colCount = 0;

            foreach (var row in tableCellElements)
            {
                if (row.Count > colCount)
                    colCount = row.Count;
            }


            float[] colWidths = new float[colCount];

            for (int col = 0; col < colCount; col++)
            {
                float maxWidth = 0f;
                for (int row = 0; row < rowCount; row++)
                {
                    if (col < tableCellElements[row].Count)
                    {
                        LayoutElement elem = tableCellElements[row][col];
                        if (elem != null)
                        {
                            float preferred = elem.preferredWidth > 0 ? elem.preferredWidth : elem.minWidth;
                            maxWidth = Mathf.Max(maxWidth, preferred);
                        }
                    }
                }
                colWidths[col] = maxWidth;
            }

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    if (col < tableCellElements[row].Count)
                    {
                        LayoutElement elem = tableCellElements[row][col];
                        if (elem != null)
                        {
                            elem.minWidth = colWidths[col];
                            elem.preferredWidth = colWidths[col];
                        }
                    }
                }
            }
        }

    }
}
