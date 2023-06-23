using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorMovement : MonoBehaviour
{
    [Header("--移動--")]
    [SerializeField]
    private float speed = 5f;           // 移動速度
    [SerializeField]
    private float turnSpeed = 0.05f;    // 旋轉速度

    [Header("--車道設定--")]
    [SerializeField]
    private float distance = 5f;        // 距離
    [SerializeField]
    private int direction = 1;          // 方向 (0 = 左, 1 = 中, 2 = 右)

    [Header("--跳躍設定--")]
    [SerializeField]
    private float gravity = 12f;        // 重力
    [SerializeField]
    private float jumpForce = 10f;      // 跳起來的推力

    [Header("--翻滾設定--")]
    [SerializeField]
    private float rollingTime = 1f;                                     // 翻轉時間
    [SerializeField]
    private float targetCapsuleHeight = 0.5f;                           // 碰撞體高度
    [SerializeField]
    private Vector3 targetCapsuleCenter = new Vector3(0, 3f, 0);        // 碰撞體位置

    [Header("--地上判定--")]
    [SerializeField]
    private float groundCheckRadius = 0.25f;                            // 圈圈
    [SerializeField]
    private float groundCheckOffset = -0.5f;                            // 碰撞體偏移量
    [SerializeField]
    private float groundCheckDistance = 0.4f;                           // 距離
    [SerializeField]
    private LayerMask groundMask;                                       // 遮罩


    // 確保角色一直接觸到地板
    private float verticalVelocity = -0.1f;                             // 垂直重力
    private CharacterController characterController;                    // 腳本控制
    private Animator animator;
    private float capsuleHeight;                                        // 碰撞體高度
    private Vector3 capsuleCenter;                                      // 碰撞體xyz
    private Vector3 groundNormal = Vector3.up;                          // 新增新的 Vector3
    private bool isRolling;                                             // 設定布林值確認是否翻滾
    private float rollElapsedTime;                                      // 翻滾計時器

    private void Start()
    {
        // 抓取元件
        characterController = GetComponent<CharacterController>();
        // 抓取碰撞體高度
        capsuleHeight = characterController.height;
        // 抓取碰撞體 xyz
        capsuleCenter = characterController.center;
        animator = GetComponent<Animator>();
    }

    private void SetMoveDirection(bool isRight)
    {
        // ? 是 （? :）的簡潔寫法，用於根據條件選擇不同的值進行運算。
        direction += isRight ? 1 : -1;              // True = 1 , False = -1
        // Clamp 界定範圍
        direction = Mathf.Clamp(direction, 0, 2);   // (0 = 左, 1 = 中, 2 = 右)
    }

    private void SetLookDirection()
    {
        // 當移動往左或往右時，旋轉角色
        Vector3 lookDirection = characterController.velocity;       // 角色速率

        if (lookDirection == Vector3.zero) return;                  // 如果為0 , return

        lookDirection.y = 0;

        // Lerp 滑順
        transform.forward = Vector3.Lerp(transform.forward, lookDirection, turnSpeed);
    }

    private void MoveTo()
    {
        // 往前移動 只需抓取Z軸 向前動就可以
        Vector3 targetPosition = transform.position.z * Vector3.forward;

        // 往左 或 往右
        if (direction == 0) targetPosition += Vector3.left * distance;
        else if(direction == 2) targetPosition += Vector3.right * distance;

        // 計算xyz移動數值
        Vector3 movement = Vector3.zero;
        movement.x = (targetPosition - transform.position).normalized.x * speed;    // 左右移動
        movement.y = verticalVelocity;                                              // 垂直速率
        movement.z = speed;

        // 移動角色
        characterController.Move(movement * Time.deltaTime);
    }

    private void Jump()
    {
        // 如果在地板
        if (CheckGrounded())
        {
            // 按下空白鍵
            if (Input.GetKeyDown(KeyCode.Space)) verticalVelocity = jumpForce;
        }
        // 設定落下時間
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
        }
        animator.SetBool("IsGround", CheckGrounded());
    }

    // 確定角色是否在地上
    private bool CheckGrounded()
    {
        // 尋找開始位置 (射線起點)
        Vector3 start = transform.position + Vector3.up * groundCheckOffset;
        // 執行圓形射線 start, radius, direction, hit info, distance, layermask (依照官方公式)
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
            // 如果正在滾動 縮小碰撞體
            characterController.height = targetCapsuleHeight;
            characterController.center = targetCapsuleCenter;
        }
        // 計時秒數
        rollElapsedTime += Time.deltaTime;

        // 重置所有值 (秒數到，回歸正常大小)
        if(rollElapsedTime>= rollingTime && isRolling)
        {
            characterController.height = targetCapsuleHeight;
            characterController.center = targetCapsuleCenter;
            rollElapsedTime = 0f;
            isRolling = false;
        }
        animator.SetBool("IsRolling", isRolling);
    }

    private void Update()
    {
        // 用指定移動按鍵設定方向
        if (Input.GetKeyDown(KeyCode.A)) SetMoveDirection(false);
        if (Input.GetKeyDown(KeyCode.D)) SetMoveDirection(true);

        MoveTo();
        SetLookDirection();
        Jump();
        Roll();
    }

    // Debug功能
    private void OnDrawGizmosSelected()
    {
        // 設定 gizmos 顏色
        Gizmos.color = Color.red;
        if (CheckGrounded()) Gizmos.color = Color.green;

        // 尋找 開始/結束 位置的圓形射線
        Vector3 start = transform.position + Vector3.up * groundCheckOffset;
        Vector3 end = start + Vector3.down * groundCheckDistance;

        // 畫圓形導引線
        Gizmos.DrawWireSphere(start, groundCheckRadius);
        Gizmos.DrawWireSphere(end, groundCheckRadius);
    }

}
