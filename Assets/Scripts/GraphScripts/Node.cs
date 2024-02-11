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

    private GameObject epf;
    private List<GameObject> edges = new List<GameObject>();
    private List<SpringJoint> joints = new List<SpringJoint>();
    private Color nodeColor;
    private Color nodeTouchColor;
    private Color nodeInColor;
    private MeshRenderer meshRenderer;
    private GameObject nodeObj;
    private GameObject subObj;
    private InteractableTest interactableStatus;
    private bool collided;

    void Start()
    {
        collided = false;
        interactableStatus = GetComponent<InteractableTest>();
        meshRenderer = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>();
        nodeObj = transform.GetChild(0).GetChild(1).gameObject;
        nodeColor = transform.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color;
        nodeColor.a = 1.0f;
        meshRenderer.material.SetColor("_Color", nodeColor);
        nodeTouchColor = new Color(nodeColor.r, 12, nodeColor.b, 0.3f);
        nodeInColor = nodeColor;
        nodeInColor.a = 0.3f;
        //transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        //transform.GetChild(0).gameObject.SetActive(false);
        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        //for (int i = 0; i < connected_nodes.Count; i++) 
        //{
        //    Node node = connected_nodes[i];
        //    AddEdge(node);
        //    AddAttrForce(node, 1);
        //    node.AddAttrForce(GetComponent<Node>(), 1);
        //}
        //nodeObj.transform.LookAt(Vector3.up);
    }
    private void FixedUpdate()
    {
        foreach (GameObject edge in edges)
        {
            int i = 0;
            SpringJoint sj = joints[i];
            GameObject target = sj.connectedBody.gameObject;
            Node n = GetComponent<Node>();
            Node m = target.GetComponent<Node>();
            n.AddAttrForce(m, 0.001f);
            m.AddAttrForce(n, 0.001f);
            i++;
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
        if (collision.gameObject.GetComponent<InteractableTest>().interactable.IsGrabbed == true)
        {
            meshRenderer.material.SetColor("_Color", nodeTouchColor);
            return;
        }
        Debug.Log(this.name + " collided!");

        collision.gameObject.SetActive(false);
        SetObj();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " " + other.tag);
        if (other.tag == "HandCollider")
        {
            Debug.Log(name + " enlarged");
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "HandCollider")
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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

        //subObj.transform.LookAt(subObj.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        //subObj.transform.LookAt(nodeObj.transform.rotation * Vector3.up);
        //subObj.GetComponent<BoxCollider>().enabled = false;
        subObj.SetActive(true);
        collided = true;
    }

    public void InitObj(Transform trackedObjTransform)
    {
        //Debug.Log(name + " Init");
        subObj = Instantiate(trackedObjTransform.GetChild(0).GetChild(1).gameObject, trackedObjTransform.position, trackedObjTransform.rotation);
        subObj.transform.parent = transform.GetChild(0).GetChild(1);
        subObj.transform.localPosition = new Vector3(0, 0, 0);
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
