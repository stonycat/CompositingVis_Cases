using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Tilia.Interactions.Interactables.Interactables;

public class Node : MonoBehaviour
{

    //public List<Node> connected_nodes = new List<Node> ();
    public bool isMoving;
    public bool isGrabbing;
    public int id;
    public bool AnimateCompose;
    public Graph ParentGraph;

    private GameObject epf;
    private List<GameObject> edges = new List<GameObject>();
    private List<SpringJoint> joints = new List<SpringJoint>();
    private Color nodeColor;
    private Color nodeTouchColor;
    private Color nodeInColor;
    private MeshRenderer meshRenderer;
    private GameObject subObj;
    private InteractableTest interactableStatus;
    private bool collided;

    void Start()
    {
        collided = false;
        interactableStatus = GetComponent<InteractableTest>();
        meshRenderer = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>();
        //nodeObj = transform.GetChild(0).GetChild(1).gameObject;
        nodeColor = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color;
        nodeColor.a = 1.0f;
        meshRenderer.material.SetColor("_Color", nodeColor);
        nodeTouchColor = new Color(nodeColor.r, 12, nodeColor.b, 0.3f);
        nodeInColor = nodeColor;
        nodeInColor.a = 0.3f;
    }
    private void FixedUpdate()
    {
        foreach (SpringJoint sj in joints)
        {
            GameObject target = sj.connectedBody.gameObject;
            Node n = GetComponent<Node>();
            Node m = target.GetComponent<Node>();
            n.AddAttrForce(m, 0.001f);
            m.AddAttrForce(n, 0.001f);
        }
    }

