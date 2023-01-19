using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class WorldTileHandler : MonoBehaviour
{
    //TileObject Class: Tile Item similar to a block in other games
    //Can convert large in world tiles to inventory tiles

    //Public Data

    public Camera CameraPrefab;

    public WorldTile data;
    public EdgeCollider2D[] DoorColliders = new EdgeCollider2D[4];
    [SerializeField] Transform _resourceNodesParent;
    [SerializeField] Transform _interactableItemsParent;
    [SerializeField] Tilemap _baseLayerTileMap;
    [SerializeField] Tilemap _interactiveLayerTileMap;
    [SerializeField] Tilemap _detailsLayerTileMap;
    public event Action OnTilemapLoaded;
    public event Action OnPictureRetrieved;
    [SerializeField] WorldItemHandler GroundItemPrefab;

    public bool isRendered = true;
    public Transform ResourceNodesParent { get { return _resourceNodesParent; } }
    public Transform InteractableItemsParent { get { return _interactableItemsParent; } }
    public Tilemap BaseLayerTileMap { get { return _baseLayerTileMap; } }
    public Tilemap InteractiveLayerTileMap { get { return _interactiveLayerTileMap; } }
    public Tilemap DetialsLayerTileMap { get { return _detailsLayerTileMap; } }
    public TileConnector[] BaseDoors = new TileConnector[4];

    //Private Members
    public Camera cam { get; private set; }

    int pictureSize;
    //Get image of map tile
    //Move Item ofscreen (+z), Set Camera above tile, Set image to new Tile
    public void getMapImage()
    {
        pictureSize = 512;
        transform.position -= new Vector3(0, 0, 5000);
        //Setup Camera
        cam = Instantiate(CameraPrefab, transform);
        cam.targetTexture = RenderTexture.GetTemporary(pictureSize, pictureSize, 25);
        RenderPipelineManager.endCameraRendering += OnPostRenderCallBack2;
        //Process Pic on Next frame
    }

    //Take Screenshot after end of frame

    private void OnPostRenderCallBack2(ScriptableRenderContext context, Camera foundCam)
    {
        if (foundCam == cam)
        {
            CamReady();
        }

    }
    public void CamReady()
    {
        RenderPipelineManager.endCameraRendering -= OnPostRenderCallBack2;
        //Generate Screenshot
        RenderTexture renderTexture = cam.targetTexture;
        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
        renderResult.filterMode = FilterMode.Point;
        renderResult.Apply();
        renderResult.ReadPixels(rect, 0, 0);
        if (!isRendered)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        transform.position += new Vector3(0, 0, 5000);
        //Save Screenshot to File
        var filename = Application.dataPath + "/TileScreenshots/" + data.TileName + "screenshot.png";

        byte[] byteArray = renderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, byteArray);
        //Generate Sprite for Item
        Texture2D pic = null;
        if (pic == null)
        {
            pic = new Texture2D(2, 2);
        }

        pic.LoadImage(byteArray);

        data.SetIcon(Sprite.Create(pic, rect, new Vector2(0.5f, 0.5f), pictureSize / 32), filename);
        //Cleanup
        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.ReleaseTemporary(cam.targetTexture);
        cam.targetTexture = null;
        cam.enabled = false;
        renderTexture = null;
        byteArray = null;

        Destroy(renderResult);
        OnPictureRetrieved?.Invoke();
    }


    public void PlaceTile(PlaceTileData placeTileData)
    {
        Vector2Int pos = placeTileData.MapPosition;
        TileDatabase tilesData = placeTileData.tileDatabase;
        InteractablesDatabase interactableDatabase = placeTileData.interactablesDatabase;
        ResourceNodeDatabase nodeDatabase = placeTileData.nodesDatabase;

        PlaceDoors();

        PlaceTileMaps();

        PlaceInteractables();

        OnTilemapLoaded?.Invoke();


        void PlaceTileMaps()
        {
            BoundsInt area = new(0, 0, 0, data.size.x, data.size.y, 1);
            TileBase[] BaseLayerTiles = new TileBase[data.size.x * data.size.y];
            TileBase[] InteractableLayerTiles = new TileBase[data.size.x * data.size.y];
            TileBase[] DetailsLayersTiles = new TileBase[data.size.x * data.size.y];


            for (int i = 0; i < data.size.x; i++)
            {
                for (int j = 0; j < data.size.y; j++)
                {
                    int index = i + (j * data.size.y);
                    if (data.BaseLayerTiles[index] > 0)
                    {
                        BaseLayerTiles[index] = tilesData.FindTile(data.BaseLayerTiles[index]);
                    }
                    if (data.InteractableLayerTiles[index] > 0)
                    {
                        InteractableLayerTiles[index] = tilesData.FindTile(data.InteractableLayerTiles[index]);
                    }
                    if (data.DetailsLayerTiles[index] > 0)
                    {
                        DetailsLayersTiles[index] = tilesData.FindTile(data.DetailsLayerTiles[index]);
                    }

                }
            }

            BaseLayerTileMap.transform.localPosition -= new Vector3Int(data.size.x / 2, data.size.y / 2, 0);
            BaseLayerTileMap.SetTilesBlock(area, BaseLayerTiles);
            InteractiveLayerTileMap.transform.localPosition -= new Vector3Int(data.size.x / 2, data.size.y / 2, 0);
            InteractiveLayerTileMap.SetTilesBlock(area, InteractableLayerTiles);
            DetialsLayerTileMap.transform.localPosition -= new Vector3Int(data.size.x / 2, data.size.y / 2, 0);
            DetialsLayerTileMap.SetTilesBlock(area, DetailsLayersTiles);
        }

        void PlaceDoors()
        {
            BaseDoors[0].transform.localPosition = new Vector3(0, data.size.y / 2);
            BaseDoors[0].TilePosition = pos;
            BaseDoors[1].transform.localPosition = new Vector3(data.size.x / 2, 0);
            BaseDoors[1].TilePosition = pos;
            BaseDoors[2].transform.localPosition = new Vector3(0, -data.size.y / 2);
            BaseDoors[2].TilePosition = pos;
            BaseDoors[3].transform.localPosition = new Vector3(-data.size.x / 2, 0);
            BaseDoors[3].TilePosition = pos;
        }

        void PlaceInteractables()
        {
            for (int i = 0; i < data.Interactables.Count; i++)
            {
                GameObject foundPrefab = interactableDatabase.FindPrefab(data.Interactables[i].InteractableID);
                Instantiate(foundPrefab, InteractableItemsParent);
            }
            for (int i = 0; i < data.resourceNodes.Count; i++)
            {
                if (data.resourceNodes[i].NodePrefabID != 0)
                {
                    ResourceNodeHandler foundPrefab = nodeDatabase.FindPrefab(data.resourceNodes[i].NodePrefabID);
                    ResourceNodeHandler RN = Instantiate(foundPrefab, ResourceNodesParent).GetComponent<ResourceNodeHandler>();
                    RN.data = data.resourceNodes[i];
                    RN.transform.localPosition = RN.data.getTilePosition(data.size);
                }

            }
        }
    }

    #region Shrink Tile Object to Inventory Item

    //Convert current TileObject to in game collectable item
    //Posistion for Where to create it
    //Time to data.Shrink
    [ContextMenu("turnToObject")]
    public void turnToObject()
    {
        //data.Shrink Item if Not Shrunk
        if (data.Shrink == 0)
        {
            getMapImage();
            //Start data.Shrink, IEnumerator/Coroutine to data.Shrink over time
            data.Shrink = 1;
            RenderPipelineManager.endCameraRendering += TurnToObjectAfterPicture;

        }

        return;

    }

    private void TurnToObjectAfterPicture(ScriptableRenderContext context, Camera foundCam)
    {
        if (foundCam == cam)
        {
            FinishTurningToObject();
        }

    }

    void FinishTurningToObject()
    {
        StartCoroutine(ShrinkTile());

        IEnumerator ShrinkTile()
        {
            if (data.Icon == null)
            {
                Debug.Log("No icon attached to tile");
            }
            //TileItem: New Gameobject to be Created
            var WUI = Instantiate(GroundItemPrefab);
            //Move data to new Gameobject's Components
            data.PlacedTileHandler = null;
            WUI.data.item = data;
            WUI.updateImage();

            //Set new gameobject transform to replace tile
            WUI.transform.position = transform.position + new Vector3Int(1, 1, 0);
            WUI.transform.localScale = new Vector3(32, 32, 1);

            //Set old tile to invisible until complete (then destroyed)
            gameObject.transform.localScale = Vector3.zero;

            PickupInteraction PI = WUI.GetComponent<PickupInteraction>();
            PI.CanBePickedUp = false;

            //data.Shrinking of new Gameobject
            while (WUI.transform.localScale.x > 1)
            {
                WUI.transform.localScale -= new Vector3(0.036f, 0.036f, 0);
                yield return new WaitForSeconds(0.005f);
            }

            //Completed data.Shrink. Generate pickup item components
            PI.CanBePickedUp = true;
            (WUI.data.item as WorldTile).Shrink = 2;
            //Destroy Original Tile
            Destroy(gameObject);

        }
    }

    #endregion


    public event Action onGrowTile;

    public void StartGrowTile(GameObject SpriteToGrow, bool isInstant)
    {
        StartCoroutine(GrowTile());

        IEnumerator GrowTile()
        {
            if (!isInstant)
            {
                while (SpriteToGrow.transform.localScale.x < 1)
                {
                    SpriteToGrow.transform.localScale += new Vector3(0.0036f, 0.0036f, 0);
                    yield return new WaitForSeconds(0.005f);
                }
            }
            var PlacedTile = this;
            Destroy(SpriteToGrow);
            PlacedTile.transform.localScale = Vector3.one;
            PlacedTile.GetComponent<WorldTileHandler>().data.Shrink = 0;

            #region Generate Colliders
            Vector2 OffSet = new Vector2(data.size.x / 2f, data.size.y / 2f) - Vector2.one;
            Vector2 MiddleLeft = new Vector2(0, ((float)data.size.y / 2)) - OffSet;
            Vector2 MiddleTop = new Vector2((float)data.size.x / 2, data.size.y) - OffSet;
            Vector2 MiddleRight = new Vector2(data.size.x, ((float)data.size.y / 2)) - OffSet;
            Vector2 MiddleDown = new Vector2(((float)data.size.x / 2), 0) - OffSet;

            Vector2 TopLeft = new Vector2(0, data.size.y) - OffSet;
            Vector2 TopRight = new Vector2(data.size.x, data.size.y) - OffSet;
            Vector2 BottomRight = new Vector2(data.size.x, 0) - OffSet;
            Vector2 BottomLeft = new Vector2(0, 0) - OffSet;

            float doorSize = 1;

            //Top left Corner
            var EdgesCollider1 = PlacedTile.transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
            var Edge1Points = new Vector2[3];

            Edge1Points[0] = MiddleLeft + new Vector2(0, doorSize / 2);
            Edge1Points[1] = TopLeft;
            Edge1Points[2] = MiddleTop - new Vector2(doorSize / 2, 0);

            EdgesCollider1.points = Edge1Points;
            //Top right corner
            var EdgesCollider2 = PlacedTile.transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
            var Edge2Points = new Vector2[3];

            Edge2Points[0] = MiddleTop + new Vector2(doorSize / 2, 0);
            Edge2Points[1] = TopRight;
            Edge2Points[2] = MiddleRight + new Vector2(0, doorSize / 2);

            EdgesCollider2.points = Edge2Points;

            //Bottom right corner
            var EdgesCollider3 = PlacedTile.transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
            var Edge3Points = new Vector2[3];

            Edge3Points[0] = MiddleRight - new Vector2(0, doorSize / 2);
            Edge3Points[1] = BottomRight;
            Edge3Points[2] = MiddleDown + new Vector2(doorSize / 2, 0);

            EdgesCollider3.points = Edge3Points;
            //Bottom left corner
            var EdgesCollider4 = PlacedTile.transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
            var Edge4Points = new Vector2[3];

            Edge4Points[0] = MiddleDown - new Vector2(doorSize / 2, 0);
            Edge4Points[1] = BottomLeft;
            Edge4Points[2] = MiddleLeft - new Vector2(0, doorSize / 2);

            EdgesCollider4.points = Edge4Points;


            //Top door
            if (data.doorConnectors[0] == null)
            {
                DoorColliders[0].enabled = true;
                var DoorPoints = new Vector2[2];
                DoorPoints[0] = MiddleTop - new Vector2(doorSize / 2, 0);
                DoorPoints[1] = MiddleTop + new Vector2(doorSize / 2, 0);
                DoorColliders[0].points = DoorPoints;
            }
            else
            {
                DoorColliders[0].enabled = false;
            }
            //Right door
            if (data.doorConnectors[1] == null)
            {
                var DoorPoints = new Vector2[2];
                DoorColliders[1].enabled = true;
                DoorPoints[0] = MiddleRight - new Vector2(0, doorSize / 2);
                DoorPoints[1] = MiddleRight + new Vector2(0, doorSize / 2);
                DoorColliders[1].points = DoorPoints;
            }
            else
            {
                DoorColliders[1].enabled = false;
            }
            //Bottom door
            if (data.doorConnectors[2] == null)
            {
                var DoorPoints = new Vector2[2];
                DoorColliders[2].enabled = true;
                DoorPoints[0] = MiddleDown - new Vector2(doorSize / 2, 0);
                DoorPoints[1] = MiddleDown + new Vector2(doorSize / 2, 0);
                DoorColliders[2].points = DoorPoints;
            }
            else
            {
                DoorColliders[2].enabled = false;
            }
            //Left Door
            if (data.doorConnectors[3] == null)
            {
                var DoorPoints = new Vector2[2];
                DoorColliders[3].enabled = true;
                DoorPoints[0] = MiddleLeft - new Vector2(0, doorSize / 2);
                DoorPoints[1] = MiddleLeft + new Vector2(0, doorSize / 2);
                DoorColliders[3].points = DoorPoints;
            }
            else
            {
                DoorColliders[3].enabled = false;
            }

            onGrowTile?.Invoke();
            onGrowTile = null;
            #endregion
        }
    }

    internal void UpdateNewConnections(Vector2Int CheckPos, WorldTileHandler otherTileHandler)
    {
        float doorsize = 1;
        int DoorNum = 0;
        int OppositeDoor = 0;
        Vector3 MovePos = Vector3.up;
        Vector2 topPoints1 = new Vector2();
        Vector2 topPoints2 = new Vector2();
        Vector2 botPoints1 = new Vector2();
        Vector2 botPoints2 = new Vector2();

        Vector2 OffSet = new Vector2(data.size.x / 2f, data.size.y / 2f) - Vector2.one;

        switch (CheckPos)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                DoorNum = 0;
                MovePos = Vector3.up;
                OppositeDoor = 2;

                topPoints1 = new Vector2((float)data.size.x / 2, data.size.y) - OffSet + new Vector2(doorsize / 2, 0);
                topPoints2 = topPoints1 + new Vector2(0, 2);

                botPoints1 = new Vector2((float)data.size.x / 2, data.size.y) - OffSet - new Vector2(doorsize / 2, 0);
                botPoints2 = botPoints1 + new Vector2(0, 2);

                break;
            case Vector2Int v when v.Equals(Vector2Int.right):
                DoorNum = 1;
                MovePos = Vector3.right;
                OppositeDoor = 3;

                topPoints1 = new Vector2(data.size.x, (float)data.size.y / 2) - OffSet + new Vector2(0, doorsize / 2);
                topPoints2 = topPoints1 + new Vector2(2, 0);

                botPoints1 = new Vector2(data.size.x, (float)data.size.y / 2) - OffSet - new Vector2(0, doorsize / 2);
                botPoints2 = botPoints1 + new Vector2(2, 0);

                break;
            case Vector2Int v when v.Equals(Vector2Int.down):
                DoorNum = 2;
                MovePos = Vector3.down;
                OppositeDoor = 0;

                topPoints1 = new Vector2((float)data.size.x / 2, 0) - OffSet + new Vector2(doorsize / 2, 0);
                topPoints2 = topPoints1 - new Vector2(0, 2);

                botPoints1 = new Vector2((float)data.size.x / 2, 0) - OffSet - new Vector2(doorsize / 2, 0);
                botPoints2 = botPoints1 - new Vector2(0, 2);


                break;
            case Vector2Int v when v.Equals(Vector2Int.left):
                DoorNum = 3;
                MovePos = Vector3.left;
                OppositeDoor = 1;

                topPoints1 = new Vector2(0, ((float)data.size.y / 2)) - OffSet + new Vector2(0, doorsize / 2);
                topPoints2 = topPoints1 - new Vector2(2, 0);

                botPoints1 = new Vector2(0, ((float)data.size.y / 2)) - OffSet - new Vector2(0, doorsize / 2);
                botPoints2 = botPoints1 - new Vector2(2, 0);
                break;
        }
        GameObject newTileConnection = Instantiate(BaseDoors[DoorNum].gameObject, BaseDoors[DoorNum].transform);
        var TopCollider = transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();
        var BotCollider = transform.GetChild(0).gameObject.AddComponent<EdgeCollider2D>();



        var topPoints = new Vector2[2];
        topPoints[0] = topPoints1;
        topPoints[1] = topPoints2;

        var botPoints = new Vector2[2];
        botPoints[0] = botPoints1;
        botPoints[1] = botPoints2;

        TopCollider.points = topPoints;
        BotCollider.points = botPoints;

        newTileConnection.transform.localPosition = Vector3.zero;
        newTileConnection.transform.localPosition += MovePos;
        DoorColliders[DoorNum].enabled = false;
        data.doorConnectors[DoorNum] = newTileConnection;

        onGrowTile += UnlockDoor;
        void UnlockDoor()
        {
            if (otherTileHandler != null)
                otherTileHandler.DoorColliders[OppositeDoor].enabled = false;
        }



    }



}

[Serializable]
public struct InteractablesData
{
    public int InteractableID;
    public Vector2Int pos;
}

public struct PlaceTileData
{
    public Vector2Int MapPosition;
    public TileDatabase tileDatabase;
    public ResourceNodeDatabase nodesDatabase;
    public InteractablesDatabase interactablesDatabase;

    public PlaceTileData(Vector2Int mapPosition, TileGeneratorObject tileGeneratorUsed)
    {
        MapPosition = mapPosition;
        tileDatabase = tileGeneratorUsed.TileDatabase;
        nodesDatabase = tileGeneratorUsed.NodeDatabase;
        interactablesDatabase = tileGeneratorUsed.InteractablesDatabase;
    }
}
