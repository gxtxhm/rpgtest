using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public int damage = 10;
    public int maxHp;
    int curHp;
    Rigidbody rigid;
    Collider enemyCollider;
    NavMeshAgent pathFinder;
    PlayerController target=null;
    public Collider attackCollider;
    Animator animator;
    public LayerMask whatIsTarget;
    public Image hp;

    bool isDamaged = false;
    bool isAttack = false;
    bool isDead = false;

    bool hasTarget
    {
        get
        {
            if (target != null && !target.isDead) return true;
            
            return false;
        }
        
    }


    void Start()
    {
        maxHp = 100;
        curHp = maxHp;
        rigid = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<CapsuleCollider>();
        pathFinder = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        hp.fillAmount = curHp/100;
        StartCoroutine("UpdatePath");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (!isDamaged&&other.tag=="Hammer")
        {
            //약간 뒤로 충격 + 1초정도 무적 코루틴으로
            Debug.Log("Hammer에 맞음!");
            curHp -= other.gameObject.GetComponentInParent<Weapon>().damage;
            hp.fillAmount = curHp / 100.0f;

            if (curHp <= 0) StartCoroutine("Died");

            StartCoroutine("Damaged");
        }
    }
    IEnumerator Died()
    {
        StopCoroutine("UpdatePath");
        animator.SetTrigger("IsDied");
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
    IEnumerator Damaged()
    {
        yield return new WaitForSeconds(0.1f);
        isDamaged = true;
        yield return new WaitForSeconds(0.6f);
        isDamaged = false;

    }
    IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        isAttack = true;
        enemyCollider.enabled = true;

        yield return new WaitForSeconds(0.4f);
        enemyCollider.enabled = false;

        yield return new WaitForSeconds(0.8f);
        isAttack = false;
        

    }
    IEnumerator UpdatePath()
    {
        while(!isDead)
        {
            if(hasTarget)
            {
                pathFinder.isStopped = false;
                pathFinder.SetDestination(target.transform.position);
                animator.SetBool("IsWalk", true);
                
                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, 3f, whatIsTarget);

                for (int i = 0; i < colliders.Length; i++)
                {
                    //죽지 않았고 거리가 닿는다면 
                    PlayerController p = colliders[i].GetComponent<PlayerController>();

                    if (p != null && p==target&& !isAttack&& !p.isDead)
                    {
                        animator.SetTrigger("IsAttack");
                        StartCoroutine("Attack");
                        break;
                    }
                }
            }
            else
            {
                animator.SetBool("IsWalk", false);
                pathFinder.isStopped = true;

                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, 10f, whatIsTarget);

                

                for (int i=0;i<colliders.Length;i++)
                {
                    //죽지 않았고 거리가 닿는다면 
                    PlayerController p = colliders[i].GetComponent<PlayerController>();

                    if (p != null && !p.isDead)
                    {
                        target = p; break;
                    }

                    
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
