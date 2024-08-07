using System;
using UnityEngine;
using UnityEngine.Events;
using Tilia.Interactions.Controllables.LinearDriver;
using Tilia.Interactions.Interactables.Interactables;
using UnityAction = UnityEngine.Events.UnityAction<Tilia.Interactions.Interactables.Interactors.InteractorFacade>;
using UnityEngine.PlayerLoop;

namespace IATK
{
    /// <summary>
    /// VR Visualisation class to act as a view controller - reads the model to create the view
    /// </summary>
    [ExecuteInEditMode]
    public class VRVisualisation : Visualisation
    {
        private const bool enableVisualisationScaling = false; // Disabled until scaling bug is fixed in Tilia: shorturl.at/ayFP3
        public GameObject linearJointDrivePrefab;
        private bool xAxisInUse = false;
        private bool yAxisInUse = false;
        private bool zAxisInUse = false;
        private const string linearJointSuffix = "[Linear Joint]";

        void Start()
        {
            // Create Interactable Normalisers
            Axis[] axes = GetComponentsInChildren<Axis>();
            if (axes == null) return;

            //cancel due to set handle
            //foreach (Axis axis in axes)
            //{
            //    ConfigureHandle(axis, "MinAxisHandle", false);
            //    ConfigureHandle(axis, "MaxAxisHandle", true);
            //}
        }

        void Update()
        {
            if (enableVisualisationScaling && transform.hasChanged)
            {
                transform.hasChanged = false;
                Axis[] axes = GetComponentsInChildren<Axis>();
                axes?.ForEach(axis =>
                {
                    Transform minLinerJoint = axis.transform.Find("MinAxisHandle" + " " + linearJointSuffix);
                    LinearDriveFacade minLinerJointFacade = minLinerJoint.GetComponent<LinearDriveFacade>();
                    minLinerJointFacade.Drive.SetUp();

                    Transform maxLinerJoint = axis.transform.Find("MaxAxisHandle" + " " + linearJointSuffix);
                    LinearDriveFacade maxLinerJointFacade = maxLinerJoint.GetComponent<LinearDriveFacade>();
                    maxLinerJointFacade.Drive.SetUp();
                });
            }
        }

        #region Visualisation Handling
        public override void updateViewProperties(AbstractVisualisation.PropertyType propertyType)
        {
            base.updateViewProperties(propertyType);
            //Debug.Log("VR Vis test");

            switch (propertyType)
            {
                case AbstractVisualisation.PropertyType.VisualisationType:
                    // Other visualisation types are not supported yet
                    // To add new visualisation types
                    // Add handles where needed visualisation types
                    // When visualisation type changes, we need to:
                    // Alter Box Collider
                    // Find any new Normaliser draggers and add a Linear Drive to each
                    // Connect all Linear Drives to the appropriate Normaliser values
                    break;
                case AbstractVisualisation.PropertyType.X:
                    xAxisInUse = !theVisualizationObject.visualisationReference.xDimension.Attribute.Equals("Undefined");
                    UpdateVisualisation();
                    break;
                case AbstractVisualisation.PropertyType.Y:
                    yAxisInUse = !theVisualizationObject.visualisationReference.yDimension.Attribute.Equals("Undefined");
                    UpdateVisualisation();
                    break;
                case AbstractVisualisation.PropertyType.Z:
                    zAxisInUse = !theVisualizationObject.visualisationReference.zDimension.Attribute.Equals("Undefined");
                    UpdateVisualisation();
                    break;
            }
        }
        private void UpdateVisualisation()
        {
            int numberOfAxisInUse = 0;
            if (xAxisInUse) numberOfAxisInUse++;
            if (yAxisInUse) numberOfAxisInUse++;
            if (zAxisInUse) numberOfAxisInUse++;

            if (numberOfAxisInUse == 0)
            {
                // If no axis are set then remove the box collider
                DestroyImmediate(gameObject.GetComponent<BoxCollider>());
                //Destroy(gameObject.GetComponent<BoxCollider>());
                return;
            }

            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);

