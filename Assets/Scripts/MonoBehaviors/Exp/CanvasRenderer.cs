using Assets.Scripts.Model.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.MonoBehaviors.Exp
{
    public class CanvasRenderer : MonoBehaviour
    {
        public GameObject _infoObj;
        public TextMeshProUGUI _infoText;
        public Button _infoButton;
        
        public GameObject _sliderObj;
        public TextMeshProUGUI _sliderText;
        public Slider _slider;
        public Button _sliderButton;
        public TextMeshProUGUI _sliderNumberText;

        private void ShowUI()
        {
            this.gameObject.SetActive(true);
            this.gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
            this.gameObject.transform.LookAt(Camera.main.transform.position);
            this.gameObject.transform.Rotate(new Vector3(0, 1, 0), 180);
        }

        public void HideAll()
        {
            this.gameObject.SetActive(false);
        }

        public void ShowInfo(string info, Action next)
        {
            this._infoText.text = info;
            this._infoButton.onClick.RemoveAllListeners();
            this._infoButton.onClick.AddListener(new UnityAction(() => {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    return;
                }
                next();
            }));

            this._infoObj.SetActive(true);
            this._sliderObj.SetActive(false);
            this.ShowUI();
        }

        public void AddSliderValue()
        {
            this._slider.value++;
        }

        public void DeductSliderValue()
        {
            this._slider.value--;
        }

        public void ShowQuestionWithSlider(Question question, Answer currentAnswer, Action next, int index = -1)
        {
            var questionStr = question.Title;
            if (index != -1)
            {
                questionStr = index.ToString() + ". " + questionStr;
            }
            this._sliderText.text = questionStr;

            this._slider.value = 0;
            this._sliderNumberText.text = "0";
            this._slider.onValueChanged.RemoveAllListeners();
            this._slider.onValueChanged.AddListener(new UnityAction<float>((x) =>
            {
                this._sliderNumberText.text = x.ToString();
            }));

            this._sliderButton.onClick.RemoveAllListeners();
            this._sliderButton.onClick.AddListener(new UnityAction(() =>
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    return;
                }

                currentAnswer.QuestionId = question.Id;

                if (question.Task == TaskType.Single)
                {
                    currentAnswer.Answer1 = "Blue";
                    currentAnswer.Answer2 = this._slider.value.ToString();
                    currentAnswer.CombineAnswers();
                }
                else if (question.Task == TaskType.Region)
                {
                    currentAnswer.Choice = this._slider.value.ToString();
                }

                //DB.Conn.Insert(currentAnswer);
                next();
            }));

            this._infoObj.SetActive(false);
            this._sliderObj.SetActive(true);
            this.ShowUI();
        }
    }
}
