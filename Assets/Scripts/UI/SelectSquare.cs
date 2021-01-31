using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSquare : MonoBehaviour
{
    [SerializeField]
    private Line topLine;
    [SerializeField]
    private Line bottomLine;
    [SerializeField]
    private Line leftLine;
    [SerializeField]
    private Line rightLine;
    private Vector2 startPosition;
    private Vector2 endPosition;
    [SerializeField]
    private float width = 3f;


    public Vector2 StartPosition { get { return startPosition; } set { startPosition = value; Redraw(); } }
    public Vector2 EndPosition { get { return endPosition; } set { endPosition = value; Redraw(); } }

    private void Redraw() 
    {
        var heading = endPosition - startPosition;
        var distance = heading.magnitude;
        var direction = heading / distance;

        Vector3 centerPos = new Vector3(startPosition.x + endPosition.x, startPosition.y + endPosition.y) / 2;
        topLine.start = startPosition;
        topLine.end = new Vector2(endPosition.x,startPosition.y);
        bottomLine.start = new Vector2(startPosition.x, endPosition.y);
        bottomLine.end = endPosition;
        leftLine.start = startPosition;
        leftLine.end = new Vector2(startPosition.x,endPosition.y);
        rightLine.start = new Vector2(endPosition.x, startPosition.y);
        rightLine.end = endPosition;
        topLine.width = 10;
        bottomLine.width = 10;
        //Debug.Log($"{topLine.start} , {bottomLine.end}");
        //lineRenderer.transform.position = centerPos;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //lineRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //var objectWidthSize = 10f / 5f; // 10 = pixels of line sprite, 5f = pixels per units of line sprite.
        //lineRenderer.transform.localScale = new Vector3(distance / objectWidthSize, width, lineRenderer.transform.localScale.z);

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