            if (xAxisInUse)
            {
                center.x = 0.5f;
                size.x = 1f;

                // Needs an additional offset if only one axis is used
                if (numberOfAxisInUse == 1) center.y = 0.03f;
            }
            if (yAxisInUse)
            {
                center.y = 0.5f;
                size.y = 1f;

                // Needs an additional offset if only one axis is used
                if (numberOfAxisInUse == 1) center.x = 0.03f;
            }
            if (zAxisInUse)
            {
                center.z = 0.5f;
                size.z = 1f;

                // Needs an additional offset if only one axis is used
                if (numberOfAxisInUse == 1) center.x = 0.03f;
            }

            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null) boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = center;
            boxCollider.size = size;
        }
        public void ConfigureHandle(Axis axis, string handleName, bool isMax)
        {
            int axisDirection = axis.AxisDirection;
            GameObject handle = axis.transform.Find(handleName)?.gameObject;

            // Runs once the first time the scene is played after the visualisation is created
            if (handle != null) ConvertToLinearJointDrive(handle, axisDirection, isMax);

            // Config LinearJointEvents
            Transform linerJoint = axis.transform.Find(handleName + " " + linearJointSuffix);
            Transform interactable = RecursiveFindChild(linerJoint, "Interactions.Interactable");
            LinearDriveFacade linerJointFacade = linerJoint.GetComponent<LinearDriveFacade>();

            LinkGrabEvent(linerJointFacade, interactable);
            LinkScalingEvent(linerJointFacade, axisDirection, isMax);
        }

        /// <summary>
        /// Wraps a given normaliser handle in a Tilia 'LinearJointDrive' and configures the Linear Joint based on a axis direction
        /// </summary>
        /// <param name="handle">The GameObject to wrap</param>
        /// <param name="axisDirection">X=1, Y=2, Z=3</param>
        /// <param name="isMax">True if the normaliser is the maximum filter</param>
        private void ConvertToLinearJointDrive(GameObject handle, int axisDirection, bool isMax)
        {
            handle.SetActive(true);

            GameObject newLinerJointPrefab = Instantiate(linearJointDrivePrefab);
            GameObject linerJoint = WrapObject(handle, newLinerJointPrefab, linearJointSuffix);
            LinearDriveFacade linerJointFacade = linerJoint.GetComponent<LinearDriveFacade>();

            ConfigNormaliserSlider(handle, axisDirection);
            ConfigLinerJoint(linerJoint.transform, axisDirection);
            ConfigLinerJointFacade(linerJointFacade, isMax);
        }
        public void LinkGrabEvent(LinearDriveFacade linerJointFacade, Transform interactable)
        {
            // Connects the linerJointFacade with the interactableFacade so the position of the normaliser object is correct 
            InteractableFacade interactableFacade = interactable.GetComponent<InteractableFacade>();
            UnityAction action = (_) => linerJointFacade.SetTargetValueByStepValue();
            interactableFacade.LastUngrabbed.AddListener(action);
        }
        private void ConfigNormaliserSlider(GameObject handle, int axisDirection)
        {
            // Sets the layer to be layer 3 ("Normaliser") so it won't collide with other normalisers
            handle.layer = 3;

            // Adds a box collider to the normaliser object so that it can interact with the user's 'hands'
            BoxCollider boxCollider = handle.GetComponent<BoxCollider>();
            if (boxCollider == null) boxCollider = handle.AddComponent<BoxCollider>();
            boxCollider.center = Vector3.zero;
            boxCollider.size = new Vector3(3, 3, 3);

            handle.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        }
        private void ConfigLinerJoint(Transform linerJoint, int axisDirection)
        {
            linerJoint.localScale = new Vector3(1f, 1f, 1f);

            // Sets the center position of the normaliser object to the middle of the axis
            // Values are not symmetrical due to slight differences in how each axis is positioned 
            linerJoint.position = axisDirection switch
            {
                1 => new Vector3(0.5f, -0.07f, 0f), // X Axis
                2 => new Vector3(-0.07f, 0.5f, 0f), // Y Axis
                3 => new Vector3(-0.07f, -0.02f, 0.5f), // Z Axis
                _ => throw new Exception("Invalid Axis Direction")
            };

            // Resets the rotation of the linerJoint to 0 because the rotation only needs to be on the normaliser object
            linerJoint.localRotation = Quaternion.identity;
        }
        private void ConfigLinerJointFacade(LinearDriveFacade linerJointFacade, bool isMax)
        {
            // All drive axes are the same as they are rotated by a parent object to the specific axis
            linerJointFacade.DriveAxis = Tilia.Interactions.Controllables.Driver.DriveAxis.Axis.YAxis;

            linerJointFacade.MoveToTargetValue = true;
            linerJointFacade.TargetValue = isMax ? 1f : 0f;
            linerJointFacade.SetStepRangeMaximum(100f);
        }
        public void LinkScalingEvent(LinearDriveFacade linerJointFacade, int axisDirection, bool isMax)
        {
            // Connects the linerJoint to the corresponding min/max axis
            UnityAction<float> setScaleAction = (axisDirection, isMax) switch
            {
                (1, false) => SetScaleBuilder(x => xDimension.minScale = x),
                (1, true) => SetScaleBuilder(x => xDimension.maxScale = x),
                (2, false) => SetScaleBuilder(x => yDimension.minScale = x),
                (2, true) => SetScaleBuilder(x => yDimension.maxScale = x),
                (3, false) => SetScaleBuilder(x => zDimension.minScale = x),
                (3, true) => SetScaleBuilder(x => zDimension.maxScale = x),
                _ => throw new Exception("Invalid Axis Direction")
            };
            linerJointFacade.NormalizedValueChanged.AddListener(setScaleAction);
            
        }
       
        /// <summary>
        /// /added new function
        /// </summary>
        /// <param name="linerJointFacade"></param>
        /// <param name="updateX"></param>
        public void DataScalingEventPartition0(LinearDriveFacade linerJointFacade, float updateX, float updateY, VRVisualisation Vis)
        {
            if (updateX > 0.0f &&  updateX < 0.31f) 
            {
                xDimension.minScale = 0.0f;
                xDimension.maxScale = -1.61f * updateX + 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateX > 0.31f && updateX < 0.6f)
            {
                xDimension.minScale = 0.0f;
                xDimension.maxScale = -0.59f * updateX + 0.68f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateX > 0.6f && updateX < 0.89f)
            {
                xDimension.minScale = 0.0f;
                xDimension.maxScale = -0.28f * updateX + 0.5f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }

            //Y 0
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 2.17f * updateY;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.68f * updateY + 0.344f;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }

        }
        public void DataScalingEventPartition01(LinearDriveFacade linerJointFacade, float updateX, float updateY, VRVisualisation Vis)
        {
            //Debug.Log("updateX:" + updateX);
            if (updateX > 0.01f && updateX < 0.31f)
            {
                xDimension.minScale = -1.61f * updateX + 1.0f;
                xDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if(updateX > 0.31f && updateX < 0.6f)
            {
                xDimension.minScale = -0.59f * updateX + 0.68f;
                xDimension.maxScale = -1.14f * updateX + 1.35f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateX > 0.6f && updateX < 0.89f)
            {
                xDimension.minScale = -0.28f * updateX + 0.5f;
                xDimension.maxScale = -0.59f * updateX + 1.02f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }

            //Y 01
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 2.17f * updateY;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.68f * updateY + 0.344f;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        

        public void DataScalingEventPartition02(LinearDriveFacade linerJointFacade, float updateX, float updateY, VRVisualisation Vis)
        {
            if (updateX > 0.31f && updateX < 0.6f)
            {
                xDimension.minScale = -1.14f * updateX + 1.35f;
                xDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateX > 0.6f && updateX < 0.89f)
            {
                xDimension.minScale = -0.59f * updateX + 1.02f;
                xDimension.maxScale = -0.86f * updateX + 1.52f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }

            //Y 02
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 2.17f * updateY;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.68f * updateY + 0.344f;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartition03(LinearDriveFacade linerJointFacade, float updateX, float updateY, VRVisualisation Vis)
        {
            if (updateX > 0.6f && updateX < 0.89f)
            {
                xDimension.minScale = -0.86f * updateX + 1.52f;
                xDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }

            //Y 03
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 2.17f * updateY;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.68f * updateY + 0.344f;
                yDimension.maxScale = 1.0f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }


        //Y 1 11 12 13
        public void DataScalingEventPartitionY1(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            //Debug.Log("real updateY" + updateY);
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 2.17f * updateY;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 1.32f * updateY - 0.304f;
                yDimension.maxScale = 0.68f * updateY + 0.344f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY11(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 2.17f * updateY;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 1.32f * updateY - 0.304f;
                yDimension.maxScale = 0.68f * updateY + 0.344f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY12(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 2.17f * updateY;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 1.32f * updateY - 0.304f;
                yDimension.maxScale = 0.68f * updateY + 0.344f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY13(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.01f && updateY < 0.23f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 2.17f * updateY;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
            else if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 1.32f * updateY - 0.304f;
                yDimension.maxScale = 0.68f * updateY + 0.344f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }

        //Y 2 21 22 23
        public void DataScalingEventPartitionY2(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 1.32f * updateY - 0.304f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY21(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 1.32f * updateY - 0.304f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY22(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 1.32f * updateY - 0.304f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }
        public void DataScalingEventPartitionY23(LinearDriveFacade linerJointFacade, float updateY, VRVisualisation Vis)
        {
            if (updateY > 0.23f && updateY < 0.48f)
            {
                yDimension.minScale = 0.0f;
                yDimension.maxScale = 1.32f * updateY - 0.304f;
                Vis.updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            }
        }


        public UnityEngine.Events.UnityAction<float> SetScaleBuilder(Action<float> setScale)
        {
            UnityEngine.Events.UnityAction<float> action = (float scale) =>
            {
                //Debug.Log("test:" + AbstractVisualisation.PropertyType.Scaling);
                updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
                setScale(scale);
                updateViewProperties(AbstractVisualisation.PropertyType.Scaling);
            };
            return action;
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Wraps the current game object in another. Used to convert visualisations into interactables.
        /// </summary>
        /// <param name="toWrapIn">The GameObject to wrap the current object in</param>
        /// <param name="suffix">The suffix name of the wrapping object</param>
        /// <returns>The wrapped GameObject</returns>
        public GameObject WrapIn(GameObject toWrapIn, string suffix) => WrapObject(gameObject, toWrapIn, suffix);

        /// <summary>
        /// Wraps a GameObject in another GameObject with a name suffix. Used to convert visualisations into interactables.
        /// </summary>
        private static GameObject WrapObject(GameObject toBeWrapped, GameObject toWrapIn, string suffix)
        {
            toWrapIn.name = toBeWrapped.name + " " + suffix;

            toWrapIn.transform.SetParent(toBeWrapped.transform.parent, false);
            toWrapIn.transform.localPosition = toBeWrapped.transform.localPosition;
            toWrapIn.transform.localRotation = toBeWrapped.transform.localRotation;
            toWrapIn.transform.localScale = toBeWrapped.transform.localScale;

            Transform meshContainer = RecursiveFindChild(toWrapIn.transform, "MeshContainer");
            foreach (Transform child in meshContainer)
                child.gameObject.SetActive(false);

            int siblingIndex = toBeWrapped.transform.GetSiblingIndex();

            toBeWrapped.transform.SetParent(meshContainer, false);
            toBeWrapped.transform.localPosition = Vector3.zero;
            toBeWrapped.transform.localRotation = Quaternion.identity;
            toBeWrapped.transform.localScale = Vector3.one;

            toWrapIn.transform.SetSiblingIndex(siblingIndex);

            return toWrapIn;
        }
        private static Transform RecursiveFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName) return child;
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null) return found;
                }
            }
            return null;
        }
        #endregion
    }
}