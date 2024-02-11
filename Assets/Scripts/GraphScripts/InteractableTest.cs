using System.Collections;
using System.Collections.Generic;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactables.Grab.Action;
using Tilia.Interactions.Interactables.Interactors;
using UnityEditor;
using UnityEngine;

public class InteractableTest : MonoBehaviour
{
    public InteractableFacade interactable;
    public GameObject leftInteractor;
    public GameObject rightInteractor;
    public bool isMoving;
    public GameObject currentAttachInteractor;

    private float velocity;
    private Vector3 previousPos;
    // Start is called before the first frame update
    void Start()
    {
        velocity = 0;
        previousPos = transform.position;
        interactable = GetComponent<InteractableFacade>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (transform.position - previousPos).magnitude / Time.deltaTime;
        isMoving = velocity > 1;
        currentAttachInteractor = (leftInteractor.GetComponent<InteractorFacade>().GrabbedObjects.Count > 0) ? leftInteractor : (rightInteractor.GetComponent<InteractorFacade>().GrabbedObjects.Count > 0) ? rightInteractor : null;
        previousPos = transform.position;
        //if (!interactable.IsGrabbed) SetGrabOffset(2);
    }

    public void SetGrabOffset(int option)
    {

        // Set "Primary Action" to "Follow"
        int primaryActionIndex = 1; // Follow
        GameObject primaryActionPrefab = (GameObject)PrefabUtility.InstantiatePrefab(interactable.Configuration.GrabConfiguration.ActionTypes.NonSubscribableElements[primaryActionIndex], interactable.Configuration.GrabConfiguration.ActionTypes.transform);
        GrabInteractableAction primaryAction = primaryActionPrefab.GetComponent<GrabInteractableAction>();
        interactable.Configuration.GrabConfiguration.PrimaryAction = primaryAction;

        // Set "Grab Offset" to "None"
        GrabInteractableFollowAction followAction = (GrabInteractableFollowAction)primaryAction;
        SerializedObject actionObject = new SerializedObject(followAction);
        SerializedProperty foundProperty = actionObject.FindProperty("grabOffset");
        foundProperty.intValue = option; // None
        foundProperty.serializedObject.ApplyModifiedProperties();

        //int secondaryActionIndex = 5; // Scale
        //GameObject secondaryActionPrefab = (GameObject)PrefabUtility.InstantiatePrefab(interactable.Configuration.GrabConfiguration.ActionTypes.NonSubscribableElements[secondaryActionIndex], interactable.Configuration.GrabConfiguration.ActionTypes.transform);
        //GrabInteractableAction secondaryAction = secondaryActionPrefab.GetComponent<GrabInteractableAction>();
        //interactable.Configuration.GrabConfiguration.SecondaryAction = secondaryAction;
    }

}
