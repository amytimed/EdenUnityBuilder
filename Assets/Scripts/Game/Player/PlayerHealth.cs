using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public byte Health = 4;

    //private Player _player;

    public Animator DamageEffect;

    public static PlayerHealth Instance;

    void Start()
    {
        // _player = gameObject.GetComponent<Player>();
        Instance = this;
        Health = 4;
    }

    public void ApplyDamage()
    {
        Health -= 1;
        DamageEffect.SetTrigger("Damaged");
    }

    public void PlayDamageEffect()
    {
        DamageEffect.SetTrigger("Damaged");
    }

    public void SetHealth(byte heal)
    {
        Health = heal;
    }

    public void Lose()
    {
        SetHealth(4);

        DamageEffect.SetTrigger("isDead");
        DamageEffect.ResetTrigger("Damaged");
        // _player.transform.position = _player.HomePosition;
    }


    void Update()
    {
        if (Health <= 0)
        {
            Lose();
        }
    }
}