using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedBarView : MonoBehaviour
{
    public Graph graph;
    public StackedBarDraw Parent;
    public InteractableTest ParentInteractable;
    public InteractableTest BarObjectInteractable;
    public Transform BarObjectTransform;

    private BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = Parent.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Parent.BarCopyAllowed) return;
        if (ParentInteractable.velocity > 2 && ParentInteractable.currentAttachInteractor != null)
        {
            boxCollider.isTrigger = true;
            if (BarObjectTransform.childCount == 0) { 
                for (int i = 0; i < Parent.attr.Keys.Count; i++)
                {
                    GameObject template = transform.GetChild(i + 1).gameObject;
                    GameObject barCopy = Instantiate(template);
                    barCopy.tag = "BarCopy";
                    barCopy.transform.parent = BarObjectTransform;
                    barCopy.transform.localPosition = new Vector3(0, template.transform.localPosition.y, template.transform.localPosition.z);
                    barCopy.transform.localScale = template.transform.localScale;
                    barCopy.GetComponent<GraphBarMoveAnimation>().Destroy();
                    barCopy.GetComponent<BoxCollider>().enabled = true;
                    //barCopy.GetComponent<BoxCollider>().isTrigger = false;
                }
                graph.trackedInteractableOld = graph.trackedInteractable;
            }
            GameObject interator = ParentInteractable.currentAttachInteractor;
            ParentInteractable.interactable.Ungrab();
            BarObjectInteractable.gameObject.SetActive(true);
            BarObjectInteractable.transform.position = interator.transform.position;
            BarObjectInteractable.transform.LookAt(BarObjectInteractable.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            BarObjectInteractable.interactable.Grab(interator);
            graph.trackedInteractable = BarObjectInteractable.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Parent.BarCopyAllowed)
        {
            Parent.BarCopyAllowed = true;
            //graph.trackedInteractable = Parent.gameObject;
        }
    }
}
