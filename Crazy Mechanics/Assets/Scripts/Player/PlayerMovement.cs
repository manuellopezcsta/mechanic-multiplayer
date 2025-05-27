using System.Collections;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 inputVector = Vector2.zero;
    private CharacterController characterController;
    Player player;

    //Dash
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashCooldown = 3f;

    // Para el stun
    private bool stunned = false;
    [SerializeField] float stunDuration = 2f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
    }

    public void SetInputVector(Vector2 direction)
    {
        inputVector = direction;
    }

    public void HandleMovement()
    {
        if (stunned || player.isSliding)
        {
            player.isWalking = false;
            return;
        }

        // Capturamos al vector desde GameImput y se lo aplicamos al char controller.
        //Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        player.moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (Player.invertControls)
        {
            player.moveDir *= -1;
        }

        // Cacheamos la direccion x si se resbala.
        if (!player.isSliding)
        {
            player.slideDir = player.moveDir;
        }
        //SimpleMove no ignora la gravedad.
        characterController.Move(player.moveDir * Time.deltaTime * player.speed);
        // Para arreglar un bug donde flota el player?
        characterController.Move(Vector3.down * Time.deltaTime * player.speed);
        //transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        // Rotamos al personaje.
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, player.moveDir, Time.deltaTime * rotateSpeed);

        // Actualizamos el booleano para la animacion.
        player.isWalking = player.moveDir != Vector3.zero;
    }

    public void Dash()
    {
        if (canDash)
        {
            canDash = false;
            StartCoroutine(DashCoroutine(player.moveDir, dashSpeed, dashDuration));
        }
    }

    private IEnumerator DashCoroutine(Vector3 moveDir, float dashSpeed, float dashTime)
    {
        float dashTimer = 0f;

        while (dashTimer < dashTime)
        {
            characterController.Move(moveDir * Time.deltaTime * dashSpeed);
            dashTimer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(DashCooldownCoroutine(dashCooldown));
    }

    private IEnumerator DashCooldownCoroutine(float cooldownDash)
    {
        yield return new WaitForSeconds(cooldownDash);
        canDash = true;
    }

    // Stun

    public IEnumerator GetStunned()
    {
        //Debug.Log("Stuneo al player " + this.gameObject.name);
        player.TriggerStunEvent();
        stunned = true;
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
    }

    public void RespawnAtRandomPos()
    {
        int i = UnityEngine.Random.Range(0, GameManager.Instance.playerSpawns.Length);
        transform.position = GameManager.Instance.playerSpawns[i].position;
    }
}
