using Assets.Scripts.Model.db;
using Assets.Scripts.MonoBehaviors.Dynamic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
//using VRTK;

namespace Assets.Scripts.MonoBehaviors.Exp
{
    public class ExpController : MonoBehaviour
    {
        public VisRenderer VisRender;
        public CanvasRenderer CanvasRender;

        //public VRTK.VRTK_ControllerEvents ControllerEvents;

        public User CurrentUser { get; private set; }

        public Question CurrentQuestion;
        private Answer _currentAnswer;
        private int CurrentActionIndex = -1;

        public VisType CurrentVisType;

        private List<Action> _actions = new List<Action>();

        private void ShowInfo()
        {
            VisRender.HideVis();
        }

        private void ShowVis(Question q, VisType vis)
        {
            this.CurrentQuestion = q;
            this.CurrentVisType = vis;
            this._currentAnswer.Vis = vis;
            this._currentAnswer.QuestionId = q.Id;

            CanvasRender.HideAll();
            VisRender.ShowVis(q.Country, vis);
        }

        private void Start()
        {
            //ControllerEvents.TriggerClicked += ControllerEvents_TriggerClicked;

            this.CurrentUser = new User();
            var userInfos = File.ReadAllLines("./Data/Exp/user.txt");
            this.CurrentUser.Name = userInfos[0];
            this.CurrentUser.Group = Convert.ToInt32(userInfos[1]);

            //var exist = DB.Conn.Table<User>().Where(u => u.Name == this.CurrentUser.Name);
            //if (!exist.Any())
            //{
            //    DB.Conn.Insert(this.CurrentUser);
            //    this.CurrentUser = (
            //        DB.Conn.Table<User>().Where(
            //            u => u.Name == this.CurrentUser.Name
            //        )
            //    ).First();
            //}
            //else
            //{
            //    this.CurrentUser = exist.First();
            //    this.CurrentUser.Group = Convert.ToInt32(userInfos[1]);
            //    DB.Conn.Update(this.CurrentUser);
            //}
            this._currentAnswer = new Answer { UserId = this.CurrentUser.Id };

            VisType[] visOrders = null;
            switch (this.CurrentUser.Group)
            {
                case 1:
                    visOrders = new VisType[]
                    {
                        VisType.Magic,
                        VisType.ButtonChange,
                        VisType.SideBySide
                    };
                    break;
                case 2:
                    visOrders = new VisType[]
                    {
                        VisType.SideBySide,
                        VisType.Magic,
                        VisType.ButtonChange
                    };
                    break;
                case 3:
                    visOrders = new VisType[]
                    {
                        VisType.ButtonChange,
                        VisType.SideBySide,
                        VisType.Magic
                    };
                    break;
                default:
                    Debug.LogError("No such user group!");
                    break;
            }

            _actions.Add(new Action(() =>
            {
                this.ShowInfo();
                var str = "Welcome to our VR study!\n";
                str += "You are going to view different combinations of thematic maps, and then answer some questions.\n";
                str += "We have <b>one</b> type of task and <b>three</b> types of maps.\n\n";
                str += "First, let us get familiar with the first combination of maps.\n";
                str += "Take as long as you like.";

                CanvasRender.ShowInfo(
                    str,
                    this.Next
                );
            }));

            var t = TaskType.Region;
            for (var j = 0; j < visOrders.Length; j++)
            {
                var v = visOrders[j];
                var groupId = j + 1;

                //var preQuestions = DB.Conn.Table<Question>().Where(
                //        q => q.IsTraining == TrainingType.Pre && q.Group == groupId && q.Country == GeoName.EU
                //    ).OrderBy(q => q.Order);
                //var trainingQuestions = DB.Conn.Table<Question>().Where(
                //    q => q.Task == t && q.IsTraining == TrainingType.Training && q.Group == groupId
                //).OrderBy(q => q.Order);
                //var realQuestions = DB.Conn.Table<Question>().Where(
                //    q => q.Task == t && q.IsTraining == TrainingType.Real && q.Group == groupId
                //).OrderBy(q => q.Order);

                //if (j == 0)
                //{
                //    _actions.Add(new Action(() =>
                //    {
                //        var thisQ = preQuestions.First();
                //        this.ShowVis(thisQ, v);
                //        this.VisRender.DrawQuestion(thisQ, false);
                //    }));
                //}
                //else
                //{
                //    _actions.Add(new Action(() =>
                //    {
                //        this.ShowInfo();
                //        var str = "Well done!\n";
                //        str += "Let us try another type of map.\n\n";
                //        str += "Again, you will have as much time as you want to get familiar with it.";

                //        CanvasRender.ShowInfo(
                //            str,
                //            this.Next
                //        );
                //    }));

                //    _actions.Add(new Action(() =>
                //    {
                //        var thisQ = preQuestions.First();
                //        this.ShowVis(thisQ, v);
                //        this.VisRender.DrawQuestion(thisQ, false);
                //    }));
                //}

                var tCount = 0;
                //foreach (var tQ in trainingQuestions)
                //{
                //    if (tCount == 0)
                //    {
                //        _actions.Add(new Action(() =>
                //        {
                //            this.ShowInfo();
                //            var str = "Now, you should be ready! ";
                //            str += "Let us start a quick <b>training</b> first.\n\n";
                //            str += "You are going to answer the question:\n";
                //            str += Question.GetTaskDescription(t);

                //            str += "\n\n";
                //            str += "The smallest value on the map is always 0;\nThe highest value is always 100.";

                //            CanvasRender.ShowInfo(
                //                str,
                //                this.Next
                //            );
                //        }));
                //    }
                //    else
                //    {
                //        _actions.Add(new Action(() =>
                //        {
                //            this.ShowInfo();
                //            var str = "Let us try another <b>training</b>.\n\n";
                //            str += "You are going to answer the <b>same</b> question:\n";
                //            str += Question.GetTaskDescription(t);

                //            str += "\n\n";
                //            str += "The smallest value on the map is always 0;\nThe highest value is always 100.";

                //            CanvasRender.ShowInfo(
                //                str,
                //                this.Next
                //            );
                //        }));
                //    }

                //    _actions.Add(new Action(() =>
                //    {
                //        this.ShowVis(tQ, v);
                //        this.VisRender.DrawQuestion(tQ);
                //        this._currentAnswer.StartTime = DateTime.Now.Ticks;
                //    }));

                //    _actions.Add(new Action(() =>
                //    {
                //        this.ShowInfo();
                //        this.CanvasRender.ShowQuestionWithSlider(tQ, this._currentAnswer, this.Next);
                //    }));

                    //_actions.Add(new Action(() =>
                    //{

                    //    var thisAnswer = DB.Conn.Table<Answer>().Last(
                    //        a => a.UserId == CurrentUser.Id &&
                    //             a.QuestionId == tQ.Id
                    //    );
                    //    var str = "";

                    //    str += "The correct answer is: ";
                    //    str += tQ.GetAnswerStr();
                    //    str += "\n";

                    //    str += "Your answer is: ";
                    //    str += thisAnswer.GetAnswerStr();
                    //    str += "\n\n";

                    //    var diff = thisAnswer.DiffWithAnswer();
                    //    if (diff <= 1)
                    //    {
                    //        str += "You are perfect!\n";
                    //    }
                    //    else if (diff <= 10)
                    //    {
                    //        str += "You are so close!\n";
                    //    }
                    //    else
                    //    {
                    //        str += "Cheer up, try to be more accurate next time!\n";
                    //    }
                    //    str += "Let's review the previous map again.";

                    //    this.ShowInfo();
                    //    CanvasRender.ShowInfo(
                    //        str,
                    //        this.Next
                    //    );
                    //}));

                //    _actions.Add(new Action(() =>
                //    {
                //        this.ShowVis(tQ, v);
                //        this.VisRender.DrawQuestion(tQ);
                //        this._currentAnswer.StartTime = DateTime.Now.Ticks;
                //    }));
                //    tCount++;
                //}

                //_actions.Add(new Action(() =>
                //{
                //    this.ShowInfo();
                //    var str = "Now it is time to start the real tasks.\n";
                //    str += $"\n\nThere will be {realQuestions.Count()} questions in the same form:\n\n";
                //    str += Question.GetTaskDescription(t);
                //    str += "\n\nPlease answer as <b>accuratly</b> and as <b>quickly</b> as you can.";

                //    CanvasRender.ShowInfo(
                //        str,
                //        this.Next
                //    );
                //}));

                //var qList = realQuestions.ToList();
                //for (var qIndex = 0; qIndex < qList.Count; qIndex++)
                //{
                //    var thisQ = qList[qIndex];
                //    _actions.Add(new Action(() =>
                //    {
                //        this.ShowVis(thisQ, v);
                //        this.VisRender.DrawQuestion(thisQ);
                //        this._currentAnswer.StartTime = DateTime.Now.Ticks;
                //        this._currentAnswer.Vis = v;
                //    }));

                //    _actions.Add(new Action(() =>
                //    {
                //        var tmpIndex = qIndex;
                //        this.ShowInfo();
                //        var thisCount = qList.IndexOf(thisQ);
                //        this.CanvasRender.ShowQuestionWithSlider(thisQ, this._currentAnswer, this.Next, ++thisCount);
                //    }));
                //}
            }

            _actions.Add(new Action(() =>
            {
                this.ShowInfo();
                var str = "Well done! Congratulations! You have finished all challenges!\n\n";
                str += "Thanks for your time.";

                CanvasRender.ShowInfo(
                    str,
                    this.Next
                );
            }));
        }

