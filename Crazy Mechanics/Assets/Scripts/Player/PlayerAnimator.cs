using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {


    private const string IS_WALKING = "IsWalking";
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
        
    }

}