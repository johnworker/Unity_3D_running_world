using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorMovement : MonoBehaviour
{
    [Header("--����--")]
    [SerializeField]
    private float speed = 5f;           // ���ʳt��
    [SerializeField]
    private float turnSpeed = 0.05f;    // ����t��

    [Header("--���D�]�w--")]
    [SerializeField]
    private float distance = 5f;        // �Z��
    [SerializeField]
    private int direction = 1;          // ��V (0 = ��, 1 = ��, 2 = �k)

    [Header("--���D�]�w--")]
    [SerializeField]
    private float gravity = 12f;        // ���O
    [SerializeField]
    private float jumpForce = 10f;      // ���_�Ӫ����O

    [Header("--½�u�]�w--")]
    [SerializeField]
    private float rollingTime = 1f;                                     // ½��ɶ�
    [SerializeField]
    private float targetCapsuleHeight = 0.5f;                           // �I���鰪��
    [SerializeField]
    private Vector3 targetCapsuleCenter = new Vector3(0, 3f, 0);        // �I�����m

    [Header("--�a�W�P�w--")]
    [SerializeField]
    private float groundCheckRadius = 0.25f;                            // ���
    [SerializeField]
    private float groundCheckOffset = -0.5f;                            // �I���鰾���q
    [SerializeField]
    private float groundCheckDistance = 0.4f;                           // �Z��
    [SerializeField]
    private LayerMask groundMask;                                       // �B�n


    // �T�O����@����Ĳ��a�O
    private float verticalVelocity = -0.1f;                             // �������O
    private CharacterController characterController;                    // �}������
    private CapsuleCollider capsuleCollider;                            // �I����
    private float capsuleHeight;                                        // �I���鰪��
    private Vector3 capsuleCenter;                                      // �I����xyz
    private Vector3 groundNormal = Vector3.up;                          // �s�W�s�� Vector3
    private bool isRolling;                                             // �]�w���L�ȽT�{�O�_½�u
    private float rollElapsedTime;                                      // ½�u�p�ɾ�

    private void Start()
    {
        // �������
        characterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        // ����I���鰪��
        capsuleHeight = capsuleCollider.height;
        // ����I���� xyz
        capsuleCenter = capsuleCollider.center;
    }

    private void SetMoveDirection(bool isRight)
    {
        // ? �O �]? :�^��²��g�k�A�Ω�ھڱ����ܤ��P���ȶi��B��C
        direction += isRight ? 1 : -1;              // True = 1 , False = -1
        // Clamp �ɩw�d��
        direction = Mathf.Clamp(direction, 0, 2);   // (0 = ��, 1 = ��, 2 = �k)
    }

    private void SetLookDirection()
    {
        // ���ʩ����Ω��k�ɡA���ਤ��
        Vector3 lookDirection = characterController.velocity;       // ����t�v

        if (lookDirection == Vector3.zero) return;                  // �p�G��0 , return

        lookDirection.y = 0;

        // Lerp �ƶ�
        transform.forward = Vector3.Lerp(transform.forward, lookDirection, turnSpeed);
    }

    private void MoveTo()
    {
        // ���e���� �u�ݧ��Z�b �V�e�ʴN�i�H
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        // ���� �� ���k
        if (direction == 0) targetPosition += Vector3.left * distance;
        else if(direction == 2) targetPosition += Vector3.right * distance;

        // �p��xyz���ʼƭ�
        Vector3 movement = Vector3.zero;
        movement.x = (targetPosition - transform.position).normalized.x * speed;    // ���k����
        movement.y = verticalVelocity;                                              // �����t�v
        movement.z = speed;

        // ���ʨ���
        characterController.Move(movement * Time.deltaTime);
    }

    private void Jump()
    {
        // �p�G�b�a�O
        if (CheckGrounded())
        {
            // ���U�ť���
            if (Input.GetKeyDown(KeyCode.Space)) verticalVelocity = jumpForce;
        }
        // �]�w���U�ɶ�
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
        }
    }

    // �T�w����O�_�b�a�W
    private bool CheckGrounded()
    {
        // �M��}�l��m (�g�u�_�I)
        Vector3 start = transform.position + Vector3.up * groundCheckOffset;
        // �����ήg�u start, radius, direction, hit info, distance, layermask (�̷өx�褽��)
        if(Physics.SphereCast(start, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
        {
            groundNormal = hit.normal;
            return true;
        }
        groundNormal = Vector3.up;
        return false;

    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isRolling || !CheckGrounded()) return;
            rollElapsedTime = 0f;
            isRolling = true;
            // �p�G���b�u�� �Y�p�I����
            capsuleCollider.height = targetCapsuleHeight;
            capsuleCollider.center = targetCapsuleCenter;
        }
        // �p�ɬ��
        rollElapsedTime += Time.deltaTime;

        // ���m�Ҧ��� (��ƨ�A�^�k���`�j�p)
        if(rollElapsedTime>= rollingTime && isRolling)
        {
            capsuleCollider.height = targetCapsuleHeight;
            capsuleCollider.center = targetCapsuleCenter;
            rollElapsedTime = 0f;
            isRolling = false;
        }

    }

    private void Update()
    {
        // �Ϋ��w���ʫ���]�w��V
        if (Input.GetKeyDown(KeyCode.A)) SetMoveDirection(false);
        if (Input.GetKeyDown(KeyCode.D)) SetMoveDirection(true);

        MoveTo();
        SetLookDirection();
        Jump();
        Roll();
    }

    // Debug�\��
    private void OnDrawGizmosSelected()
    {
        // �]�w gizmos �C��
        Gizmos.color = Color.red;
        if (CheckGrounded()) Gizmos.color = Color.green;

        // �M�� �}�l/���� ��m����ήg�u
        Vector3 start = transform.position + Vector3.up * groundCheckOffset;
        Vector3 end = start + Vector3.down * groundCheckDistance;

        // �e��ξɤ޽u
        Gizmos.DrawWireSphere(start, groundCheckRadius);
        Gizmos.DrawWireSphere(end, groundCheckRadius);
    }

}
