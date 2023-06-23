using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("關卡"), SerializeField]
    private GameObject[] levels;
    [Header("關卡停留時間"), SerializeField]
    private float stayTime = 15f;

    // 是否已經生成關卡
    private bool isSpawn;
    // 經過的時間
    private float elapsedTime;

    public void SpawnLevel()
    {
        if (levels == null) return;

        int randomIndex = Random.Range(0, levels.Length);

        GameObject level = Instantiate(levels[randomIndex], transform.position, transform.rotation) as GameObject;

        // 移除生成物件時名稱賦予 "Clone"
        level.name = level.name.Replace("(Clone)", "").Trim();

        isSpawn = true;
    }

    private void Update()
    {
        if (!isSpawn) return;

        // 摧毀關卡
        elapsedTime += Time.deltaTime;
        print("elapsed time: " + elapsedTime);

        // 如果 elapsedTime 大於等於 設定好的時間
        if (elapsedTime >= stayTime)
        {
            elapsedTime = 0;
            isSpawn = false;
            Destroy(transform.parent.parent.gameObject);
        }
    }
}
