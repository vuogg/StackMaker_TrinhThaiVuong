using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Camera;
    public Transform Player;

    [SerializeField] private Vector3 offset;

    private void Update()
    {
        Camera.position = Vector3.Lerp(Camera.position, Player.position + offset, Time.deltaTime * 5f);
    }
}
