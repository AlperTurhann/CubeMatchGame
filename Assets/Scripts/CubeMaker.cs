using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    X -> -0.5
    Y -> 0.5
    Z -> -0.5
*/

public class CubeMaker : MonoBehaviour
{
    //Oyunun zoruluğuna göre değiştirilebilir
    const int CUBE_MIN_LENGTH = 1;
    const int CUBE_MAX_LENGTH = 3;
    const int COLOR_COUNT = 6;

    [SerializeField] GameObject cube;
    [SerializeField] List<Material> colors;

    GameManager gm;
    public List<int> createdCubeLengths;
    public List<Material> createdCubeMaterials;
    List<GameObject> createdCubes;
    public List<Material> lastMaterials;
    int cubeCount;

    public List<GameObject> allCubes;
    public List<GameObject> multipleCubes;
    public List<GameObject> singleCubes;

    void Start()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        //Küp sayısını alma
        cubeCount = gm.placeHolders.Count;

        //Küpleri oluşturma
        int lastIndex = 0;
        lastIndex = CreateCubes(lastIndex);
        lastIndex = CreateCubes(lastIndex);

        //Eğer ki tüm place holderlar dolu değilse kalanları doldurma
        if (lastIndex != cubeCount) FillCube(lastIndex);

        foreach (GameObject cube in GameObject.FindGameObjectsWithTag("Cube")) allCubes.Add(cube); //Oyunda bulunan tüm küpler
        allCubes.Sort((a, b) => a.name.CompareTo(b.name)); //Küpleri adlarına göre sıralama

        //Küpleri gruplama
        GroupCubes();

