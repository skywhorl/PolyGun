using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 파괴 효과를 생성할 particleSystem
    public new GameObject particleSystem;

    // 오브젝트에서 사용할 컴포넌트들
    private BoxCollider myBoxCollider = null;
    private MeshRenderer myMeshRenderer = null;


    // 회전 속도와 전진 속도
    private float rotationSpeed = 0.5f;
    public static float moveSpeed = 15f;
    // 적을 파괴할 지 여부
    private bool destroyEnemy = false;
    // 연습 모드일 때 파괴한 적의 수를 계산하는 변수
    public static int destroyEnemyCount = 0;

    // 오브젝트의 꼭짓점 수(구는 별다른 설정이 없기에 저절로 0)
    public int enemyVertex_start = 0;
    // 다른 스크립트에서 참고하는 오브젝트의 꼭짓점 수
    public static int enemyVertex = 0;

    // 적을 한번에 여러마리 소환하면 충돌이 안 먹어서 한 텀에 한 마리만 소환하도록 설정
    public static bool thereIsNoEnemy = true;

    // Update 문에서 파티클 무한 생성을 막기 위한 변수(해당 변수가 없으면 Update에서 파괴를 계속해서 호출하므로 없던 파티클이 계속해서 생성됨)
    private bool destroyOnce = true;


    // 적 파괴 시 재생되는 효과음
    private AudioSource explodeAudio;
    // 효과음 소스
    public AudioClip explodeSound;


    void Start()
    {
        // 적이 필드에 있음을 알림
        thereIsNoEnemy = false;

        // Update 문에서 파티클 무한 생성 막기
        destroyOnce = true;

        // 미리 설정해 둔 꼭짓점 수를 다른 스크립트가 참고할 수 있게 저장
        enemyVertex = enemyVertex_start;

        // 오브젝트에 필요한 컴포넌트 저장(충돌 영역과 랜더링, 구도 BoxCollider로 설정함)
        myBoxCollider = GetComponent<BoxCollider>();
        myMeshRenderer = GetComponent<MeshRenderer>();

        // 오디오 컴포넌트 설정
        explodeAudio = GetComponent<AudioSource>();
    }


    void Update()
    {
        // 뒤로(플레이어의 기준에서는 앞에서 뒤로 옴) 가면서 회전
        transform.position -= transform.forward * Time.deltaTime * moveSpeed;
        transform.Rotate(0, 0, rotationSpeed);
        
        // 적이 움직이다가 플레이어가 게임 오버 시 + 한 번만 파괴 
        if (GameController.gameOver&& destroyOnce)
        {
            // 적 삭제
            Explode();
            destroyOnce = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 다각형이나 구 중에서 플레이어와 충돌 시
        if (other.tag == "Player")
        {
            // 서바이벌 모드일 때
            if (GameController.startSurvivalMode)
            {
                // 체력 설정
                PlayerController.getPlayerDamaged = true;
            }

            // 적 파괴 허용
            destroyEnemy = true;
        }
        // 1. 다각형이 총알에 충돌 시 + 총알에 설정한 수와 동일할 때
        // 2. 구가 방패에 충돌 시 + 방패가 작동할 때
        else if (other.tag == "Bullet" && gameObject.tag == "Polygon" && PlayerController.bulletCount == enemyVertex
            || other.tag == "Shield" && gameObject.tag == "Sphere" && PlayerController.shieldActivate)
        {
            // 연습 모드일 때
            if (GameController.startPracticeMode)
            {
                destroyEnemyCount++;
            }
            // 적 파괴 허용
            destroyEnemy = true;
        }
            
        // 적 파괴가 허용될 때(따로 구분하는 이유 : 벽 충돌이 있을 수도 있어서)
        if (destroyEnemy)
        {
            // 연습 모드일 때
            if (GameController.startPracticeMode)
            {
                // 적의 수 1 줄이기
                GameController.spawnCount--;
            }
            // 오브젝트 파괴와 더불어 파괴 효과 생성
            Explode();
        }
            
    }


    // ParticleSystem을 통한 파괴 효과 재생, 후에 해당 오브젝트를 파괴
    private void Explode()
    {
        // 파괴 효과 생성(앞에 GameObject로 캐스팅 안 할 시 메모리 보호 문제로 이후에 삭제를 안 함)
        particleSystem = (GameObject)Instantiate(particleSystem, transform.position, transform.rotation);
        // 파괴 효과 재생
        particleSystem.GetComponent<ParticleSystem>().Play();
        // 효과음 재생
        explodeAudio.PlayOneShot(explodeSound, 4.0f);
        // 파괴 효과가 재생 중일 때 오브젝트 자체가 파괴되면 안되므로 충돌과 랜더링 삭제
        Destroy(myMeshRenderer);
        Destroy(myBoxCollider);
        // 파괴 효과와 오브젝트 삭제
        StartCoroutine(DestroyParticleSystem(particleSystem));
    }


    // 파괴 효과 재생 후 파괴 효과와 오브젝트 삭제 
    IEnumerator DestroyParticleSystem(GameObject particleSystem)
    {
        // 1초 대기(효과 재생 시간)
        yield return new WaitForSeconds(1f);
        // 필드에 적이 없다고 알리기
        thereIsNoEnemy = true;
        // ParticleSystem과 오브젝트(적) 파괴
        Destroy(particleSystem);
        Destroy(gameObject);
    }
}
