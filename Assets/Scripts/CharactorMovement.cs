using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorMovement : MonoBehaviour
{
    [Header("移動")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float turnSpeed = 0.05f;

    [Header("車道設定")]
    [SerializeField]
    private float distance = 5f;
    [SerializeField]
    private int direction = 1;      // 0 = 左, 1 = 中, 2 = 右

}
