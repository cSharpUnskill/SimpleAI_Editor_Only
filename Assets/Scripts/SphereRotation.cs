using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRotation : MonoBehaviour
{
    [SerializeField]
    private GameObject _sphere;

    void Update()
    {
        if (_sphere.activeSelf)
        {
            _sphere.transform.Rotate(Vector3.up, 300f * Time.deltaTime);
        }
    }
}
