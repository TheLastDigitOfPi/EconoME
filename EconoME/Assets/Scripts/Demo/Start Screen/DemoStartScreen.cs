using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DemoStartScreen : MonoBehaviour
{

    [SerializeField] GameObject TileChoiceImages;
    [SerializeField] GameObject ResourceNodeChoiceImages;
    [SerializeField] GameObject EnemyChoiceImages;
    [SerializeField] GameObject GenerateTileText;
    [SerializeField] GameObject TileGeneratedText;
    [SerializeField] ItemBase GeneralTilebase;
    [SerializeField] Camera ViewingCamera;
    [SerializeField] List<ResourceNodeSpawnSetting> _possibleSpawnSettings = new();
    List<ResourceNodeSpawnSetting> _spawnSettings = new();
    [SerializeField] List<MobSpawnSetting> _possibleMobSpawnSettings = new();
    List<MobSpawnSetting> _mobSpawnSettings = new();

    TileBiome biome;
    bool choicesMade = false;
    void Start()
    {
        TileChoiceImages.transform.DOLocalMove(Vector3.zero, 1.5f).SetEase(Ease.OutSine);
    }

    public void MakeTileChoice(int option)
    {
        TileChoiceImages.transform.DOLocalMove(Vector3.right * 2000, 1.5f).SetEase(Ease.InSine).onComplete += () => { ResourceNodeChoiceImages.transform.DOLocalMove(Vector3.zero, 1.5f).SetEase(Ease.OutSine); TileChoiceImages.SetActive(false); };
        switch (option)
        {
            case 0:
                biome = TileBiome.Arctic;
                break;
            case 1:
                biome = TileBiome.Forest;
                break;
            case 2:
                biome = TileBiome.DarkForest;
                break;
            case 3:
                biome = TileBiome.Desert;
                break;
            default:
                biome = TileBiome.Arctic;
                break;
        }
    }
    public void MakeResourceNodeChoice(int option)
    {
        switch (option)
        {
            case 0:
                _spawnSettings.Add(_possibleSpawnSettings[0]);
                break;
            case 1:
                _spawnSettings.Add(_possibleSpawnSettings[1]);
                break;
            case 2:
                _spawnSettings.Add(_possibleSpawnSettings[2]);
                break;
            default:
                break;
        }
        ResourceNodeChoiceImages.transform.DOLocalMove(Vector3.right * 2000, 1.5f).SetEase(Ease.InSine).onComplete += () => { EnemyChoiceImages.transform.DOLocalMove(Vector3.zero, 1.5f).SetEase(Ease.OutSine); ResourceNodeChoiceImages.SetActive(false);};
    }
    public void MakeEnemyChoice(int option)
    {
        switch (option)
        {
            case 0:
                _mobSpawnSettings.Add(_possibleMobSpawnSettings[0]);
                break;
            case 1:
                _mobSpawnSettings.Add(_possibleMobSpawnSettings[1]);
                break;
            default:
                break;
        }
        EnemyChoiceImages.transform.DOLocalMove(Vector3.right * 2000, 1.5f).SetEase(Ease.InSine).onComplete += ChoicesMade;
    }

    async void ChoicesMade()
    {
        if(choicesMade)
            return;
        choicesMade = true;
        EnemyChoiceImages.SetActive(false);
        GenerateTileText.SetActive(true);
        await Task.Delay(1500);
        GenerateTileText.transform.DOLocalMove(Vector3.up * 1000, 1).SetEase(Ease.OutSine).onComplete += () =>  GenerateTileText.SetActive(false);
        await Task.Delay(500);
        TileSettings settings = new(GeneralTilebase, 0, biome, _spawnSettings, _mobSpawnSettings);
        TileItem newTile = new(settings);
        WorldTileManager.Instance.TryPlaceTile(newTile, Vector2Int.zero, out var tileMade);
        tileMade.OnTileLoad += TileLoaded;
    }

    private async void TileLoaded(WorldTileHandler obj)
    {
        await Task.Delay(500);
        TileGeneratedText.SetActive(true);
        await Task.Delay(2000);
        ViewingCamera.gameObject.SetActive(false);
        DemoWorldManager.Instance.StartGamePlay();
        TileGeneratedText.SetActive(false);
    }
}
