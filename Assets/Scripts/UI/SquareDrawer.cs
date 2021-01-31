using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDrawer : MonoBehaviour
{
    public delegate void SquareDrawnDelegate(Vector2 start, Vector2 emd);
    public event SquareDrawnDelegate OnSquareDrawn;

    public RectTransform selectRect;
    private LineRenderer lineRenderer;
    public Color color = Color.black;
    private Vector2 start;
    [SerializeField]
    Camera playerCamera;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool drawing = false;

    private readonly int topLeftIdx = 0;
    private readonly int topRightIdx = 1;
    private readonly int bottomRightIdx = 2;
    private readonly int bottomLeftIdx = 3;
    private readonly int closeIdx = 4;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 6;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            startPos = pos;
            start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            RedrawUIRect(start);
            drawing = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drawing = false;
            var min = selectRect.anchoredPosition - (selectRect.sizeDelta / 2);
            var max = selectRect.anchoredPosition + (selectRect.sizeDelta / 2);
            OnSquareDrawn?.Invoke(min, max);
            ReleaseUIRect();
        }

        if (drawing)
        {
            endPos = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RedrawUIRect(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    void RedrawUIRect(Vector2 currentMousePos)
    {
        if (!selectRect.gameObject.activeInHierarchy)
        {
            selectRect.gameObject.SetActive(true);
        }
        var width = currentMousePos.x - start.x;
        var height = currentMousePos.y - start.y;
        selectRect.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectRect.anchoredPosition = start + new Vector2(width / 2, height / 2);
    }

    void ReleaseUIRect()
    {
        selectRect.gameObject.SetActive(false);
    }

    /// <summary>
    /// Draw box using line renderers in world space.
    /// This never really worked the way I wanted it to so I changed to using UI rectangle. Should remove later.
    /// </summary>
    /// <param name="start">Box start in world space</param>
    /// <param name="end">Box end in world space</param>
    void Redraw(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(topLeftIdx, start);
        lineRenderer.SetPosition(topRightIdx, new Vector3(end.x,start.y, start.z));
        lineRenderer.SetPosition(bottomRightIdx, end);
        lineRenderer.SetPosition(bottomLeftIdx, new Vector3(start.x,end.y,end.z));
        lineRenderer.SetPosition(closeIdx, start);
        lineRenderer.SetPosition(5, new Vector3(end.x, start.y, start.z));
    }

    /// <summary>
    /// Get a list of active lines and deactivates them.
    /// Deprecated.
    /// </summary>
    public void Clear()
	{
        lineRenderer.enabled = false;
	}
}
