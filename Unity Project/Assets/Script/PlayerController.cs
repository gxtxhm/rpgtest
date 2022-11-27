using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int maxHp;
    int curHp;

    Rigidbody playerRigid;
    Animator animator;
    public Image hp;
    public Image mp;
    public bool isDead = false;
    public float speed = 3f;

    bool isDamaged = false;

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
        PlayerMove();
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
    void PlayerMove()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        moveVec = new Vector3(horizontal, 0, vertical).normalized;
        transform.LookAt(transform.position + moveVec);
        if((horizontal!=0||vertical!=0)&&Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("IsRun", true);
            transform.position += moveVec *2* speed * Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsRun", false);
            transform.position += moveVec * speed * Time.deltaTime;
        }

        
        if(horizontal!=0||vertical!=0)
        {
            animator.SetBool("IsWalk", true);
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
