using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("���d"), SerializeField]
    private GameObject[] levels;
    [Header("���d���d�ɶ�"), SerializeField]
    private float stayTime = 15f;

    // �O�_�w�g�ͦ����d
    private bool isSpawn;
    // �g�L���ɶ�
    private float elapsedTime;

    public void SpawnLevel()
    {
        if (levels == null) return;

        int randomIndex = Random.Range(0, levels.Length);

        GameObject level = Instantiate(levels[randomIndex], transform.position, transform.rotation) as GameObject;

        // �����ͦ�����ɦW�ٽᤩ "Clone"
        level.name = level.name.Replace("(Clone)", "").Trim();

        isSpawn = true;
    }

    private void Update()
    {
        if (!isSpawn) return;

        // �R�����d
        elapsedTime += Time.deltaTime;
        print("elapsed time: " + elapsedTime);

        // �p�G elapsedTime �j�󵥩� �]�w�n���ɶ�
        if (elapsedTime >= stayTime)
        {
            elapsedTime = 0;
            isSpawn = false;
            Destroy(transform.parent.parent.gameObject);
        }
    }
}
