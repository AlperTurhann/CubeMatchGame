using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public int touchId;

    public LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector2 touchPosition = Input.GetTouch(touchId).position;
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 1f));
        line.SetPosition(1, pos);
    }
}
