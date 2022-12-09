using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // 초반에 뜨는 메뉴얼 판들(0:왼쪽[연습모드], 1:오른쪽[서바이벌모드], 2:중앙[환영합니다])
    public Image[] manuals = new Image[3];
    // 결과 창들(0:연습 모드 결과, 1 : 서바이벌 모드 결과)
    public Image[] results = new Image[2];


    // 메뉴 창에 뜨는 총알 공격 카운트 표시
    public Text manual_bulletCountText;
    // 플레이 결과에 뜨는 총알 공격 카운트 표시(위의 내용과 같이 연동했을 때 텍스트가 사라지는 오류로 결과 창의 내용은 따로 만듦)
    public Text result_bulletCountText;
    // 연습 모드에서는 남은 적의 수를 표시, 서바이벌 모드에서는 시간 점수를 저장할 텍스트
    public Text spawnCountOrScoreText;
    // 결과 창에 뜨는 점수 표시
    public Text resultScore;

    // 0부터 꼭짓점이 많은 순으로 적을 담을 오브젝트
    public GameObject[] enemies;
    // 스폰 장소들(0:왼쪽, 1:중간, 2:오른쪽)
    public Transform[] spawnPoints;


    // 게임 모드의 시작을 알리는 변수(해당 모드 시작 시 true로 변함)
    public static bool startPracticeMode = false;
    public static bool startSurvivalMode = false;

    // 연습 모드일 때 나오는 적을 설정하는 변수
    public static int spawnCount = 0;
    // 서바이벌 모드에서 플레이 시간을 담을 변수
    private float countTime = 0f;
    // 서바이벌 모드에서 체력이 0이 될 때 게임 오버를 결정하는 변수
    public static bool gameOver = false;


    // 스폰 장소 중 랜덤하게 하나 정할 변수
    private int randomSpawnSpot = 0;
    //소환 되는 적의 타입을 랜덤하게 지정할 변수
    private int randomSelectEnemy = 0;
    // 시간이 지날 때 마다 스폰되는 랜덤한 시간을 설정하는 변수
    private int randomTime = 0;
    // 연습 모드에 처음 설정된 적 수 저장(이후에 최종 점수에 사용)
    private int firstSetspawnCount = 0;
    // 현재 적이 스폰 중인지(아니면 false)
    private bool isSpawning = false;
    
    // 게임 시작 여부를 설정하는 변수
    public static bool startGame = false;
    // 게임 시작 전 초기 설정을 결정하는 변수
    public static bool beforeStart = false;
    // 판에 뜨는 총알 공격 카운트
    public static int manual_result_bulletCount;


    void Start()
    {
        // 초반에 표시될 판 활성화
        for (int i = 0; i < manuals.Length; i++)
        {
            manuals[i].enabled = true;
        }
        // 이후에 나오는 결과 창 비활성화(이미지 곂치는 오류 방지)
        for (int i = 0; i < results.Length; i++)
        {
            results[i].enabled = false;
        }
        spawnCountOrScoreText.enabled = false;
        resultScore.enabled = false;
        result_bulletCountText.enabled = false;

        // 총알 공격 카운트 랜덤하게 설정 후 띄우기
        manual_bulletCountText.enabled = true;
        // 맨 처음에 뜨는 수가 3이라서 설명을 보기 전에 시작되는 것을 막기 위해 4부터 6으로 설정
        manual_result_bulletCount = Random.Range(4, 6);
        manual_bulletCountText.text = manual_result_bulletCount.ToString();
    }

    void Update()
    {
        // 모드가 결정된 게임 시작일 때(한 번만 설정)
        if (startGame)
        {
            // 게임 시작 직전이면
            if (beforeStart)
            {
                // 창 비활성화 밒 모드에 맞게 게임 설정
                StartCoroutine(StartGameSet());
            }
            
            // 연습 모드면
            if (startPracticeMode)
            {
                // 남은 적 수 표시
                spawnCountOrScoreText.text = "남은 적 : " + spawnCount;
                // 시간 계산
                countTime += Time.deltaTime;
                // 남은 적이 0 이하일 때
                if (spawnCount <= 0)
                {
                    // 게임 끝
                    GameOver(EnemyController.destroyEnemyCount);
                }
            }
            // 서바이벌 모드면
            else if (startSurvivalMode)
            {
                // 시간 계산
                countTime += Time.deltaTime;
                // 점수 표시
                spawnCountOrScoreText.text = "점수[시간] : " + (int)countTime;

                if (gameOver)
                {
                    // 게임 끝
                    GameOver((int)countTime);
                }
            }

            // 적을 스폰 중이 아닐 때 + 적이 필드 위에 있지 않을 때
            if (!isSpawning && EnemyController.thereIsNoEnemy)
            {
                // 적을 소환
                StartCoroutine(SpawnEnemy());
            }
        } 
    }


    private void GameOver(int result)
    {
        // 게임 플레이 중지
        startGame = false;
        StopAllCoroutines();

        // 적 속도 초기화
        EnemyController.moveSpeed = 20f;

        // 점수 판 비활성화
        spawnCountOrScoreText.enabled = false;

        // 연습 모드 판과 서바이벌 모드 판 띄우기
        manuals[0].enabled = true;
        manuals[1].enabled = true;
        // 가운데에 이미지 활성화(결과 및 다시 시작 하려면?)
        // 모드에 맞게 판 띄우기, 점수 텍스트 설정
        if (startPracticeMode)
        {
            results[0].enabled = true;
            resultScore.text = result.ToString() + "/" + firstSetspawnCount;
        }
        else if (startSurvivalMode)
        {
            results[1].enabled = true;
            resultScore.text = result.ToString();
        }

        // 모드에 맞게 모든 설정이 끝났으니 전부 비활성화(예외 방지를 위해 다 비활성화)
        startPracticeMode = false;
        startSurvivalMode = false;
        beforeStart = false;
        // 점수 표시
        resultScore.enabled = true;
        

        // 총알 공격 카운트 랜덤하게 설정 후 띄우기
        do
        {
            // 마지막에 총에 설정된 수와 다르게 하기(도중에 실수로 발사해서 바로 시작될 수 있어서)
            manual_result_bulletCount = Random.Range(3, 6);
        } while (manual_result_bulletCount == PlayerController.bulletCount);
        result_bulletCountText.enabled = true;
        result_bulletCountText.text = manual_result_bulletCount.ToString();

        // 시간 경과 초기화
        countTime = 0;

        // 계속해서 적이 소환되는 것을 방지
        isSpawning = true;
    }



    // 시간과 위치, 적 타입 설정 후 적 소환
    IEnumerator SpawnEnemy()
    {
        // 시간 경과에 따라서 스폰 되는 텀과 적의 속도 설정(시간이 지날수록 어려워짐)
        if (countTime <= 10.0f)
        {
            randomTime = Random.Range(4, 5);
        }      
        else if (countTime > 10.0f && countTime <= 20)
        {
            randomTime = Random.Range(3, 4);
            EnemyController.moveSpeed = 20;
        }     
        else if (countTime > 20.0f && countTime <= 30)
        {
            randomTime = Random.Range(2, 3);
            EnemyController.moveSpeed = 25;
        }    
        else if (countTime > 30.0f && countTime <= 40)
        {
            randomTime = Random.Range(1, 2);
            EnemyController.moveSpeed = 30;
        }  
        else if (countTime > 40.0f && countTime <= 50)
        {
            randomTime = 0;
            EnemyController.moveSpeed = 35;
        }
        else if(countTime > 50.0f && countTime <= 60)
        {
            EnemyController.moveSpeed = 40;
        }
        else if(countTime > 60)
        {
            EnemyController.moveSpeed = 50;
        }

        // 적을 스폰할 위치와 적 선택 후 적을 소환
        randomSpawnSpot = Random.Range(0, spawnPoints.Length);
        randomSelectEnemy = Random.Range(0, enemies.Length);

        isSpawning = true;
        // 랜덤으로 정한 시간 경과 후 적을 소환
        yield return new WaitForSeconds(randomTime);
        Instantiate(enemies[randomSelectEnemy], spawnPoints[randomSpawnSpot]);
        
        isSpawning = false;
    }


    // 초반에 플레이어가 결정한 모드에 맞게 설정
    IEnumerator StartGameSet()
    {
        // 메뉴얼 판 비활성화(이후에 나오는 결과 창까지 비활성화)
        for (int i = 0; i < manuals.Length; i++)
        {
            manuals[i].enabled = false;
        }
        for (int i = 0; i < results.Length; i++)
        {
            results[i].enabled = false;
        }
        manual_bulletCountText.enabled = false;
        resultScore.enabled = false;
        result_bulletCountText.enabled = false;

        // 점수 판 활성화
        spawnCountOrScoreText.enabled = true;

        // 연습 모드일 때
        if (startPracticeMode)
        {
            // 파괴한 적 수 초기화
            EnemyController.destroyEnemyCount = 0;
            // 스폰할 적 수 설정
            spawnCount = Random.Range(20, 30);
            // 연습 모드에 처음 설정된 적 수 저장(이후에 최종 점수에 사용)
            firstSetspawnCount = spawnCount;
            // 적 수 표시
            spawnCountOrScoreText.text = "남은 적 : " + spawnCount;
        }
        // 서바이벌 모드일 때
        else if (startSurvivalMode)
        { 
            // 플레이어 초기 체력 설정
            PlayerController.playerHealth = 5;
            PlayerController.survivalHealth = true;
            // 점수 표시
            spawnCountOrScoreText.text = "점수[시간] : " + (int)countTime;
        }

        // 시간 초기화(확인을 위해 다시 초기화 함)
        countTime = 0;
        // 플레이 중에는 설정하면 안되므로 false 처리
        beforeStart = false;
        // 이전 플레이 도중에 스폰 오류가 있으므르 false 처리
        isSpawning = false;

        yield return null;
    }
}
