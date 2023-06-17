using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelector : MonoBehaviour
{
    [SerializeField] GameObject clickEffect;

    GameManager gm;
    CubeMaker cubeMaker;
    GameObject wall;
    GameObject effect;
    List<GameObject> selectedCubes = new List<GameObject>();
    int cubeTouch;
    bool cubeDetected = false;

    void Start()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        wall = GameObject.FindWithTag("Wall");
        cubeMaker = GameObject.FindWithTag("GameManager").GetComponent<CubeMaker>();
    }

    void Update()
    {
        if (!cubeDetected)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[i].position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (Input.GetTouch(i).phase == TouchPhase.Began && hit.transform.tag == "Cube")
                    {
                        effect = Instantiate(clickEffect, hit.transform.position, hit.transform.rotation) as GameObject;
                        selectedCubes.Add(hit.transform.gameObject);
                        selectedCubes[0].GetComponent<MeshRenderer>().material.SetFloat("_Metallic", 1f);
                        cubeTouch = Input.GetTouch(i).fingerId;
                        effect.GetComponent<FollowCursor>().touchId = cubeTouch;
                        effect.GetComponent<FollowCursor>().line.SetPosition(0, hit.transform.position);
                        cubeDetected = true;
                        break;
                    }
                }
            }
        }
        else
        {
            if (Input.GetTouch(cubeTouch).phase == TouchPhase.Ended)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(cubeTouch).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Cube") selectedCubes.Add(hit.transform.gameObject);
                    else selectedCubes.Add(wall);
                }

                cubeDetected = false;
                Destroy(effect);

                CheckSelections();
            }
        }
    }

    void CheckSelections()
    {
        if (selectedCubes[0] != selectedCubes[1] && selectedCubes[0].name == selectedCubes[1].name) //Eşli olan küpler seçilmişse
        {
            //Seçilen küpler aynıysa ama farklı listelerdeyse listeler arası transfer yapılması
            int index = cubeMaker.singleCubes.IndexOf(selectedCubes[0]); //İlk seçimin kontrolü
            if (index != -1)
            {
                cubeMaker.multipleCubes.Add(cubeMaker.singleCubes[index]);
                cubeMaker.singleCubes.RemoveAt(index);
                for (int i = 0; i < cubeMaker.multipleCubes.Count; i++)
                {
                    if (selectedCubes[0].name == cubeMaker.multipleCubes[i].name && selectedCubes[1] != cubeMaker.multipleCubes[i])
                    {
                        cubeMaker.singleCubes.Add(cubeMaker.multipleCubes[i]);
                        cubeMaker.multipleCubes.RemoveAt(i);
                        break;
                    }
                }
            }
            index = cubeMaker.singleCubes.IndexOf(selectedCubes[1]); //İkinci seçimin kontrolü
            if (index != -1)
            {
                cubeMaker.multipleCubes.Add(cubeMaker.singleCubes[index]);
                cubeMaker.singleCubes.RemoveAt(index);
                for (int i = 0; i < cubeMaker.multipleCubes.Count; i++)
                {
                    if (selectedCubes[1].name == cubeMaker.multipleCubes[i].name && selectedCubes[0] != cubeMaker.multipleCubes[i])
                    {
                        cubeMaker.singleCubes.Add(cubeMaker.multipleCubes[i]);
                        cubeMaker.multipleCubes.RemoveAt(i);
                        break;
                    }
                }
            }

            //Listeden küpleri çıkarma
            cubeMaker.multipleCubes.Remove(selectedCubes[0]);
            cubeMaker.multipleCubes.Remove(selectedCubes[1]);

            gm.CubeDestroyer(selectedCubes[0], selectedCubes[1]); //Küpleri yok etme

            ClearSelections(); //Seçimleri temizleme
        }
        else if (selectedCubes[0] != selectedCubes[1]) ClearSelections(); //Yanlış seçim yapılmışsa
        else //Eşi olmayan küp seçilmişse
        {
            int index = cubeMaker.singleCubes.IndexOf(selectedCubes[0]);
            if (index != -1) //Küp listede var mı kontrolü
            {
                cubeMaker.singleCubes.Remove(selectedCubes[0]);
                gm.CubeDestroyer(selectedCubes[0]); //Kübü yok etme
            }

            ClearSelections(); //Seçimleri temizleme
        }
    }

    void ClearSelections()
    {
        selectedCubes[0].GetComponent<MeshRenderer>().material.SetFloat("_Metallic", 0f);
        for (int i = selectedCubes.Count - 1; i >= 0; i--) selectedCubes.RemoveAt(i);
    }
}
