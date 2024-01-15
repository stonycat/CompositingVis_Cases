using Assets.Scripts.MonoBehaviors;
using Assets.Scripts.MonoBehaviors.Dynamic;
//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.db
{
    public class Answer
    {
        //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //[SQLite.Column("user_id")]
        public int UserId { get; set; }

        //[SQLite.Column("question_id")]
        public int QuestionId { get; set; }

        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Choice { get; set; }

        public VisType Vis { get; set; }

        public string Answer1;
        public string Answer2;

        public void CombineAnswers()
        {
            this.Choice = $"{this.Answer1};;{this.Answer2}";
        }

        public string GetAnswerStr()
        {
            //var thisQ = DB.Conn.Table<Question>().First(q => q.Id == this.QuestionId);
            //if(thisQ.Task == TaskType.Single)
            //{
            //    var results = this.Choice.Split(new string[] { ";;" }, StringSplitOptions.None);
            //    //if (results[0].StartsWith("B"))
            //    //    return $"<b><#377eb8>Blue</color></b> is greater than <b><color=#4daf4a>Green</color></b> by {results[1]}";
            //    //else
            //    //    return $"<b><color=#4daf4a>Green</color></b> is greater than <b><#377eb8>Blue</color></b> by {results[1]}";
            //    return $"{results[1]}";
            //}
            //else
            //{
            //    return $"{this.Choice}";
            //}
            return "dummy string lalala";
        }

        public int DiffWithAnswer()
        {
            //var thisQ = DB.Conn.Table<Question>().First(q => q.Id == this.QuestionId);
            //if (thisQ.Task == TaskType.Single)
            //{
            //    var answers = thisQ.Answer.Split(new string[] { ";;" }, StringSplitOptions.None);
            //    var answerNum = (int)(float.Parse(answers[1]));

            //    var results = this.Choice.Split(new string[] { ";;" }, StringSplitOptions.None);
            //    var resultNum = (int)(float.Parse(results[1]));

            //    return Math.Abs(answerNum - resultNum);
            //}
            //else
            //{
            //    var answerNum = (int)(float.Parse(thisQ.Answer));
            //    var resultNum = (int)(float.Parse(this.Choice));
            //    return Math.Abs(answerNum - resultNum);
            //}
            return 10086;
        }
    }
}
