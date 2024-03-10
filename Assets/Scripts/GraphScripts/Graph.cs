using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactables.Grab.Action;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Graph : MonoBehaviour
{
    public GameObject trackedInteractable;
    public GameObject trackedInteractableOld;
    public List<Node> nodeList;
    public TextAsset file;
    public GameObject nodepf;
    public GameObject edgepf;
    public float width;
    public float length;
    public float height;
    public bool AnimateCompose;

    private List<GameObject> nodes;
    private List<float> distances;
    private int numNodes;
    private ClosestNodeListener closestNodeListener;
    private MinDistanceListener minDistanceListener;
    private int grabNodeIdx;
    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<GameObject>();
        distances = new List<float>();
        closestNodeListener = new ClosestNodeListener();
        closestNodeListener.OnVariableChange += ClosestNodeListener_OnVariableChange;
        minDistanceListener = new MinDistanceListener();
        minDistanceListener.OnVariableChange += updateNodes;
        trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().MaxX = 0.46f;
        trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().MinX = -0.54f;
        trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().Loading();
        LoadGMLFromFile(file);
        trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().CreateChart();
        if (AnimateCompose) { SetAnimationTarget(); }
    }

    private void updateNodes(float minDist)
    {
        if (minDist > 0.5f)
        {
            closestNodeListener.ClosestNode = -1;
            DeHighlightAll();
        }
        else
        {
            closestNodeListener.ClosestNode = distances.IndexOf(minDist);
        }
        if (minDist == 0)
        {
            // composited
            for (int i = 0; i < numNodes; i++)
            {
                Node node = nodes[i].GetComponent<Node>();
                node.SetObj();
                node.HighlightNode();
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = i + 1; j < nodeList.Count; j++)
            {
                nodeList[i].AddRepForce(nodeList[j]);
                nodeList[j].AddRepForce(nodeList[i]);
            }
        }
    }

    private void DeHighlightAll()
    {
        foreach (GameObject child in nodes)
        {
            child.GetComponent<Node>().DeHighlightNode();
        }
    }

    private void ClosestNodeListener_OnVariableChange(int newVal)
    {
        DeHighlightAll();
        nodes[newVal].GetComponent<Node>().HighlightNode();
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = false;
        int grabNode = -1;
        for (int i = 0; i < numNodes; i++)
        {
            Node node = nodes[i].GetComponent<Node>();
            isMoving = isMoving || node.isMoving;
            if (node.isGrabbing)
            {
                grabNode = i;
            }
            float dist = node.DistanceToObj(trackedInteractable);
            distances[i] = dist;
            //node.SetText(dist.ToString("F2"));
        }
        isMoving = isMoving && (grabNode > -1);
        minDistanceListener.MinDist = distances.Min();      
        if (isMoving)
        {
            // Breaking
            for (int i = 0; i < numNodes; i++)
            {
                Node node = nodes[i].GetComponent<Node>();
                node.BreakObj();
                node.DeHighlightNode();
            }
            GameObject controller = nodes[grabNode].GetComponent<Node>().UnGrab();
            nodes[grabNode].GetComponent<SphereCollider>().enabled = false;
            grabNodeIdx = grabNode;
            trackedInteractable.SetActive(true);
            //trackedObj.GetComponent<InteractableTest>().SetGrabOffset(0);
            trackedInteractable.transform.position = controller.transform.position;
            trackedInteractable.transform.LookAt(trackedInteractable.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            trackedInteractable.GetComponent<InteractableTest>().interactable.Grab(controller);
            if (AnimateCompose) trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().GraphDecompose();
            StartCoroutine(EnableCollider());
        }
    }

    

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1);
        nodes[grabNodeIdx].GetComponent<SphereCollider>().enabled = true;
        //trackedObj.GetComponent<InteractableTest>().interactable.Ungrab();
        //trackedObj.GetComponent<InteractableTest>().SetGrabOffset(2);
    }

    public void LoadGMLFromFile(TextAsset f)
    {
        if (f == null) return;
        string[] lines = f.text.Split('\n');
        int currentobject = -1; // 0 = graph, 1 = node, 2 = edge
        int stage = -1; // 0 waiting to open, 1 = waiting for attribute, 2 = waiting for id, 3 = waiting for label, 4 = waiting for source, 5 = waiting for target
        Node n = null;
        Dictionary<string, Node> nodeDict = new Dictionary<string, Node>();
        foreach (string line in lines)
        {
            string l = line.Trim();
            string[] words = l.Split(' ');
            foreach (string word in words)
            {
                if (word == "graph" && stage == -1)
                {
                    currentobject = 0;
                }
                if (word == "node" && stage == -1)
                {
                    currentobject = 1;
                    stage = 0;
                }
                if (word == "edge" && stage == -1)
                {
                    currentobject = 2;
                    stage = 0;
                }
                if (word == "[" && stage == 0 && currentobject == 2)
                {
                    stage = 1;
                }
                if (word == "[" && stage == 0 && currentobject == 1)
                {
                    stage = 1;
                    GameObject go = Instantiate(nodepf, new Vector3(Random.Range(transform.position.x - width / 2, transform.position.x + width / 2), Random.Range(transform.position.y - length / 2, transform.position.y + length / 2), Random.Range(transform.position.z - height / 2, transform.position.z + height / 2)), Quaternion.identity);
                    go.SetActive(true);
                    n = go.GetComponent<Node>();
                    n.ParentGraph = this;
                    n.transform.parent = transform;
                    n.AnimateCompose = AnimateCompose;
                    n.SetEdgePrefab(edgepf);
                    nodes.Add(go);
                    continue;
                }
                if (word == "]")
                {
                    stage = -1;
                }
                if (word == "id" && stage == 1 && currentobject == 1)
                {
                    stage = 2;
                    continue;
                }
                if (word == "label" && stage == 1 && currentobject == 1)
                {
                    stage = 3;
                    continue;
                }
                if (stage == 2)
                {
                    nodeDict.Add(word, n);
                    n.name = word;
                    distances.Add(0f);
                    stage = 1;
                    break;
                }
                if (stage == 3)
                {
                    n.name = word;
                    stage = 1;
                    break;
                }
                if (word == "source" && stage == 1 && currentobject == 2)
                {
                    stage = 4;
                    continue;
                }
                if (word == "target" && stage == 1 && currentobject == 2)
                {
                    stage = 5;
                    continue;
                }
                if (stage == 4)
                {
                    n = nodeDict[word];
                    stage = 1;
                    break;
                }
                if (stage == 5)
                {
                    n.AddEdge(nodeDict[word]);
                    stage = 1;
                    break;
                }
            }
        }
        nodeList = nodeDict.Values.ToList();
        numNodes = nodeList.Count;
        SetNodeSubObjs();
    }

    private void SetNodeSubObjs()
    {
        trackedInteractable.SetActive(false);
        for (int i = 0; i < numNodes; i++)
        {
            nodeList[i].id = i;
            nodeList[i].InitObj(trackedInteractable.transform);
        }
        trackedInteractable.SetActive(true);
    }

    private void SetAnimationTarget()
    {
        Dictionary<string, Transform> targets = new Dictionary<string, Transform>();
        for (int i = 0; i < numNodes; i++)
        {
            //Debug.Log("name " + nodeList[i].name);
            targets.Add(nodeList[i].name, nodeList[i].transform);
        }
        trackedInteractable.transform.GetChild(0).GetChild(1).GetComponent<StackedBarDraw>().AddGraphBarMoveAnimation(targets);
    }
}
