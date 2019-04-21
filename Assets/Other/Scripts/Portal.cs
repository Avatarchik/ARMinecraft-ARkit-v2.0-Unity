﻿/*
 * This Class handles flipping the global _StencilTest property when the device
 * moves throught the portal, from any direction
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityStandardAssets.CrossPlatformInput;
public class Portal : MonoBehaviour
{

    public Transform device;
    public GameObject moveUI;
    public bool canMove = false;

    //bool for checking if the device is not in the same direction as it was
    bool wasInFront;
    //bool for knowing that on the next change of state, what to set the stencil test
    bool inOtherWorld;

    //This bool is on while device colliding, done so we ensure the shaders are being updated before render frames
    //Avoids flickering
    bool isColliding;
    Rigidbody rb;

    void Start()
    {
        //start outside other world
        SetMaterials(false);
        moveUI.SetActive(false);
        rb = Camera.main.GetComponent<Rigidbody>();
    }

    void SetMaterials(bool fullRender)
    {
        var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
        Shader.SetGlobalInt("_StencilTest", (int)stencilTest);
    }

    bool GetIsInFront()
    {
        Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;

        Vector3 pos = transform.InverseTransformPoint(worldPos);
        return pos.z >= 0 ? true : false;
    }


    //This technique registeres if the device has hit the portal, flipping the bool

    void OnTriggerEnter(Collider other)
    {
        if (other.transform != device)
            return;
        //Important to do this for if the user re-enters the portal from the same side
        wasInFront = GetIsInFront();
        isColliding = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform != device)
            return;
        isColliding = false;
    }


    /*If there has been a change in the relative position of the device to the portal, flip the
     *Stencil Test
     */

    void WhileCameraColliding()
    {
        if (!isColliding)
            return;
        bool isInFront = GetIsInFront();
        if ((isInFront && !wasInFront) || (wasInFront && !isInFront))
        {
            inOtherWorld = !inOtherWorld;
            SetMaterials(inOtherWorld);
            if(canMove){
                //moveUI.SetActive(inOtherWorld);
            }

        }
        wasInFront = isInFront;
    }

    void OnDestroy()
    {
        //ensure geometry renders in the editor
        SetMaterials(true);
    }


    void Update()
    {
        WhileCameraColliding();

        if(canMove){
            /*
            float x = CrossPlatformInputManager.GetAxis("Horizontal");
            float y = CrossPlatformInputManager.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(x, 0, y);
            rb.velocity = movement * 2f;

            if(x!=0 && y!=0){
               Camera.main.transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x,Mathf.Atan2(x,y)* Mathf.Rad2Deg,Camera.main.transform.eulerAngles.z);
            }
            */
        }
    }
}
