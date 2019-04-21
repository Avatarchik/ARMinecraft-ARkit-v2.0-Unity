using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;


public class ItemPlacement : MonoBehaviour
{
    UnityARCameraManager manager;

    [SerializeField]
    GameObject[] ItemPrefabs;
    int Index = 0;

    FocusSquare focusSquare;
    [SerializeField]
    GameObject focusIndicator;

    [SerializeField]
    Image Icon_BuildBtn;
    [SerializeField]
    Sprite Icon_Builder;
    [SerializeField]
    Sprite Icon_Destoryer;

    bool IsEnabled = true;
    bool IsHitCubeSurfaces = false;
    bool IsBuildMode = true;

    SurfaceTarget currSurfaceTarget;
    SurfaceTarget lastSurfaceTarget;
    CubeTarget currActualTarget;
    CubeTarget lastActualTarget;

    // Start is called before the first frame update
    void Start()
    {
        focusSquare = this.GetComponent<FocusSquare>();

        if (IsBuildMode)
        {
            Icon_BuildBtn.sprite = Icon_Builder;
        }
        else
        {
            Icon_BuildBtn.sprite = Icon_Destoryer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit;

        if (IsEnabled)
        {
            if (Physics.Raycast(ray, out hit))
            {
                IsHitCubeSurfaces = (hit.transform.gameObject.tag == "placearea") ? true : false;

                if (hit.transform.gameObject.GetComponent<SurfaceTarget>() != null)
                {
                    lastSurfaceTarget = currSurfaceTarget;
                    currSurfaceTarget = hit.transform.gameObject.GetComponent<SurfaceTarget>();
                    lastActualTarget = currActualTarget;
                    currActualTarget = currSurfaceTarget.originTransform.GetComponent<CubeTarget>();
                }

                if(IsHitCubeSurfaces)
                {
                    if (IsBuildMode)
                    {
                        if (currSurfaceTarget != lastSurfaceTarget)
                            lastSurfaceTarget.SetRenderer(false);
                        
                        currActualTarget.SetRenderer(true); 
                        currSurfaceTarget.SetRenderer(true);
                        focusIndicator.transform.position = currSurfaceTarget.transform.position;
                        focusIndicator.transform.rotation = currSurfaceTarget.spawnTransform.rotation;
                    }
                    else
                    {
                        if (currActualTarget != lastActualTarget)
                            lastActualTarget.SetRenderer(true);
                        
                        currActualTarget.SetRenderer(false);
                        focusIndicator.transform.position = currActualTarget.transform.position;
                        focusIndicator.transform.rotation = Quaternion.identity;
                    }
                }
                else
                {
                    currActualTarget.SetRenderer(true);
                    currSurfaceTarget.SetRenderer(false);
                }
            }
        }

    }

    public void SelectItem(int index)
    {
        Index = index;
    }

    public void Reset()
    {
        IsEnabled = true;
        IsHitCubeSurfaces = false;
        focusIndicator.SetActive(true);
        var planes = GameObject.FindGameObjectsWithTag("DebugPlane");
        foreach (GameObject item in planes)
        {
            item.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        }
        var cubes = GameObject.FindGameObjectsWithTag("cubes");
        foreach (GameObject item in cubes)
        {
            Destroy(item);
        }
    }

    public void SwitchBuildMode()
    {
        currSurfaceTarget.SetRenderer(false);
        IsBuildMode = !IsBuildMode;
        Icon_BuildBtn.sprite = (IsBuildMode) ? Icon_Builder : Icon_Destoryer;
    }

    public void OnItemPlaced()
    {
        if (focusSquare.SquareState != FocusSquare.FocusState.Found || ItemPrefabs.Length == 0) return;

        /*
        UnityARSessionNativeInterface.GetARSessionNativeInterface()
                                     .RunWithConfigAndOptions(manager.sessionConfiguration, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors);
        */

        if (IsHitCubeSurfaces)
        {
            lastSurfaceTarget.SetRenderer(false);
            currSurfaceTarget.SetRenderer(false);
            if (IsBuildMode)
            {
                Instantiate(ItemPrefabs[Index], currSurfaceTarget.spawnTransform.position, currSurfaceTarget.spawnTransform.rotation);
            }
            else
            {
                Destroy(currActualTarget.gameObject);
            }

        }
        else
        {
            if (!IsBuildMode) return;
            var screenPosition = Camera.main.ScreenToViewportPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            ARPoint pt = new ARPoint
            {
                x = screenPosition.x,
                y = screenPosition.y
            };

            // Try to hit within the bounds of an existing AR plane.
            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(
                                                   pt,
                                                   ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);

            if (hitResults.Count > 0)
            { // If a hit is found, set the position and reset the rotation.
                var pos = UnityARMatrixOps.GetPosition(hitResults[0].worldTransform);
                Instantiate(ItemPrefabs[Index], new Vector3(pos.x, pos.y + ItemPrefabs[Index].transform.localScale.y * 0.55f, pos.z), focusIndicator.transform.rotation);
            }
        }
    }

}





