using Assets.Scripts.MonoBehaviors;
using Assets.Scripts.MonoBehaviors.Dynamic;
//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.db
{
    [Serializable]
    public class Log
    {
        //[PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int UserGroup { get; set; }
        public VisType Vis { get; set; }
        public ThematicType SubVis { get; set; }

        public long Time { get; set; }
        
        public float Progress { get; set; }

        public void SetCameraInfo(Transform trans)
        {
            this.CameraPositionX = trans.position.x;
            this.CameraPositionY = trans.position.y;
            this.CameraPositionZ = trans.position.z;

            this.CameraRotateX = trans.rotation.x;
            this.CameraRotateY = trans.rotation.y;
            this.CameraRotateZ = trans.rotation.z;
            this.CameraRotateW = trans.rotation.w;
        }
        public float CameraPositionX { get; set; }
        public float CameraPositionY { get; set; }
        public float CameraPositionZ { get; set; }

        public float CameraRotateX { get; set; }
        public float CameraRotateY { get; set; }
        public float CameraRotateZ { get; set; }
        public float CameraRotateW { get; set; }


        public void SetControllerInfo(Transform trans)
        {
            this.ControllerPositionX = trans.position.x;
            this.ControllerPositionY = trans.position.y;
            this.ControllerPositionZ = trans.position.z;

            this.ControllerRotateX = trans.rotation.x;
            this.ControllerRotateY = trans.rotation.y;
            this.ControllerRotateZ = trans.rotation.z;
            this.ControllerRotateW = trans.rotation.w;
        }
        public float ControllerPositionX { get; set; }
        public float ControllerPositionY { get; set; }
        public float ControllerPositionZ { get; set; }

        public float ControllerRotateX { get; set; }
        public float ControllerRotateY { get; set; }
        public float ControllerRotateZ { get; set; }
        public float ControllerRotateW { get; set; }
        

        public void SetMapInfo(Transform trans)
        {
            this.MapPositionX = trans.position.x;
            this.MapPositionY = trans.position.y;
            this.MapPositionZ = trans.position.z;

            this.MapRotateX = trans.rotation.x;
            this.MapRotateY = trans.rotation.y;
            this.MapRotateZ = trans.rotation.z;
            this.MapRotateW = trans.rotation.w;
        }
        public float MapPositionX { get; set; }
        public float MapPositionY { get; set; }
        public float MapPositionZ { get; set; }

        public float MapRotateX { get; set; }
        public float MapRotateY { get; set; }
        public float MapRotateZ { get; set; }
        public float MapRotateW { get; set; }
    }
}
