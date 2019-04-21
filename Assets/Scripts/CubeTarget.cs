using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTarget : MonoBehaviour
{
    Renderer renderer;

    [SerializeField]
    Material normalMaterial;
    [SerializeField]
    Material weakMaterial;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    public void SetRenderer(bool normal)
    {
        renderer.material = normal ? normalMaterial : weakMaterial;
    }
}
