using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorTool : BaseCounter
{
    public bool hasMotor = false;

    [SerializeField] GameObject[] motorPieces;

    void Start()
    {
        HideMotor();   
    }

    void ShowMotor() {
        if (hasMotor) {
            foreach (GameObject piece in motorPieces) {
                piece.SetActive(true);
            }
        }
    }

    void HideMotor() {
        if (!hasMotor) {
            foreach (GameObject piece in motorPieces) {
                piece.SetActive(false);
            }
        }
    }
}