        gm.CubeCount = multipleCubes.Count + singleCubes.Count;
    }

    int CreateCubes(int index)
    {
        //Küplerin kesin eşi olsun diye alanı 2 parçaya bölme
        int mustCreateCube = cubeCount / 2;
        //Oluşturulacak küplerin boyutunu ve rengini belirleme
        while (mustCreateCube > 0 && index == 0)
        {
            int randomLength = Random.Range(CUBE_MIN_LENGTH, CUBE_MAX_LENGTH + 1);
            Material randomColor = colors[Random.Range(0, COLOR_COUNT)];
            bool cubeOkey = randomLength <= mustCreateCube;

            //Oluşturulacak küplerin özellikleri üstenilen gibi mi kontrolü
            if (index != 0) while (randomColor == createdCubeMaterials[createdCubeMaterials.Count - 1]) randomColor = colors[Random.Range(0, COLOR_COUNT)];
            while (!cubeOkey)
            {
                randomLength = Random.Range(CUBE_MIN_LENGTH, CUBE_MAX_LENGTH + 1);
                if (randomLength <= mustCreateCube) cubeOkey = true;
            }

            mustCreateCube -= randomLength;
            createdCubeLengths.Add(randomLength);
            createdCubeMaterials.Add(randomColor);
        }

        for (int j = 0; j < createdCubeLengths.Count; j++)
        {
            while (gm.placeHoldersSituation[index]) if (index < cubeCount - 1) index++; else break;
            if (!gm.placeHoldersSituation[index])
            {
                GameObject createdCube = Instantiate(cube, gm.placeHolders[index].transform) as GameObject;
                bool cubeSetted = false;
                int rotIndex = 0; //Boyutuna göre hangi konuma doğru uzayacak onu tutmak için oluşturulan değişken
                if (createdCubeLengths[j] > 1)
                {
                    bool[] cubeIndexsSituation = new bool[createdCubeLengths[j] - 1];
                    while (!cubeSetted)
                    {
                        switch (rotIndex)
                        {
                            case 0: //X
                                //Sıradaki x konumu boş mu durum bilgisi kaydetme
                                for (int x = 1; x < createdCubeLengths[j]; x++)
                                {
                                    if ((index % gm.CubeIndex) + x < gm.CubeIndex) cubeIndexsSituation[x - 1] = !gm.placeHoldersSituation[index + x];
                                    else cubeIndexsSituation[x - 1] = false;
                                }
                                break;
                            case 1: //Z
                                //Sıradaki z konumu boş mu durum bilgisi kaydetme
                                for (int z = 1; z < createdCubeLengths[j]; z++)
                                {
                                    if ((index + (z * gm.CubeIndex)) / gm.CubeIndex < gm.CubeIndex) cubeIndexsSituation[z - 1] = !gm.placeHoldersSituation[index + (z * gm.CubeIndex)];
                                    else cubeIndexsSituation[z - 1] = false;
                                }
                                break;
                            case 2: //Y
                                //Sıradaki y konumu boş mu durum bilgisi kaydetme
                                for (int y = 1; y < createdCubeLengths[j]; y++)
                                {
                                    if ((index + (y * gm.CubeIndex * gm.CubeIndex)) / (gm.CubeIndex * gm.CubeIndex) < gm.CubeIndex) cubeIndexsSituation[y - 1] = !gm.placeHoldersSituation[index + (y * gm.CubeIndex * gm.CubeIndex)];
                                    else cubeIndexsSituation[y - 1] = false;
                                }
                                break;
                            default:
                                //Küpü yerleştirecek yer yoksa
                                createdCubeLengths[index % createdCubeLengths.Count] = CUBE_MIN_LENGTH;
                                createdCubeMaterials[index % createdCubeMaterials.Count] = colors[0];
                                for (int i = 0; i < cubeIndexsSituation.Length; i++) cubeIndexsSituation[i] = true; 
                                break;
                        }

                        cubeSetted = true;
                        foreach (bool situation in cubeIndexsSituation) //Kübün oluşmasına engel varsa diğer rotasyonlara göre kontrolünü sağlamak için ayarlama
                        {
                            if (!situation && rotIndex < 3)
                            {
                                cubeSetted = false;
                                rotIndex++;
                                break;
                            }
                        }
                    }
                }
                createdCube.GetComponent<MeshRenderer>().material = createdCubeMaterials[index % createdCubeMaterials.Count];
                createdCube.name = createdCubeLengths[j] + createdCube.GetComponent<MeshRenderer>().material.name;
                createdCube.name = createdCube.name.Replace(" (Instance)", string.Empty);
                switch (rotIndex) //Kübün boyutunu, konumunu ve materyalini verme
                {
                    case 0: //X
                        createdCube.transform.localScale = new Vector3(createdCubeLengths[j], 1, 1);
                        createdCube.transform.localPosition = new Vector3((createdCubeLengths[j] - 1) * -0.5f, 0, 0);
                        for (int x = 0; x < createdCubeLengths[j]; x++) gm.placeHoldersSituation[index + x] = true;
                        break;
                    case 1: //Z
                        createdCube.transform.localScale = new Vector3(1, 1, createdCubeLengths[j]);
                        createdCube.transform.localPosition = new Vector3(0, 0, (createdCubeLengths[j] - 1) * -0.5f);
                        for (int z = 0; z < createdCubeLengths[j]; z++) gm.placeHoldersSituation[index + (z * gm.CubeIndex)] = true;
                        break;
                    case 2: //Y
                        createdCube.transform.localScale = new Vector3(1, createdCubeLengths[j], 1);
                        createdCube.transform.localPosition = new Vector3(0, (createdCubeLengths[j] - 1) * 0.5f, 0);
                        for (int y = 0; y < createdCubeLengths[j]; y++) gm.placeHoldersSituation[index + (y * gm.CubeIndex * gm.CubeIndex)] = true;
                        break;
                    default:
                        //Küpü yerleştirecek yer yoksa
                        createdCube.transform.localScale = new Vector3(1, 1, 1);
                        createdCube.transform.localPosition = new Vector3(0, 0, 0);
                        gm.placeHoldersSituation[index] = true;
                        createdCube.name = 1 + "Black";
                        break;
                }
                index++;
            }
        }
        return index;
    }

    void FillCube(int index)
    {
        int lastCubesCount = cubeCount - index;

        for (int i = 0; i < lastCubesCount; i++)
        {
            if (!gm.placeHoldersSituation[index + i])
            {
                GameObject createdCube = Instantiate(cube, gm.placeHolders[index + i].transform) as GameObject;
                Material randomColor = colors[Random.Range(0, COLOR_COUNT)];
                if (lastMaterials.Count != 0) while (randomColor == lastMaterials[lastMaterials.Count - 1]) randomColor = colors[Random.Range(0, COLOR_COUNT)];
                lastMaterials.Add(randomColor);

                createdCube.transform.localScale = new Vector3(1, 1, 1);
                createdCube.transform.localPosition = new Vector3(0, 0, 0);
                createdCube.GetComponent<MeshRenderer>().material = lastMaterials[index % lastMaterials.Count];
                gm.placeHoldersSituation[index + i] = true;
                createdCube.name = 1 + createdCube.GetComponent<MeshRenderer>().material.name;
                createdCube.name = createdCube.name.Replace(" (Instance)", string.Empty);
            }
        }
    }

    void GroupCubes()
    {
        int secondCubeIndex = 1;
        while (allCubes.Count > 0)
        {
            if (allCubes.Count > 1 && allCubes[0].name == allCubes[secondCubeIndex].name) //Küpün eşi bulunursa
            {
                multipleCubes.Add(allCubes[0]);
                multipleCubes.Add(allCubes[secondCubeIndex]);

                allCubes.RemoveAt(secondCubeIndex);
                allCubes.RemoveAt(0);

                secondCubeIndex = 0;
            }
            else if (secondCubeIndex >= allCubes.Count - 1) //Küpün eşi yoksa
            {
                singleCubes.Add(allCubes[0]);

                allCubes.RemoveAt(0);
                secondCubeIndex = 0;
            }
            secondCubeIndex++;
        }
    }
}