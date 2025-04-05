using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisualInvisible : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private List<GameObject> visualGameObjectArray;
    [SerializeField] private GameObject counterTopPoint;
    [SerializeField] private Material materialSelecto;
    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelecterCounterChanged;   
    }

    private void Player_OnSelecterCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if(e.selectedCounter == baseCounter) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            if(visualGameObject == null) {
                continue;
            }
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            if(visualGameObject == null) {
                continue;
            }
            visualGameObject.SetActive(false);
        }
    }

    public void SetUpSelected() {
        // Preparamos una visual de selected, creando una copia y ajustando sus materiales.
        
        // Busca el primer hijo de "CounterTopPoint"
        if (counterTopPoint.transform.childCount > 0)
        {
            GameObject childToCopy = counterTopPoint.transform.GetChild(0).gameObject;

            // Crea una copia del hijo
            GameObject copiedChild = Instantiate(childToCopy);

            copiedChild.name = "Clon Visual";
            // Establece el padre de la copia como "Selected"
            copiedChild.transform.SetParent(transform);
            //copiedChild.transform.rotation = childToCopy.transform.rotation;

            // Ajusta la posición local de la copia al valor que desees
            copiedChild.transform.localPosition = Vector3.zero; // Por ejemplo, posición centrada
            copiedChild.transform.localRotation = Quaternion.identity; // Resetea la rotación

            visualGameObjectArray.Clear();
            // Para todas los items en child select, cambiarles el material x el selecto
            FixMaterial(copiedChild.transform);
            // Apagamos el select.
            visualGameObjectArray.Add(copiedChild.gameObject);
            copiedChild.gameObject.SetActive(false);
        }
    }

    private void FixMaterial(Transform target) {
        
        Transform holderVisual = target.GetChild(0);
        foreach (Transform child in holderVisual) {
            if (child.TryGetComponent(out MeshRenderer meshRenderer)) {
                meshRenderer.material = materialSelecto;
            }
        }
    }
}