using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Collider weaponCollider;
    public int damage=20;
    void Start()
    {
        // 나중에 무기 꺼낼때나 먹을 때 드는 거로 바꿔야함
        //weapon = GetComponentInChildren<GameObject>();
        weaponCollider = GetComponentInChildren<Collider>();
    }

    // Update is called once per frame
    public void UseWeapon()
    {
        // 해머면
        if (gameObject.CompareTag("Hammer"))
        {
            StopCoroutine("Weird");
            StartCoroutine("Weird");
        }
    }

    IEnumerator Weird()
    {
        yield return new WaitForSeconds(0.1f);
        weaponCollider.enabled = true;

        yield return new WaitForSeconds(0.4f);
        weaponCollider.enabled = false;
    }
}
