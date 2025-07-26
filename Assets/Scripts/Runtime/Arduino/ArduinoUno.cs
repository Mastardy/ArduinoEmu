using System;
using UnityEngine;

public class ArduinoUno : MonoBehaviour
{
    private ATmega328P MicroController = new();

    private void Awake()
    {
        MicroController.Debug(MicroController.GPWR[0x1F]);
        MicroController.GPWR[0x1F] = 0x7F;
        MicroController.GPWR[0x1E] = 0x01;
        MicroController.ADD(0x1F, 0x1E);
        MicroController.Debug(MicroController.GPWR[0x1F]);
        MicroController.Debug();
    }
}