        private bool _oneClick = false;
        private long _timeForFirstClick;
        private void ControllerEvents_TriggerClicked(object sender)
        {
            if (this.VisRender.CurrentMap == null) return;
            if (!this.VisRender.IsVisShown) return;

            if (_oneClick)
            {
                if (this._currentAnswer != null) this._currentAnswer.EndTime = DateTime.Now.Ticks;
                _oneClick = false;
                this.Next();
            }
            else
            {
                _oneClick = true;
                _timeForFirstClick = DateTime.Now.Ticks;
            }
        }

        private void Next()
        {
            CurrentActionIndex++;
            CurrentActionIndex = Mathf.Min(CurrentActionIndex, this._actions.Count - 1);

            this._actions[CurrentActionIndex]();
        }

        private void Pre()
        {
            CurrentActionIndex--;
            CurrentActionIndex = Mathf.Max(CurrentActionIndex, 0);

            this._actions[CurrentActionIndex]();
        }

        private bool _isInitialed = false;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this._currentAnswer == null || this._currentAnswer.QuestionId < 0)
                {
                    this.Pre();
                    return;
                }
                else
                {
                    this._currentAnswer.EndTime = DateTime.Now.Ticks;
                    this._currentAnswer.Choice = "-1";
                    //DB.Conn.Insert(this._currentAnswer);
                    this.Pre();
                }
            }

            if ((DateTime.Now.Ticks - _timeForFirstClick) / 10000 > 350)
            {
                _oneClick = false;
            }

            if (_isInitialed || Camera.main == null) return;
            this._isInitialed = true;

            this.Next();
        }

        private void OnApplicationQuit()
        {
            //DB.Conn.Close();
        }

    }
}
