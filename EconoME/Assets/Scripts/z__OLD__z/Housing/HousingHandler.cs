using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HousingHandler : MonoBehaviour
{
    [field: SerializeField] public HouseBase HouseBase { get; private set; }
    [SerializeField] HouseTravel TravelingDirection = HouseTravel.Enter;

    private void Awake()
    {
        //If loading a scene for the house
        if (TravelingDirection == HouseTravel.Enter)
            HouseBase.OnEnter += OnEnterHouseScene;
        if (TravelingDirection == HouseTravel.Exit)
            HouseBase.OnExit = OnExitHouseScene;
        HouseBase.OnEnter += () => {Debug.Log("Entered House"); };
    }


    void OnEnterHouseScene()
    {
        SceneManager.LoadSceneAsync(HouseBase.SceneNum, LoadSceneMode.Additive);
    }

    void OnExitHouseScene()
    {
        SceneManager.UnloadSceneAsync(HouseBase.SceneNum);
    }

    private void OnDisable()
    {
        if (TravelingDirection == HouseTravel.Enter)
            HouseBase.OnEnter = null;
        if (TravelingDirection == HouseTravel.Exit)
            HouseBase.OnExit = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            AttemptHousingTravel();

        void AttemptHousingTravel()
        {
            if (TravelingDirection == HousingHandler.HouseTravel.Enter)
            {
                HouseBase.OnEnter?.Invoke();
                HouseBase.OnEnter = null;
                return;
            }

            HouseBase.OnExit?.Invoke();
            HouseBase.OnExit = null;
            return;
        }
    }

    enum HouseTravel
    {
        Enter,
        Exit
    }
}

