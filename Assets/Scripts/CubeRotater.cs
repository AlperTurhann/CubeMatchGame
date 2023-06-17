using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotater : MonoBehaviour
{
    const float ROTATE_SPEED = 0.2f;

    bool isActive = false;

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    void Update()
    {

        if (isActive)
        {
            Touch screenTouch = Input.GetTouch(0);
            if (screenTouch.phase == TouchPhase.Moved)
            {
                transform.Rotate(new Vector3(screenTouch.deltaPosition.y, screenTouch.deltaPosition.x, 0f) * ROTATE_SPEED);
            }
            if (screenTouch.phase == TouchPhase.Ended) isActive = false;
        }
    }
}
