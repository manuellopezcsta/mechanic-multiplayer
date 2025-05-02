using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {


    private const string IS_WALKING = "IsWalking";
    private const string IS_DANCING = "Dancing";
    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem stunnedParticles;


    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
        player.OnStun += Animator_OnStun;
    }

    // Por alguna razon las particulas se ven al iniciar ? Aunque no tengan play on Awake.
    private void Animator_OnStun(object sender, EventArgs e)
    {
        Debug.Log("Se ejecuto el stun");
        stunnedParticles.Play();
    }

    private void Update() {
        
        animator.SetBool(IS_WALKING, player.IsWalking());
        /*if (player.IsWalking())
        {
            animator.SetBool(IS_DANCING, false);
        }*/
    }

    public void StartDance()
    {
        //El baile se va a realizar solo cuando el personaje este quieto
        if (!player.IsWalking() && animator != null)
        {
            animator.SetTrigger(IS_DANCING);
        }
    }

}