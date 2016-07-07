﻿using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{

    public Rigidbody rigidbody;
    private bool currentlyInteracting;
    private WandController attachedWand;
    private Transform interactionPoint;
    private Vector3 posDelta;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;
    private float rotationFactor = 40;
    private float velocityFactor = 20000f;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform;
        velocityFactor /= rigidbody.mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand != null && currentlyInteracting)
        {
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta.normalized * velocityFactor * Time.fixedDeltaTime * Mathf.Pow(posDelta.magnitude, 1);

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * Mathf.Pow(angle, 2) * axis) * rotationFactor;
        }
    }

    public void BeginInteraction(WandController wand)
    {
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(WandController wand)
    {
        if (wand == attachedWand)
        {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }
}
