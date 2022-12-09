using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour
{
    private float moveSpeed = 100f;


    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 아직 게임 시작이 아닐 때 + 각 모드에 총을 쐈을 경우 + 총알 공격 카운트가 맞을 때, 해당 모드의 시작을 알림
        if (!GameController.startGame && other.gameObject.tag == "StartPracticeMode" 
            && GameController.manual_result_bulletCount == PlayerController.bulletCount)
        {
            GameController.startPracticeMode = true;
            GameController.startGame = true;
            GameController.beforeStart = true;
            GameController.gameOver = false;
        }
        else if (!GameController.startGame && other.tag == "StartSurvivalMode" 
            && GameController.manual_result_bulletCount == PlayerController.bulletCount)
        {
            GameController.startSurvivalMode = true;
            GameController.startGame = true;
            GameController.beforeStart = true;
            GameController.gameOver = false;
        }

        // 구한테 충돌했을 때
        if(other.tag == "Sphere")
        {
            // 총알 삭제
            Destroy(gameObject);
        }

        // 벽(안 보이는 영역)이랑 충돌했을 때
        if(other.tag == "Wall")
        {
            // 총알 삭제
            Destroy(gameObject);
        }
    }
}
