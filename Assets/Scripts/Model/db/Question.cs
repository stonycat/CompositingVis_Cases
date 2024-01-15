using Assets.Scripts.MonoBehaviors;
//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.db
{
    public enum TaskType
    {
        Single = 1,
        Region = 2
    }

    public enum TrainingType
    {
        Pre = 1,
        Training = 2,
        Real = 3
    }

    public enum AnswerRangeType
    {
        S = 1,
        M = 2,
        L = 3
    }

    public class Question
    {
        //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public TaskType Task { get; set; }
        public int Group { get; set; }
        public GeoName Country { get; set; }

        public TrainingType IsTraining { get; set; }

        public int Order { get; set; }
        public int Repetition { get; set; }

        public string Answer { get; set; }
        public AnswerRangeType AnswerRange { get; set; }

        public string Targets { get; set; }

        //public static string GetTaskTitle(TaskType t)
        //{
        //    switch (t)
        //    {
        //        case TaskType.Single:
        //            var optionStr = "For the two areas colored <b><#377eb8>Blue</color></b> and <b><color=#4daf4a>Green</color></b>, which one has a larger population density?";
        //            var sliderStr = "Please estimate the <b>difference</b> between the two colored areas.";
        //            return $"{optionStr};;{sliderStr}";
        //        case TaskType.Region:
        //            return "Please estimate the population density of the whole colored region.";
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        public string GetAnswerStr()
        {
            if (this.Task == TaskType.Single)
            {
                var targets = this.Targets.Split(new string[] { ";;" }, StringSplitOptions.None);
                var answers = this.Answer.Split(new string[] { ";;" }, StringSplitOptions.None);
                var value = (int)(float.Parse(answers[1]));

                //if (answers[0].StartsWith(targets[0]))
                //    return $"<b><#377eb8>Blue</color></b> is greater than <b><color=#4daf4a>Green</color></b> by {value}";
                //else
                //    return $"<b><color=#4daf4a>Green</color></b> is greater than <b><#377eb8>Blue</color></b> by {value}";
                return $"{value}";
            }
            else
            {
                return $"{(int)float.Parse(this.Answer)}";
            }
        }

        public static string GetTaskDescription(TaskType t)
        {
            switch (t)
            {
                case TaskType.Single:
                    return "Please estimate the <b>difference</b> between the two marked areas.";
                case TaskType.Region:
                    return "Please estimate the population density of the whole marked region.";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string Title
        {
            get
            {
                return GetTaskDescription(this.Task);
            }
        }

        private string CountryStr
        {
            get
            {
                switch (this.Country)
                {
                    case GeoName.UK:
                        return "uk";
                    case GeoName.US:
                        return "us";
                    case GeoName.EU:
                        return "eu";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string IsTrainingStr
        {
            get
            {
                switch (this.IsTraining)
                {
                    case TrainingType.Pre:
                        return "pre-training";
                    case TrainingType.Training:
                        return "training";
                    case TrainingType.Real:
                        return "real";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string TaskStr
        {
            get
            {
                switch (this.Task)
                {
                    case TaskType.Single:
                        return "Single";
                    case TaskType.Region:
                        return "Region";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private string AnswerRangeStr
        {
            get
            {
                switch(this.AnswerRange)
                {
                    case AnswerRangeType.S:
                        return "S";
                    case AnswerRangeType.M:
                        return "M";
                    case AnswerRangeType.L:
                        return "L";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string FileName =>
            $"{this.TaskStr}_{this.CountryStr}_{this.IsTrainingStr}_{this.AnswerRangeStr}_{this.Group}_{this.Repetition}";
    }
}
