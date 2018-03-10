using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FeedScreen.Experiment.Missions.Broadcasts.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Survey
{
    public class GenericSurvey : MonoBehaviour
    {
        private AnswerButton _backButton;
        private List<GameObject> _go;
        private AnswerButton _nextButton;

        private int _questionIndex;

        private bool _surveyLock;
        private int _surveyNumber;
        private List<QuestionDetails> _surveyQuestionList;

        public GameObject FreeResponsePrefab;
        public GameObject MessegePrefab;
        public GameObject MultiplePrefab;
        public GameObject NumericPrefab;
        public GameObject ScalarPrefab;
        public GameObject ScalePrefab;

        public IEnumerator StartUp(int tempSurveyNumber)
        {
            Debug.Log("Starting Survey");

            _surveyNumber = tempSurveyNumber;
            _go = new List<GameObject>();
            var downloadManager = gameObject.AddComponent<LoadSurveyFromWeb>();

            /* for can
            if (transform.childCount > 0)
                foreach (var child in _go)
                    DestroyImmediate(child.gameObject);
            */


            while (!downloadManager.Loading)
                yield return new WaitForEndOfFrame();
            Debug.Log(downloadManager.Loading + " is 1st state of loading");

            StartCoroutine(downloadManager.LoadSurveyEnumerator(_surveyNumber));

            _nextButton = gameObject.transform.parent.GetChild(1).GetChild(0)
                .GetComponent<AnswerButton>();
            _nextButton.BehaviorOfButton = 1;
            _backButton = gameObject.transform.parent.GetChild(1).GetChild(1)
                .GetComponent<AnswerButton>();
            _backButton.BehaviorOfButton = -1;
            _nextButton.BehaviorOfButton = 1;

            _questionIndex = -1;

            while (downloadManager.Loading)
                yield return new WaitForEndOfFrame();
            Debug.Log(downloadManager.Loading + " is 2nd state of loading");

            _surveyQuestionList = downloadManager.SurveyList;
            if (_surveyQuestionList.Count == 0)
                GoToNextScene();
            //Debug.Log(_surveyQuestionList);
            //Debug.Log(_surveyQuestionList.Count);
            //Debug.Log(HasNextQuestion());
            _backButton.gameObject.SetActive(true);
            _nextButton.gameObject.SetActive(true);
            NextQuestion();

            _surveyLock = false;

            //go.transform.position = new Vector2(x: Screen.width/2, y: Screen.height/2);
            //_question = GameObject.Find("Question").GetComponent<Text>();
            //Load(path);
        }

        private void Start()
        {
            _surveyLock = false;
            Debug.Log("GenericSurvey Start");
            ActivateSurveyWithNumber.CallListener();
        }

        private void OnEnable()
        {
            EventManager.Load += OnLoad;
            _surveyLock = false;
        }

        private void OnLoad(object sender, IntEventArgs e)
        {
            //Debug.Log(e.intField);
            StartCoroutine(StartUp(e.intField));
        }

        private void OnDisable()
        {
            EventManager.Load -= OnLoad;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool HasNextQuestion()
        {
            return _questionIndex + 1 < _surveyQuestionList.Count;
        }

        public bool HasPreviousQuestion()
        {
            return _questionIndex - 1 > -1;
        }

        /**
    private list<list<string>> _questions;
    private text _question;
    private int _questionindex;
    //avoids double-clicks failing code
    private bool _malloc = true;

    // Use this for initialization

*/


        /// <summary>
        ///     Sets up a question with a given prefab />
        /// </summary>
        /// <param name="tempPrefab"></param>
        /// <param name="question"></param>
        /// <returns>returns an instantiated GameObject</returns>
        private static GameObject InstatiatePrefabAndPopulateAnswer(
            GameObject tempPrefab, string question)
        {
            var answerSelection = Instantiate(tempPrefab);
            answerSelection.transform.GetChild(0).GetChild(0)
                .GetComponent<Text>().text = question;
            return answerSelection;
        }

        /// <summary>
        /// </summary>
        /// <param name="answerSelection"></param>
        /// <param name="answers"></param>
        private static void PopulateAnswers(Component answerSelection,
            IList<string> answers)
        {
            var firstAnswer = answerSelection.gameObject;
            firstAnswer.SetActive(false);
            //Debug.Log(firstAnswer.name + "is firstAnswer");
            //Debug.Log(answerSelection.name + "is firstAnswer");
            foreach (var answer in answers)
            {
                var newAnswer = answer;
                var instance = Instantiate(firstAnswer);
                instance.SetActive(true);
                instance.transform.SetParent(firstAnswer.transform.parent);
                if (newAnswer[0] == '@')
                {
                    newAnswer = answer.Substring(1);
                    instance.GetComponent<InsertInputFieldInParentsParent>()
                        .SpawnInsertFieldOnTrue = true;
                }

                instance.transform.GetChild(0).GetComponent<Text>().text =
                    newAnswer;
            }
        }


        /// <summary>
        ///     Sets up a question with an <see cref="InputField" />
        /// </summary>
        /// <param name="questionDetails">A list consisting of a question and the rest is answers</param>
        /// <returns>returns an instantiated GameObject</returns>
        private GameObject FreeResponseSetUp(QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(FreeResponsePrefab,
                    questionDetails.QuestionString);

            return tempPrefab;
        }

        /// <summary>
        ///     Sets up a question prefab with a <see cref="Dropdown" />
        /// </summary>
        /// <param name="questionDetails">A list consisting of a question and the rest is answers</param>
        /// <returns>returns an instantiated GameObject</returns>
        /// <remarks>ToList() is used because Unity's <see cref="Dropdown" /> does not accept Itterables.</remarks>
        private GameObject MultipleSetup(QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(MultiplePrefab,
                    questionDetails.QuestionString);
            var answerSelection = tempPrefab.transform.GetChild(1).GetChild(0)
                .GetChild(1);
            //Debug.Log(answerSelection.name + " is in MultipleSetup");
            PopulateAnswers(answerSelection, questionDetails.OfferedAnswerList);
            return tempPrefab;
        }

        /// <summary>
        ///     Sets up a question prefab with a several <see cref="Button" />
        /// </summary>
        /// <param name="questionDetails">A list consisting of a question and the rest is answers</param>
        /// <returns>returns an instantiated GameObject</returns>
        /// <remarks>ToList() is used because Unity's <see cref="Dropdown" /> does not accept Itterables.</remarks>
        private GameObject ScalarSetup(QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(ScalarPrefab,
                    questionDetails.QuestionString);
            var answerSelection = tempPrefab.transform.GetChild(1).GetChild(0)
                .GetChild(1);
            //Debug.Log(answerSelection.name + " is in ScalarSetup");
            PopulateAnswers(answerSelection, questionDetails.OfferedAnswerList);
            return tempPrefab;
        }

        /// <summary>
        ///     Sets up a question prefab with a several <see cref="Button" />
        /// </summary>
        /// <param name="questionDetails">A list consisting of a question and the rest is answers</param>
        /// <returns>returns an instantiated GameObject</returns>
        /// <remarks>ToList() is used because Unity's <see cref="Dropdown" /> does not accept Itterables.</remarks>
        private GameObject NumericSetup(QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(NumericPrefab,
                    questionDetails.QuestionString);
            var answerSelection = tempPrefab.transform.GetChild(1).GetChild(0)
                .GetChild(0);
            //Debug.Log(answerSelection.name);
            var answerList = new List<string>();
            foreach (var answer in questionDetails.OfferedAnswerList)
            {
                var rangeOrAnswer = Regex.Split(answer, @"-");
                if (rangeOrAnswer.Length > 1)
                {
                    int initialRange;
                    int.TryParse(rangeOrAnswer[0], out initialRange);
                    int finalRange;
                    int.TryParse(rangeOrAnswer[1], out finalRange);
                    for (var i = initialRange; i < finalRange; ++i)
                        answerList.Add(i.ToString());
                }
                else
                {
                    answerList.Add(answer);
                }
            }

            //Debug.Log(answerSelection.GetChild(0).GetChild(0).name +
            //          " is dropdown");
            var dropdown = answerSelection.GetChild(0).GetChild(0)
                .GetComponent<Dropdown>();
            dropdown.AddOptions(answerList);
            return tempPrefab;
        }

        private GameObject MessegeSetup(QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(MessegePrefab,
                    questionDetails.QuestionString);

            return tempPrefab;
        }

        private GameObject PickAllSetup(QuestionDetails currentDetails)
        {
            var tempPrefab = MultipleSetup(currentDetails);
            Destroy(tempPrefab.transform.GetChild(1).GetChild(0).GetChild(0)
                .gameObject);
            return tempPrefab;
        }

        private GameObject ScaleSetup(
            QuestionDetails questionDetails)
        {
            var tempPrefab =
                InstatiatePrefabAndPopulateAnswer(ScalePrefab,
                    questionDetails.QuestionString);
            //Debug.Log(tempPrefab.transform.GetChild(1).GetChild(0).GetChild(0)
            //    .GetChild(0).GetChild(0).GetChild(0).name);
            tempPrefab.transform.GetChild(1).GetChild(0)
                    .GetChild(0).GetChild(0).GetChild(0).GetChild(0)
                    .GetComponent<Text>().text =
                questionDetails.OfferedAnswerList[0];
            tempPrefab.transform.GetChild(1).GetChild(0)
                    .GetChild(0).GetChild(0).GetChild(0).GetChild(1)
                    .GetComponent<Text>().text =
                questionDetails.OfferedAnswerList[1];
            return tempPrefab;
        }


        /// <summary>
        ///     Calls specific method based on which question type it is.
        /// </summary>
        /// <param name="currentDetails"></param>
        /// <returns></returns>
        private GameObject FindOutQuestionType(QuestionDetails currentDetails)
        {
            //Debug.Log(currentDetails.QuestionType);
            switch (currentDetails.QuestionType)
            {
                case "FreeResponse":
                    return FreeResponseSetUp(currentDetails);
                case "Multiple":
                    return MultipleSetup(currentDetails);
                case "Scalar":
                    return ScalarSetup(currentDetails);
                case "Numerical":
                    return NumericSetup(currentDetails);
                //special cases bellow

                case "Intro":
                    return MessegeSetup(currentDetails);
                case "Outro":
                    return MessegeSetup(currentDetails);
                case "Debrief":
                    return MultipleSetup(currentDetails);
                case "IfYesRespond":
                    return MultipleSetup(currentDetails);
                case "IfNoRespond":
                    return MultipleSetup(currentDetails);
                case "IfScalarLessThan3Respond":
                    return MultipleSetup(currentDetails);
                case "Scale":
                    return ScaleSetup(currentDetails);
                case "TLX":
                    return ScaleSetup(currentDetails);

                case "PickAll":
                    return PickAllSetup(currentDetails);
            }

            return FreeResponseSetUp(currentDetails);

            //Debug.Log(currentDetails.QuestionType + " is question type");
            //Debug.Log(currentDetails.QuestionString + " is question string");
            //Debug.Assert(false,
            //    string.Format("Question of type {0} does not exist",
            //        currentDetails.QuestionType));
            return null;
        }

        /// <summary>
        ///     Loads a prefab based on List of info about the question.
        /// </summary>
        /// <param name="questionDetails">
        ///     Follows the format of :
        ///     question_type, question string, answer, ..., answer
        /// </param>
        private void LoadPrefab(QuestionDetails questionDetails)
        {
            var go = FindOutQuestionType(questionDetails);
            go.transform.SetParent(gameObject.transform);
            go.name = _questionIndex.ToString();
            go.GetComponent<RectTransform>().sizeDelta = go.transform.parent
                .GetComponent<RectTransform>().sizeDelta;
            go.GetComponent<RectTransform>().position = go.transform.parent
                .GetComponent<RectTransform>().position;
            if (_go.Count != 0)
            {
                var x = _go.Last();
                x.SetActive(false);
            }

            _go.Add(go);
        }

        private void ReloadPrefab()
        {
            _go.Last().SetActive(false);
            var go = _go[_questionIndex];
            go.transform.SetAsLastSibling();
            go.SetActive(true);
        }

        //possibly different implemintations based on survey
        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// TODO:help me
        //private IEnumerable Load(int path)
        //{
        //    var downloadManager = gameObject.AddComponent<LoadSurveyFromWeb>();
        //    downloadManager.SurveyList
        //    return stuff;

        //    //read survey from path (textfile)
        //}
        /// <summary>
        ///     Changes the question prefab to a new prefab.
        /// </summary>
        /// TODO: make it so that if there is no previous question, button doenst exist, else
        /// there is a previous button
        /// TODO: when there are no next questions, have an end button.
        private void UpdateLiveFeed()
        {
            if (_questionIndex == _surveyQuestionList.Count)
            {
                return;
            }
            else
            {
                _nextButton.GetComponentInChildren<Text>().text =
                    HasNextQuestion() ? "Next" : "Continue";
                if (HasPreviousQuestion())
                {
                    _backButton.Enable();
                }
                else
                {
                    _backButton.Disable();
                    _nextButton.GetComponentInChildren<Text>().text = "Begin";
                }

                Debug.Log(_go.Count + " = Count:Index = " + _questionIndex);
                if (_go.Count == _questionIndex)
                {
                    LoadPrefab(_surveyQuestionList[_questionIndex]);
                }
                else if (_go.Count < _questionIndex)
                {
                    _nextButton.Disable();
                    GoToNextScene();
                }
                else
                {
                    ReloadPrefab();
                }
            }
        }

        /// <summary>
        ///     Switches to next question in the survey
        /// </summary>
        public void NextQuestion()
        {
            /*if (HasNextQuestion())
            {
                _backButton.Enable(); //disable last question button;
            }

            else
            {
                _nextButton.Disable(); //disable last question button;
            }*/

            ++_questionIndex;
            UpdateLiveFeed();
        }

        /// <summary>
        ///     Switches to previous question in the survey
        /// </summary>
        public void PreviousQuestion()
        {
            --_questionIndex;
            UpdateLiveFeed();
        }

        internal void AnswerQuestion(string response)
        {
            //record answer
        }

        /// <summary>
        ///     Loads the next scene/kills this one.
        /// </summary>
        /// <remarks>
        ///     Because Collin wanted this.
        /// </remarks>
        /// <remarks>
        ///     Last Modified: Collin Miller
        ///     Date: 1/12/18
        ///     Reason now the survey will fork to the final screen if the participant is done with experiment.
        /// </remarks>
        private void GoToNextScene()
        {
            var ag = gameObject.AddComponent<AnswerGatherer>();

            ag.GatherAnswers(ref _go, ref _surveyQuestionList, _surveyNumber);
            //Participant.Instance.CurrentSurvey += 1;


            //SceneManager.getSurveyName();

            // SceneManager.LoadScene(
            //    ParticipantBehavior.Instance.CurrentMission == 6
            //       ? "FinalScene"
            //      : "QueryScreen");
            //_nextButton.Disable();
        }
    }
}