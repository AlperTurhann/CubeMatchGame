using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRotater : MonoBehaviour
{
    CubeRotater rotater;

    void Start()
    {
        rotater = GameObject.FindWithTag("MainCube").GetComponent<CubeRotater>();
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Wall") rotater.IsActive = !rotater.IsActive;
            }
        }
    }
}
