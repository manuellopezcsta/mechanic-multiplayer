using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldCharacterAnimator : MonoBehaviour
{   
    [SerializeField] private PlayerController player;
    private const string IS_WALKING = "IsWalking";

    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
      animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
