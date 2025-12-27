using System.Collections.Generic;
using UnityEngine;

namespace Supercyan.AnimalPeopleSample
{
    public class SimpleSampleCharacterControl : MonoBehaviour
    {
        [SerializeField] private float speed = 2f; // Walking speed
        [SerializeField] private float jumpSpeed = 5f; // Jump speed
        [SerializeField] private float gravity = 9.8f; // Gravity
        [SerializeField] private float rotationSpeed = 5f; // Speed of rotation

        [SerializeField] private Animator m_animator = null;
        private CharacterController controller;
        private Vector3 moveDirection = Vector3.zero;

        private void Awake()
        {
            if (!m_animator) { m_animator = GetComponent<Animator>(); }
            controller = GetComponent<CharacterController>();
            
            // Add CharacterController if it doesn't exist
            if (controller == null)
            {
                controller = gameObject.AddComponent<CharacterController>();
                controller.radius = 0.3f;
                controller.height = 1.8f;
                controller.center = new Vector3(0, 0.9f, 0);
            }
        }

        private void Update()
        {
            // Get input
            bool isMoving = false;
            Vector3 targetDirection = Vector3.zero;

            // WASD movement
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                targetDirection = Vector3.forward; // Move forward
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                targetDirection = Vector3.back; // Move backward
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                targetDirection = Vector3.right; // Move right
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                targetDirection = Vector3.left; // Move left
                isMoving = true;
            }

            if (isMoving)
            {
                // Get target rotation
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                
                // Smoothly rotate towards target
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
                // Set animator to walk
                if (m_animator) m_animator.SetFloat("MoveSpeed", 1f);
            }
            else
            {
                // Set animator to idle
                if (m_animator) m_animator.SetFloat("MoveSpeed", 0f);
            }

            // Handle vertical movement
            if (controller.isGrounded)
            {
                moveDirection.y = -0.5f; // Small downward force to keep grounded
                
                // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    moveDirection.y = jumpSpeed;
                }
            }
            else
            {
                // Apply gravity when in air
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Horizontal movement
            Vector3 move = transform.forward * (isMoving ? speed : 0) * Time.deltaTime;
            move.y = moveDirection.y * Time.deltaTime;
            
            controller.Move(move);

            // Update grounded state for animator
            if (m_animator) m_animator.SetBool("Grounded", controller.isGrounded);
        }
    }
}