using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private bool canClimb = false; // ��ٸ� ���� �ȿ� �ִ��� ����
    private bool isClimbing = false; // ��ٸ� ������ ����
    private Rigidbody playerRb;
    private float climbSpeed = 5.0f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = true; // ��ٸ� ���� �ȿ� ����
            playerRb = other.GetComponent<Rigidbody>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = false; // ��ٸ� ������ ����� climbing �Ұ���
            isClimbing = false;
            playerRb.useGravity = true;
            playerRb.velocity = Vector3.zero; // ��ٸ��� ��� �� �ӵ� �ʱ�ȭ
        }
    }

    void Update()
    {
        if (canClimb) // ��ٸ� ���� �ȿ� ���� ���� climbing ����
        {
            float verticalInput = Input.GetAxis("Vertical");

            if (verticalInput != 0) // ��/�Ʒ� Ű�� ������ climbing ���� Ȱ��ȭ
            {
                isClimbing = true;
                playerRb.useGravity = false;
                playerRb.velocity = new Vector3(0, verticalInput * climbSpeed, 0);
            }

            if (isClimbing && verticalInput == 0) // Ű�� ���� ����
            {
                playerRb.velocity = Vector3.zero;
            }

            if (isClimbing && Input.GetButtonDown("Jump")) // �����ϸ� ��ٸ����� ������
            {
                isClimbing = false;
                canClimb = false; // ��ٸ����� ������� ����
                playerRb.useGravity = true;
                playerRb.AddForce(Vector3.up * 3.0f, ForceMode.Impulse); // ��¦ ���� ����
            }
        }
    }
}
