using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    //Static data
    public static GlobalSceneManager Instance;


    [SerializeField] SceneTransitionHandler sceneTransitionHandler;

    public static event Action OnSceneLoad { add { Instance._onSceneLoad += value; } remove { Instance._onSceneLoad -= value; } }
    public static event Action OnSceneStartDisable { add { Instance._onSceneStartDisable += value; } remove { Instance._onSceneStartDisable -= value; } }
    public static event Action OnSceneVisible { add { Instance._onSceneVisible += value; } remove { Instance._onSceneVisible -= value; } }
    public static event Action<WorldLocationData> OnLocationChange { add { Instance._onLocationChange += value; } remove { Instance._onLocationChange -= value; } }
    public static event Action OnScenesDataAquired { add { Instance._onScenesDataAquired += value; } remove { Instance._onScenesDataAquired -= value; } }

    event Action _onSceneLoad;
    event Action _onSceneStartDisable;
    event Action _onSceneVisible;
    event Action<WorldLocationData> _onLocationChange;
    event Action _onScenesDataAquired;

    [SerializeField] List<int> SceneIndexesThatMustBeLoaded = new();
    [SerializeField] int StartingSceneIndex;
    [SerializeField] int PersistantSceneIndex;
    bool _sceneLoading;

    public WorldLocationData CurrentLocation { get; private set; }
    public bool SceneTransitioning { get { return _sceneLoading; } }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 Global Scene Manager found!");
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    SceneTransitionHandler currentLoadingScreen;
    public void StartGame()
    {

        Scene currentScene = SceneManager.GetActiveScene();
        currentLoadingScreen = Instantiate(sceneTransitionHandler, transform);
        currentLoadingScreen.Initialize();
        currentLoadingScreen.OnScreenInvisible += () => { InitializeGame(); };
        currentLoadingScreen.OnScreenVisible += ScreenVisible;
        _onScenesDataAquired += GameStarted;

        _onSceneStartDisable?.Invoke();

        void GameStarted()
        {
            _onScenesDataAquired -= GameStarted;
            _onSceneLoad?.Invoke();
        }

        void ScreenVisible()
        {
            _sceneLoading = false;
            _onSceneVisible?.Invoke();
            Destroy(currentLoadingScreen.gameObject);
        }
    }

    private void InitializeGame()
    {
        Debug.Log("Unloading scenes...");
        StartCoroutine(RemoveInvalidScenes());
        IEnumerator RemoveInvalidScenes()
        {
            _sceneLoading = true;
            var sceneCount = SceneManager.sceneCount;

            List<int> openScenes = new();
            for (int i = 0; i < sceneCount; i++)
            {
                openScenes.Add(SceneManager.GetSceneAt(i).buildIndex);
            }

            currentLoadingScreen.AddLoadingText("Loading Persistant Scene...");
            //First load persistant scene if it isn't open so events can subscribe properly to it
            if (!openScenes.Contains(PersistantSceneIndex))
            {
                AsyncOperation PersistantSceneOperation = SceneManager.LoadSceneAsync(PersistantSceneIndex, LoadSceneMode.Additive);
                while (!PersistantSceneOperation.isDone)
                {
                    yield return null;
                }
                openScenes.Add(PersistantSceneIndex);
            }
            
            currentLoadingScreen.AddLoadingText("Loading all required scenes...");
            //Go through all the scenes we need to be loaded, and load them.
            List<AsyncOperation> SceneLoadingOperations = new();
            foreach (var sceneIndex in SceneIndexesThatMustBeLoaded)
            {
                //Skip scenes that are already loaded
                if (openScenes.Contains(sceneIndex))
                    continue;

                //Otherwise load em up baby!
                SceneLoadingOperations.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
            }

            //Keep checking for when the scenes are all loaded
            bool allScenesLoaded = false;
            while (!allScenesLoaded)
            {
                allScenesLoaded = true;
                foreach (var operation in SceneLoadingOperations)
                {
                    if (!operation.isDone)
                        allScenesLoaded = false;
                }
                yield return null;
            }
            //Once they are all loaded, unload em. We don't need em anymore, we are just using them for their data (location). Basically we have data in each scene that we may want to access from other scenes without having to load the other scene up. So we STEAL the data, and use the scenes like the little hoes they are.
            
            currentLoadingScreen.AddLoadingText("Unloading temporary scenes...");
            List<AsyncOperation> SceneUnloadingOperations = new();
            sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != PersistantSceneIndex && scene.buildIndex != StartingSceneIndex)
                {
                    SceneUnloadingOperations.Add(SceneManager.UnloadSceneAsync(scene));
                }
            }

            //Keep checking for when the scenes are all unloaded
            bool allScenesUnloaded = false;
            while (!allScenesUnloaded)
            {
                allScenesUnloaded = true;
                foreach (var operation in SceneUnloadingOperations)
                {
                    if (!operation.isDone)
                        allScenesUnloaded = false;
                }
                yield return null;
            }
            
            currentLoadingScreen.AddLoadingText("Done!");
            yield return null;
            _sceneLoading = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(StartingSceneIndex));
            Debug.Log("Data Aquired :)");
            _onScenesDataAquired?.Invoke();
        }



    }

    public void TransitionPlayerToLocation(WorldLocationData location)
    {
        if (location == null)
            return;

        LoadScene(location);
    }

    void LoadScene(WorldLocationData location)
    {
        if (_sceneLoading)
            return;
        _sceneLoading = true;
        int sceneIndex = location.SceneIndex;

        Scene currentScene = SceneManager.GetActiveScene();
        var transitionHandler = Instantiate(sceneTransitionHandler, transform);
        transitionHandler.Initialize();
        transitionHandler.OnScreenInvisible += () => { StartCoroutine(StartLoadingScene()); };
        transitionHandler.OnScreenVisible += ScreenVisible;
        _onSceneStartDisable?.Invoke();

        IEnumerator StartLoadingScene()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
            while (!loadOperation.isDone && !unloadOperation.isDone)
            {
                transitionHandler.UpdateStatus(loadOperation, unloadOperation);
                yield return null;
            }
            CurrentLocation = location;
            _onLocationChange?.Invoke(location);
            _onSceneLoad?.Invoke();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));

        }

        void ScreenVisible()
        {
            _sceneLoading = false;
            _onSceneVisible?.Invoke();
            Destroy(transitionHandler.gameObject);
        }
    }


}

