using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class NumericData
    {
        public Dictionary<string, List<float>> Data;
        private Dictionary<string, int> propIndexMap;
        private List<Dictionary<string, int>> prop2Rank;

        private List<float> minValue;
        private List<float> maxValue;

        public string PropName = null;

        public NumericData(string filePath)
        {
            this.Data = new Dictionary<string, List<float>>();
            this.propIndexMap = new Dictionary<string, int>();

            this.minValue = new List<float>();
            this.maxValue = new List<float>();

            using (TextReader textReader = new StreamReader(filePath))
            {
                var header = textReader.ReadLine();
                var headers = header.Split(',');

                var properties = headers.Skip(1).ToArray();
                for (var i = 0; i < properties.Length; i++)
                {
                    Debug.Log("Add " + properties[i].Replace("\"", "") + " to propIndexMap");
                    this.propIndexMap.Add(properties[i].Replace("\"", ""), i);

                    this.minValue.Add(float.MaxValue);
                    this.maxValue.Add(float.MinValue);
                }

                var csv = new CsvReader(textReader, System.Globalization.CultureInfo.CreateSpecificCulture("en"));
                while(csv.Read())
                {
                    var thisName = csv.GetField<string>(0).Replace("\"", "");
                    if (this.Data.ContainsKey(thisName))
                    {
                        Debug.LogWarning($"{thisName} already exists in NumericData.");
                    }
                    var thisList = new List<float>();
                    for(var i = 0; i < properties.Length; i++)
                    {
                        var thisV = csv.GetField<float>(i + 1);
                        thisList.Add(thisV);

                        this.minValue[i] = Mathf.Min(this.minValue[i], thisV);
                        this.maxValue[i] = Mathf.Max(this.maxValue[i], thisV);
                    }
                    this.Data[thisName] = thisList;
                }
            }

            this.prop2Rank = new List<Dictionary<string, int>>();
            for (var i = 0; i < this.propIndexMap.Count; i++)
            {
                var items = from pair in this.Data
                            orderby pair.Value[i] descending
                            select pair;
                var rank = new Dictionary<string, int>();
                var index = 0;
                foreach (var pair in items)
                {
                    rank.Add(pair.Key, index);
                    index++;
                }

                this.prop2Rank.Add(rank);
            }
        }

        public float GetValue(string name)
        {
            if (this.PropName != null)
                return this.GetValue(name, this.PropName);
            return float.NaN;
        }

        public float GetValue(string name, string propName)
        {
            if(!this.Data.ContainsKey(name))
                Debug.LogWarning($"{name} not in NumericData.");
            if (!this.propIndexMap.ContainsKey(propName))
                Debug.LogWarning($"{propName} is not a valid property.");

            return (this.Data[name])[this.propIndexMap[propName]];
        }

        public int GetRank(string name)
        {
            if (this.PropName != null)
                return this.GetRank(name, this.PropName);
            return -1;
        }

        public int GetRank(string name, string propName)
        {
            if (!this.Data.ContainsKey(name))
                Debug.LogWarning($"{name} not in NumericData.");
            if (!this.propIndexMap.ContainsKey(propName))
                Debug.LogWarning($"{propName} is not a valid property.");

            return (this.prop2Rank[this.propIndexMap[propName]])[name];
        }

        public float GetMin()
        {
            if (this.PropName != null)
                return this.GetMin(this.PropName);
            return float.NaN;
        }

        public float GetMin(string propName)
        {
            if (!this.propIndexMap.ContainsKey(propName))
                Debug.LogWarning($"{propName} is not a valid property.");
            return (this.minValue)[this.propIndexMap[propName]];
        }

        public float GetMax()
        {
            if (this.PropName != null)
                return this.GetMax(this.PropName);
            return float.NaN;
        }


        public float GetMax(string propName)
        {
            if (!this.propIndexMap.ContainsKey(propName))
                Debug.LogWarning($"{propName} is not a valid property.");

            return (this.maxValue)[this.propIndexMap[propName]];
        }

    }
}
