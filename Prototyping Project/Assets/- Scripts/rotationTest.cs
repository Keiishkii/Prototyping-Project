using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationTest : MonoBehaviour
{
    public float offset;

    public Transform containerTransfrom;
    public Transform dial;


    private void Update()
    {
        Quaternion offsetRotation = Quaternion.EulerRotation(new Vector3(offset, 0, 0));

        Quaternion rotation = containerTransfrom.rotation;

        Vector3 eurler = rotation.eulerAngles;

        Quaternion testOne = Quaternion.Euler(new Vector3(eurler.x + offset, eurler.y, eurler.z));
        Quaternion testTwo = rotation * offsetRotation;

        dial.rotation = testTwo;
    }
}
