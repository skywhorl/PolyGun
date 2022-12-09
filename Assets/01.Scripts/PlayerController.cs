using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    // 입력 소스 정의(왼손, 오른손, 양손)
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources anyHand = SteamVR_Input_Sources.Any;

    // Trigger 버튼을 눌렀을 때 숫자 조정[왼손] 및 총알 발사[오른손]하도록 하는변수
    public SteamVR_Action_Boolean shootTrigger = SteamVR_Actions.default_InteractUI;
    // 액션 - 그립 버튼의 잡기(GrabGrip)
    public SteamVR_Action_Boolean grip = SteamVR_Input.GetBooleanAction("GrabGrip");
    // 액션 - 햅틱(Haptic)
    public SteamVR_Action_Vibration haptic = SteamVR_Actions.default_Haptic;


    // 플레이어의 체력
    public static int playerHealth = 5;
    // 플레이어가 피해를 입었는 지의 여부
    public static bool getPlayerDamaged = false;
    // 총알 공격 카운트(삼각기둥이면 3, 사각기둥이면 4), 사용할 수는 3, 4, 5, 6 이어서 가장 낮은 3으로 초기화
    public static int bulletCount = 3;
    // 방패 작동 여부(Grip 버튼을 누른 상태만 방패가 활성화 됨)
    public static bool shieldActivate = false;

    // 플레이어의 체력을 눈으로 보여주는 이미지
    public Image[] playerHealthToImage = new Image[5];
    // 총에 있는 총알 공격 카운트를 텍스트로 표현하는 부분
    public Text bulletCountToText;
    // 처음에 체력 창을 정상적으로 활성화 하기 위한 변수
    public static bool survivalHealth = false;

    // 총알 발사 위치
    public Transform fireSpot;
    // 총알
    public GameObject bullet;
    public GameObject Shield;
    // 방패 Material(0:비활성화된 방패, 1:활성화된 방패)
    public Material[] materials;

    // 총알 발사 시 재생되는 효과음
    private AudioSource shootAudio;
    // 효과음 소스
    public AudioClip shootSound;


    void Start()
    {
        // 체력 창 비활성화
        for (int i = 0; i < playerHealthToImage.Length; i++)
        {
            playerHealthToImage[i].enabled = false;
        }
        // 총에 총알 카운트 띄우는 부분의 텍스트 수정
        bulletCountToText.text = bulletCount.ToString();
        // 오디오 컴포넌트 설정
        shootAudio = GetComponent<AudioSource>();
    }


    void Update()
    {
        // Vive 장비의 초기화 여부 확인
        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
        {
            return;
        }

        // 오른쪽 컨트롤러의 트리거 버튼을 한 번 클릭했을 경우
        if (shootTrigger.GetStateDown(rightHand))
        {
            // 총알 발사
            Instantiate(bullet, fireSpot.position, fireSpot.rotation);
            // 효과음 재생
            shootAudio.PlayOneShot(shootSound, 1.0f);
            // 오른손 컨트롤러에 진동을 발생시킴
            haptic.Execute(0.2f, 0.2f, 100.0f, 0.5f, rightHand);
        }

        // 왼쪽 컨트롤러의 트리거 버튼을 클릭했을 경우
        if (shootTrigger.GetStateDown(leftHand))
        {
            bulletCount++;
            if (bulletCount > 6)
            {
                bulletCount = 3;
            }
            // 총에 총알 카운트 띄우는 부분의 텍스트 수정
            bulletCountToText.text = bulletCount.ToString();
        }

        // 왼손 컨트롤러의 그립을 잡았을 경우
        if (grip.GetStateDown(leftHand))
        {
            // 방패 활성화
            shieldActivate = true;
            // Material 변경(활성화 되었음을 표시)
            Shield.GetComponent<MeshRenderer>().material = materials[1];
            // 왼손 컨트롤러에 진동을 발생시킴
            haptic.Execute(0.2f, 0.2f, 50.0f, 0.5f, leftHand);
        }
        // 왼손 컨트롤러의 그립을 뗐을 경우
        else if(grip.GetStateUp(leftHand))
        {
            // Material 변경(원래 색상이자 비활성화 되었음을 표시)
            Shield.GetComponent<MeshRenderer>().material = materials[0];
            // 방패 비활성화
            shieldActivate = false;
        }


        // 서바이벌 모드가 시작되었을 때 + 한 번만 설정하는 거라면
        if (survivalHealth)
        {
            // 체력 창 활성화
            for (int i = 0; i < playerHealthToImage.Length; i++)
            {
                playerHealthToImage[i].enabled = true;
            }
            survivalHealth = false;
        }
        

        // 플레이어가 피해를 입었을 시
        if (getPlayerDamaged)
        {
            // 체력 1 감소
            playerHealth--;

            // 양 손에 진동이 오게 하기
            haptic.Execute(0.2f, 0.2f, 100.0f, 0.5f, anyHand);

            // 체력 창 재설정
            for (int i = playerHealth; i < playerHealthToImage.Length; i++)
            {
                playerHealthToImage[i].enabled = false;
            }
            if (playerHealth <= 0)
            {
                // 게임 오버 설정
                GameController.gameOver = true;
            }
            getPlayerDamaged = false;
        }
    }
}
