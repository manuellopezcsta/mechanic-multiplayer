using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class OilSplatter : BaseCounter, IHasProgress
{
    [SerializeField] private ObjectsSO cleaningTool;
    private int cleaningProgress;
    [SerializeField] private int cleaningProgressMax;

    // Para el slide
    private Vector3 slideDirection;
    float slideDuration = 1f;
    [SerializeField] float slideSpeed = 5f;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public static event EventHandler OnOilSpillCleaning;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.isSliding = true;
            CharacterController controller = player.GetComponent<CharacterController>();
            //slideDirection = Tools.GetRandomDirection();
            slideDirection = player.slideDir;
            StartCoroutine(SlideAndStunRoutine(player, controller));
        }
    }

    private IEnumerator SlideAndStunRoutine(Player player, CharacterController controller)
    {
        
        // Deslizar en la dirección actual del movimiento
        float elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.isSliding = false;
        StartCoroutine(player.GetStunned());
    }


    public override void Interact(Player player)
    {
        // Si el player esta sosteniendo la cleaning tool correcta.
        if (player.HasCarObject() && player.GetCarObject().GetObjectSO() == cleaningTool) {
            cleaningProgress++;
            // Sonidito de limpieza.
            OnOilSpillCleaning?.Invoke(this, EventArgs.Empty);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
            progressNormalized = (float) cleaningProgress / cleaningProgressMax
        });
        }

        // Si lo terminamos de limpiar.
        if(cleaningProgress == cleaningProgressMax) {
            Destroy(gameObject);
        }
    }


}
