using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandAnimation : MonoBehaviour
{
    bool isRotating = false;
    bool isMoving = false;
    float zRotationMax = 5;
    float zRotationMin = -5;
    float yPositionMaxOffset = 0.01f; // Déplacement maximum en Y
    float yPositionMinOffset = -0.01f; // Déplacement minimum en Y
    float movementSpeed = 2f; // Vitesse du mouvement en Y
    float initialYPosition; // Position de départ en Y
    float rotationSpeed; // Vitesse de la rotation
    private void Start()
    {
        isRotating = Random.Range(0, 2) == 0;
        isMoving = Random.Range(0, 2) == 0;

        // Enregistrer la position initiale en Y
        initialYPosition = transform.position.y;
        rotationSpeed = Random.Range(0, 5);
    }
    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            float zRotation = Mathf.Lerp(zRotationMin, zRotationMax, Mathf.PingPong(Time.time * rotationSpeed, 1));

            transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }
        if (isMoving)
        {
            float yOffset = Mathf.Lerp(yPositionMinOffset, yPositionMaxOffset, Mathf.PingPong(Time.time * movementSpeed, 1));
            transform.position = new Vector3(transform.position.x, initialYPosition + yOffset, transform.position.z);
        }
    }
}
