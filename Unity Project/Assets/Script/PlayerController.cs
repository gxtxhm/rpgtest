﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int maxHp;
    int curHp;

    bool visibleCursor = false;

    public Camera cam;
    public GameObject player;
    Rigidbody playerRigid;
    Animator animator;
    public Image hp;
    public Image mp;
    public bool isDead = false;
    public float speed = 3f;

    bool isDamaged = false;
    private Vector3 MoveDir;
    float horizontal, vertical;
    Vector3 moveVec;

    Weapon weapon;
    public float swingDelay = 0.5f;
    float lastSwing;
    void Start()
    {
        maxHp = 100;
        curHp = maxHp;
        playerRigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        // 나중에 바꿔야함
        weapon = GetComponentInChildren<Weapon>();
        hp.fillAmount = curHp/100;
        mp.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(!visibleCursor) PlayerMove();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (visibleCursor) { visibleCursor = false; cam.GetComponent<CameraMove>().canMove = true; }
            else { visibleCursor = true; cam.GetComponent<CameraMove>().canMove = false; }

            Cursor.visible = visibleCursor;

        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyCollider"&&!isDamaged)
        {
            //약간 뒤로 충격 + 1초정도 무적 코루틴으로
            Debug.Log("적에게 맞음!");
            curHp -= other.gameObject.GetComponentInParent<Enemy>().damage;
            hp.fillAmount = curHp / 100.0f;

            if (curHp <= 0) Died();

            StartCoroutine("Damaged");
        }
    }
    IEnumerator Damaged()
    {
        yield return new WaitForSeconds(0.1f);
        isDamaged = true;
        yield return new WaitForSeconds(0.6f);
        isDamaged = false;

    }
    public void Died()
    {
        Debug.Log("플레이어 사망");
    }

    void MoveLookAt()
    {
        //메인카메라가 바라보는 방향입니다.
        Vector3 dir = cam.transform.localRotation * Vector3.forward;
        //카메라가 바라보는 방향으로 팩맨도 바라보게 합니다.
        transform.localRotation = cam.transform.localRotation;
        //팩맨의 Rotation.x값을 freeze해놓았지만 움직여서 따로 Rotation값을 0으로 세팅해주었습니다.
        transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);

        player.transform.localRotation = transform.localRotation;
    }

    void PlayerMove()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        moveVec = new Vector3(horizontal, 0, vertical).normalized;
        
        MoveLookAt();

        // 카메라 이동방향
        Quaternion v3Rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        moveVec = v3Rotation * moveVec;

        if ((horizontal!=0||vertical!=0)&&Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", true);
            transform.position += moveVec *2* speed * Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsRun", false);
            
        }

        
        if(horizontal!=0||vertical!=0)
        {
            animator.SetBool("IsWalk", true);
            transform.position += moveVec    * speed * Time.deltaTime;
        }
        else
        {

            animator.SetBool("IsWalk", false);
        }

        if(Input.GetMouseButtonDown(0))
        {
            // 
            if(swingDelay+lastSwing<Time.time)
            {
                Debug.Log("공격!");
                animator.SetTrigger("IsAttack");
                weapon.UseWeapon();
                lastSwing = Time.time;
            }
        }
    }

}
