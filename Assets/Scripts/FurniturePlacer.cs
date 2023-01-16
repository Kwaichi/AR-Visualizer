using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FurniturePlacer : MonoBehaviour
{
    public Transform placementIndicator;
    public GameObject selectionUI;
    //public GameObject placementUI;
    //public GameObject infoUI;
    private List<GameObject> furniture = new List<GameObject>();
    private GameObject curSelected;
    private Camera cam;

    //public ObjectInfoText infoText;
    //public ObjectText infoText;
    //public Text nameText1;
    public TMP_Text nameText;
    public TMP_Text infoText;

    public Animator animator;
    public Animator animatorLibrary;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        selectionUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
        {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject != null && furniture.Contains(hit.collider.gameObject))
                {
                    if(curSelected != null && hit.collider.gameObject != curSelected)    //mam prave vybraty objekt a nie je to prave vybraty objekt?                
                        Select(hit.collider.gameObject);
                    else if(curSelected == null) //ak nemam prave vybraty objekt, vyber ho taktiez
                        Select(hit.collider.gameObject);                    
                }
                
            }
            else //ak raycast prave na nic nemieri
            Deselect();
        }

        if(curSelected != null && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
           MoveSelected();
    }

    void Select (GameObject selected)
    {
        if(curSelected != null)
        ToggleSelectionVisual(curSelected, false);

        curSelected = selected;
        ToggleSelectionVisual(curSelected, true);
        selectionUI.SetActive(true);
    }

    void MoveSelected ()
    {
        //nebudem hybat  podla poctu pixelov - displej z vyssim pix count - rychjelsi pohyb
        Vector3 curPos = cam.ScreenToViewportPoint(Input.touches[0].position); //zistim pohyb prsta po displeji
        Vector3 lastPos = cam.ScreenToViewportPoint(Input.touches[0].position - Input.touches[0].deltaPosition);

        Vector3 touchDir = curPos - lastPos; // zistim smer pohybu

        Vector3 camRight = cam.transform.right; //zorientujem polohu objektu v priestore relativne/normalizovane ku kamere
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        curSelected.transform.position += (camRight * touchDir.x + camForward * touchDir.y); //a budem hybat objektom vzhladom na aktualnu relativnu polohu kamery, nie SVETOVEHO SUR SYSTEMU 
    }

    void Deselect ()
    {
        if(curSelected != null)
        ToggleSelectionVisual(curSelected, false);

        curSelected = null;
        selectionUI.SetActive(false);
    }

    void ToggleSelectionVisual (GameObject obj, bool toggle)
    {
        obj.transform.Find("Selected").gameObject.SetActive(toggle);
    }

    public void PlaceFurniture (GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, placementIndicator.position, Quaternion.identity); //Quaternion. identity - zachovam rotaciu prefabu
        furniture.Add(obj); //priudam objekt do Listu

        //select the object after placement
        Select(obj);
    }

    public void ScaleSelected (float rate)
    {
        curSelected.transform.localScale += Vector3.one * rate;
    }

    public void RotateSelected (float angle)
    {
        curSelected.transform.eulerAngles += Vector3.up * angle;
    }
    public void SetColour (Image buttonImage)
    {
        MeshRenderer[] meshRenderers = curSelected.GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer mr in meshRenderers)
        {
            if(mr.gameObject.name == "Selected")
              continue;
            
            mr.material.color = buttonImage.color;
        }
    }

    public void TriggerInfo()
    {
        //FindObjectOfType<TextManager>().DisplayInfo(infoText); //dialogue - parameter, ktoru konverzaciu mam zacat
        //curSelected.GetComponent<ObjectInfoTrigger>().nameTxt = nameText1;

        /*curSelected.GetComponent<ObjectText>().objectName = nameText;
        curSelected.GetComponent<ObjectText>().objectInfo = infoText;*/
        GameObject sel = Instantiate(curSelected);
        sel.GetComponent<ObjectText>().objectName = nameText;
        sel.GetComponent<ObjectText>().objectInfo = infoText;

        //nameText.Clear();
        //infoText.Clear();

        animator.SetBool("IsOpen", true);

        Destroy(sel,1);
    }

    public void CloseInfo()
    {
        animator.SetBool("IsOpen", false);
    }

    public void DeleteSelected ()
    {
        furniture.Remove(curSelected);
        Destroy(curSelected);
        Deselect();
    }

    public void HideSelectionLibrary ()
    {
        animatorLibrary.SetBool("HideLibrary", true);
    }

    public void ShowSelectionLibrary ()
    {
        animatorLibrary.SetBool("HideLibrary", false);
    }
}
