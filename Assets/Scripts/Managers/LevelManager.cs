using Commands;
using Data.UnityObjects;
using UnityEngine;
using Signals;



namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        #endregion

        #region Serialized Variables

        [SerializeField] private int totalLevelCount, levelID;
        [SerializeField] private Transform levelHolder;

        #endregion

        #region Private Variables

        private CD_Level _levelData;

        private OnLevelLoaderCommand _levelLoaderCommand;
        private OnLevelDestroyerCommand _levelDestroyerCommand;

        #endregion

        #endregion

        private void Awake()
        {
            _levelData = GetLevelData();
            levelID = GetActiveLevel();

            Init();
        }

        private int GetActiveLevel()
        {
            if (ES3.FileExists())
            {
                if (ES3.KeyExists("Level"))
                {
                    return ES3.Load<int>("Level");
                }
            }

            return 0;
        }

        private CD_Level GetLevelData() => Resources.Load<CD_Level>("Data/CD_Level");

        private void Init()
        {
            _levelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _levelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize += _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel += _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;



        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize -= _levelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel -= _levelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onNextLevel -= OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;


        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        private void Start()
        {
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID);
            CoreUISignals.Instance.onOpenPanel?.Invoke(Enums.UIPanelTypes.Start, 1);
        }

        private void OnNextLevel()
        {
            levelID++;
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID);
        }

        private void OnRestartLevel()
        {
            //levelID++;
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke(levelID);
        }




    }
}