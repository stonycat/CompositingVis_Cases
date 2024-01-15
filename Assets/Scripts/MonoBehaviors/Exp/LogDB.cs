using Assets.Scripts.Model.db;
using Assets.Scripts.MonoBehaviors.Dynamic;
using CsvHelper;
//using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MonoBehaviors.Exp
{
    class LogDB : MonoBehaviour
    {
        private float _nextUpdate = 0f;
        private float _updateRate = 10f;

        public GameObject CurrentCamera;
        public GameObject Controller;
        public ExpController Sys;

        //private SQLiteConnection _db;
        private List<Log> _logs;
        private Log _log;

        private void OnApplicationQuit()
        {
            //_db.InsertAll(this._logs);
            //_db.Close();
        }

        List<long> ids = new List<long>();

        private void UpDateDB()
        {
            //var logs = _db.Table<Log>().Where(u => u.Vis == VisType.SideBySide && u.Progress < 0).OrderBy(i => i.Id);
            //var count = logs.Count() / 3;

            //Debug.Log(count);

            //Log clog = null;
            //Log pLog = null;
            //Log bLog = null;

            //foreach (var l in logs)
            //{
            //    if (clog == null)
            //        clog = l;
            //    else if (pLog == null)
            //        pLog = l;
            //    else if (bLog == null)
            //    {
            //        bLog = l;
                    
            //        var cameraP = new Vector3(clog.CameraPositionX, clog.CameraPositionY, clog.CameraPositionZ);
            //        var cameraR = new Quaternion(clog.CameraRotateX, clog.CameraRotateY, clog.CameraRotateZ, clog.CameraRotateW);
            //        var cameraF = cameraR * Vector3.forward;

            //        var cP = new Vector3(clog.MapPositionX, clog.MapPositionY, clog.MapPositionZ);
            //        var pP = new Vector3(pLog.MapPositionX, pLog.MapPositionY, pLog.MapPositionZ);
            //        var bP = new Vector3(bLog.MapPositionX, bLog.MapPositionY, bLog.MapPositionZ);

            //        var cPP = Vector3.Project(cP - cameraP, cameraF);
            //        var pPP = Vector3.Project(pP - cameraP, cameraF);
            //        var bPP = Vector3.Project(bP - cameraP, cameraF);

            //        var cDis = Vector3.Distance(cP, cPP);
            //        var pDis = Vector3.Distance(pP, pPP);
            //        var bDis = Vector3.Distance(bP, bPP);

            //        if (cDis < pDis && cDis < bDis)
            //        {
            //            clog.Progress = 0;
            //            pLog.Progress = 0;
            //            bLog.Progress = 0;
            //        }
            //        else if (pDis < cDis && pDis < bDis)
            //        {
            //            clog.Progress = (45.0f + 75.0f) / 2.0f / 90.0f;
            //            pLog.Progress = (45.0f + 75.0f) / 2.0f / 90.0f;
            //            bLog.Progress = (45.0f + 75.0f) / 2.0f / 90.0f;
            //        }
            //        else
            //        {
            //            clog.Progress = 1;
            //            pLog.Progress = 1;
            //            bLog.Progress = 1;
            //        }

            //        _db.RunInTransaction(() =>
            //        {
            //            _db.Update(clog);
            //            _db.Update(pLog);
            //            _db.Update(bLog);
            //        });

            //        clog = null;
            //        pLog = null;
            //        bLog = null;
            //    }
            //}
        }

        private void Start()
        {
            //_nextUpdate = Time.time;
            //_db = new SQLiteConnection("./Data/Exp/log.db");
            //_db.CreateTable<Log>();

            //this._logs = new List<Log>();

            //UpDateDB();

        }

        private void Update()
        {
            //if (Time.time > this._nextUpdate)
            //{
            //    _log = new Log();
            //    this._log.Id = -1;

            //    this._nextUpdate += 1f / this._updateRate;
            //    this._log.Time = DateTime.Now.Ticks;

            //    var thisMap = this.Sys.VisRender.CurrentMap;
            //    if (thisMap == null || thisMap.gameObject.activeSelf == false)
            //    {
            //        return;
            //    }

            //    this._log.UserId = this.Sys.CurrentUser == null ? -100 : this.Sys.CurrentUser.Id;
            //    this._log.UserGroup = this.Sys.CurrentUser == null ? -100 : this.Sys.CurrentUser.Group;
            //    this._log.QuestionId = this.Sys.CurrentQuestion == null ? -100 : this.Sys.CurrentQuestion.Id;

            //    this._log.Vis = this.Sys.CurrentVisType;

            //    if (this.CurrentCamera != null)
            //        this._log.SetCameraInfo(this.CurrentCamera.transform);
            //    if (this.Controller != null)
            //        this._log.SetControllerInfo(this.Controller.transform);

            //    switch(this._log.Vis)
            //    {
            //        case Dynamic.VisType.Magic:
            //            this._log.SubVis = ThematicType.PropotionalSymbols;
            //            this._log.SetMapInfo(this.Sys.VisRender.CurrentMap.MagicMap.MapObj.transform);
            //            this._log.Progress = this.Sys.VisRender.CurrentMap.LookAtProgress;

            //            this._logs.Add(this._log);
            //            break;
            //        case Dynamic.VisType.ButtonChange:
            //            this._log.SubVis = ThematicType.PropotionalSymbols;
            //            this._log.SetMapInfo(this.Sys.VisRender.CurrentMap.ButtonChange.MapObj.transform);
            //            this._log.Progress = this.Sys.VisRender.CurrentMap.LookAtProgress;

            //            this._logs.Add(this._log);
            //            break;
            //        case Dynamic.VisType.SideBySide:
            //            this._log.SubVis = ThematicType.Choropleth;
            //            this._log.SetMapInfo(this.Sys.VisRender.CurrentMap.SideBySideChoropleth.MapObj.transform);
            //            this._log.Progress = this.Sys.VisRender.CurrentMap.LookAtProgress;
            //            this._logs.Add(this._log);

            //            this._log = Settings.DeepClone(this._log);
            //            this._log.SubVis = ThematicType.Prism;
            //            this._log.SetMapInfo(this.Sys.VisRender.CurrentMap.SideBySidePrism.MapObj.transform);
            //            this._log.Progress = this.Sys.VisRender.CurrentMap.LookAtProgress;
            //            this._logs.Add(this._log);

            //            this._log = Settings.DeepClone(this._log);
            //            this._log.SubVis = ThematicType.Bar;
            //            this._log.SetMapInfo(this.Sys.VisRender.CurrentMap.SideBySideBars.MapObj.transform);
            //            this._log.Progress = this.Sys.VisRender.CurrentMap.LookAtProgress;
            //            this._logs.Add(this._log);
            //            break;
            //    }


            //    if (this._logs.Count >= 100)
            //    {
            //        this._db.InsertAll(this._logs);
            //        this._logs.Clear();
            //    }
            //}
        }

    }
}
