using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWPlayerView : MonoBehaviour
{
    private OWPlayerPresenter playerPresenter;
    [SerializeField] PauseMenuOverWorld pauseMenuScript;
    [SerializeField] private Animator animator;
    private const string IS_WALKING = "IsWalking";

    void Awake()
    {
        playerPresenter = GetComponent<OWPlayerPresenter>();
    }

    void OnEnable()
    {
        if (playerPresenter != null)
        {
            playerPresenter.OnPause += HandlePause;
            playerPresenter.OnMovingRotate += HandleRotation;
            playerPresenter.MoveAnimation += HandleMove;
        }
    }

    void OnDisable()
    {
        if (playerPresenter != null)
        {
            playerPresenter.OnPause -= HandlePause;
            playerPresenter.OnMovingRotate -= HandleRotation;
            playerPresenter.MoveAnimation += HandleMove;
        }
    }

    private void HandlePause()
    {
        pauseMenuScript.TogglePause();
    }

    private void HandleRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }

    private void HandleMove(bool isMoving)
    {
        animator.SetBool(IS_WALKING, isMoving);
    }
}
