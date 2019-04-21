using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTarget : MonoBehaviour 
{
    [SerializeField]
    Renderer targetRenderer;

    public Transform spawnTransform 
    {
        get
        {
            return targetRenderer.transform;
        }
    }


    [HideInInspector]
    public Transform originTransform;

	void Start()
	{
        if(originTransform==null)
        {
            originTransform = transform.parent;
        }
	}

	public void SetRenderer(bool show)
    {
        targetRenderer.enabled = show;
    }


}
