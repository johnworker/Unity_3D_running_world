using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorMovement : MonoBehaviour
{
    [Header("����")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float turnSpeed = 0.05f;

    [Header("���D�]�w")]
    [SerializeField]
    private float distance = 5f;
    [SerializeField]
    private int direction = 1;      // 0 = ��, 1 = ��, 2 = �k

}