    void Update()
    {
        isMoving = interactableStatus.isMoving && collided;
        isGrabbing = interactableStatus.interactable.IsGrabbed;
        int i = 0;
        foreach (GameObject edge in edges)
        {
            edge.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            SpringJoint sj = joints[i];
            GameObject target = sj.connectedBody.gameObject;
            edge.transform.LookAt(target.transform);
            Vector3 ls = edge.transform.localScale;
            ls.z = Vector3.Distance(transform.position, target.transform.position);
            edge.transform.localScale = ls;
            edge.transform.position = new Vector3((transform.position.x + target.transform.position.x) / 2,
                        (transform.position.y + target.transform.position.y) / 2,
                        (transform.position.z + target.transform.position.z) / 2);
            i++;
        }
        subObj.transform.LookAt(subObj.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Node>() != null || collision.gameObject.tag == "Edge")
        {
            // ignore collision with other nodes and edges
            return;
        }
        //Debug.Log("Tag: " + collision.gameObject.tag + "; name: " + collision.gameObject.name);
        //if (collision.gameObject.tag == "BarCopy")
        //{
        //    Debug.Log("Color changed");
        //    meshRenderer.material.SetColor("_Color", nodeTouchColor);
        //    //StartCoroutine(AdjustScale());
        //    Debug.Log("Scale: " + transform.localScale);
        //    return;
        //}
        if (collision.gameObject.GetComponent<InteractableTest>() == null)
        {
            return;
        }
        if (collision.gameObject.GetComponent<InteractableTest>().interactable.IsGrabbed == true)
        {
            //meshRenderer.material.SetColor("_Color", nodeTouchColor);
            //transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            return;
        }
        Debug.Log(this.name + " collided!");


        //if (AnimateCompose) ParentGraph.trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().GraphCompose(this);
        //else
        //{
        //    SetObj();
        //    collision.gameObject.SetActive(false);
        //}
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HandCollider" && collided)
        {
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            GetComponent<Rigidbody>().mass = 3f;
        }
        if (other.tag == "BarCopy" && !collided)
        {
            Debug.Log("lalala");
            if (other.transform.parent.parent.parent.gameObject.GetComponent<InteractableTest>().interactable.IsGrabbed == true)
            {
                //transform.position = Vector3.zero;
                meshRenderer.material.SetColor("_Color", nodeTouchColor);
                //GetComponent<Rigidbody>().WakeUp();
                transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                return;
            }
            if (AnimateCompose)
            {
                //transform.localPosition = 0.05f * Vector3.one;
                //ParentGraph.trackedInteractableOld = other.transform.parent.GetComponent<BarCopyObjectInteractable>().origin;
                //ParentGraph.trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().GraphCompose(this);
                transform.localScale = 0.05f * Vector3.one;
                other.transform.parent.GetComponent<BarCopyObjectInteractable>().origin.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().GraphCompose(this);
            }
            else
            {
                SetObj();
                other.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "HandCollider" && collided)
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetComponent<Rigidbody>().mass = 1f;
        }
        if (other.tag == "BarCopy" && !collided)
        {
            meshRenderer.material.SetColor("_Color", nodeColor);
            //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Node>() != null || collision.gameObject.tag == "Edge")
        {
            // ignore collision with other nodes and edges
            return;
        }
        meshRenderer.material.SetColor("_Color", nodeColor);
        //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        collided = false;
    }

    public void SetEdgePrefab(GameObject epf)
    {
        this.epf = epf;
    }

    public void AddEdge(Node n)
    {
        SpringJoint sj = gameObject.AddComponent<SpringJoint>();
        sj.autoConfigureConnectedAnchor = false;
        sj.anchor = new Vector3(0, 0.5f, 0);
        sj.connectedAnchor = new Vector3(0, 0, 0);
        sj.enableCollision = true;
        sj.connectedBody = n.GetComponent<Rigidbody>();
        //sj.damper = 10;
        //sj.spring = 20;
        GameObject edge = Instantiate(this.epf, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        edge.SetActive(true);
        edge.transform.parent = transform.parent;
        edges.Add(edge);
        joints.Add(sj);
    }

    public float DistanceToObj(GameObject obj)
    {
        if (collided) return 0;
        return Vector3.Distance(transform.position, obj.transform.position);
    }

    public void SetText(string str)
    {
        TextMesh textMesh = transform.GetChild(0).GetComponent<TextMesh>();
        textMesh.text = str;
        textMesh.transform.LookAt(textMesh.transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
    }

    public void HighlightNode()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void DeHighlightNode()
    {
        transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    public void SetObj()
    {
        if (collided) return;
        meshRenderer.material.SetColor("_Color", nodeInColor);
        ParentGraph.trackedInteractable = ParentGraph.trackedInteractableOld;
        subObj.SetActive(true);
        collided = true;
    }

    public void InitObj(Transform trackedObjTransform)
    {
        GameObject trackedObj = trackedObjTransform.GetChild(0).GetChild(1).gameObject;
        subObj = Instantiate(trackedObj, trackedObjTransform.position, trackedObjTransform.rotation);
        StackedBarDraw stackedBarDraw = subObj.GetComponent<StackedBarDraw>();
        stackedBarDraw.MaxX = 0.56f;
        stackedBarDraw.MinX = -0.44f;
        stackedBarDraw.attr = new Dictionary<string, List<float>>();
        stackedBarDraw.isStacked = false;
        stackedBarDraw.useDifferentMaterialEachBar = true;
        stackedBarDraw.materials = trackedObj.GetComponent<StackedBarDraw>().materials;
        foreach (string s in trackedObj.GetComponent<StackedBarDraw>().attr.Keys)
        {
            stackedBarDraw.attr.Add(s, new List<float> { trackedObj.GetComponent<StackedBarDraw>().attr[s][id] });
        }
        stackedBarDraw.ResetData();
        stackedBarDraw.CreateChart();
        subObj.transform.parent = transform.GetChild(0).GetChild(1);
        subObj.transform.localPosition = new Vector3(0, 0f, 0);
        subObj.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        subObj.GetComponent<BoxCollider>().enabled = false;
        subObj.SetActive(false);
    }
    public void BreakObj()
    {
        if (!collided) return;
        collided = false;
        subObj.SetActive(false);
        meshRenderer.material.SetColor("_Color", nodeColor);
        GetComponent<Rigidbody>().mass = 1f;
    }

    public void AddRepForce(Node n)
    {
        Rigidbody thisNode = GetComponent<Rigidbody>();
        Vector3 force = transform.position - n.transform.position;
        float r = force.magnitude;
        Vector3 direction = force.normalized;
        thisNode.AddForce(direction * 0.5f * (n.GetComponent<Rigidbody>().mass * thisNode.mass / (r * r)));
    }

    public void AddAttrForce(Node n, float dist)
    {
        Rigidbody thisNode = GetComponent<Rigidbody>();
        Vector3 force = n.transform.position - transform.position;
        float r = force.magnitude;
        Vector3 direction = force.normalized;
        thisNode.AddForce(direction * 100 * (r - dist));
    }

    public GameObject UnGrab()
    {
        interactableStatus.interactable.Ungrab();
        return interactableStatus.currentAttachInteractor;
    }
}
