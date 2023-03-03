using UnityEngine;
using TMPro;

namespace Assets.Scripts.Admin
{
    internal class AdminDebug : MonoBehaviour
    {
        [SerializeField] bool DebugOn;
        [SerializeField] CanvasGroup _debugCanvas;
        [SerializeField] TextMeshProUGUI _debugText;


        private void OnEnable()
        {
            
        }
        private void Start()
        {
            WorldTimeManager.Instance.OnGameTick += OnTimeTick;
        }

        private void OnTimeTick()
        {
            if (DebugOn)
            {
                if (_debugCanvas.alpha != 0)
                    _debugCanvas.SetCanvas(true);

                var currentTime = WorldTimeManager.CurrentTime;
                string timeText = $"Day: {currentTime.Day} \n" +
                    $"Time: {WorldTimeManager.CurrentGameTimeToHumanTime}\n" +
                    $"Tick: {currentTime.TimeOfDayTick}\n";

                _debugText.text = timeText;
                return;

            }

             if (_debugCanvas.alpha == 0)
                    _debugCanvas.SetCanvas(false);

        }

        private void OnDisable()
        {
            _debugCanvas.SetCanvas(false);
            WorldTimeManager.Instance.OnGameTick -= OnTimeTick;
        }
    }

}



