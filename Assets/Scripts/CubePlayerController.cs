﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CubePlayerController : NetworkBehaviour
{

    public float speed = 1.0f;

    void HandleMovement() {
        if (IsLocalPlayer) {
            var vertical = Input.GetAxis("Vertical");
            var horizontal = Input.GetAxis("Horizontal");
            var movement = new Vector3(horizontal * speed * Time.deltaTime, vertical * speed * Time.deltaTime, 0);
            Debug.Log(movement);
            transform.position = transform.position  + movement;
        }
    }

    private void Update()
    {
        HandleMovement();
    }
}
