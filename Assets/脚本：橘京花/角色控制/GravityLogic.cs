using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLogic : MonoBehaviour
{
    public float gravity = -9.81f; // 重力加速度

    private CharacterController characterController;
    private bool isGrounded;         // 是否在地面
    private Vector3 velocity;        // 角色的速度

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        velocity = Vector3.zero; // 初始化速度
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 微小负值确保角色稳定贴合地面
        }

        // 应用重力
        velocity.y += gravity * Time.deltaTime;

        // 移动角色
        characterController.Move(velocity * Time.deltaTime);
    }
}