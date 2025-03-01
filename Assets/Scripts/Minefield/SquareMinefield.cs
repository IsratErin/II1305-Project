using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SquareMinefield : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TMP_Text mineCountText;

    private int mineId;
    private int surroundingMines = 0;
    private GameObject mine;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;

     [SerializeField] private AudioSource minesDiscovered;
    [SerializeField] private AudioSource minesNotFound;
     [SerializeField] private AudioSource safePath;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = GameManager.Instance;
    } 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager.CurrentState == GameManager.GameState.Mine)
        {
            if (spriteRenderer.sharedMaterial == gameManager.MinefieldMaterial || gameManager.MinefieldDark)
            {
                gameObject.ApplyCorrectMaterial();
            }
        }
        else if (gameManager.CurrentState == GameManager.GameState.Battle)
        {
            if (gameManager.SelectedSquareSafe != null)
            {
                SquareSafe selectedSafeSquare = gameManager.SelectedSquareSafe.GetComponent<SquareSafe>();
                if (selectedSafeSquare.Drawing)
                {
                    if (!gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().Waypoints.Contains(gameObject))
                    {
                        Vector2Int pos1 = gameManager.GetPosition(gameObject);
                        Vector2Int pos2 = gameManager.GetPosition(gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().LastWaypoint());
                        if (Utils.AreAdjacent(pos1, pos2) && !Utils.isBackwards(pos1, pos2))
                        {
                            safePath.Play();
                            gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().AddWaypoint(gameObject);
                        }
                    }
                }

            }
        }
        else
        {
            gameObject.ApplyIncorrectMaterial();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spriteRenderer.sharedMaterial != gameManager.PathMaterial)
        {
            gameObject.ApplyOriginalMaterial();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameManager.CurrentState == GameManager.GameState.Mine)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameManager.Instance.mineCounter > 9) {
                    return;
                }
                AddMine(-1);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveMine();
            }
        }
        else if (gameManager.CurrentState == GameManager.GameState.Minesweeper)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                gameManager.clicksCounter++;
                if (gameManager.clicksCounter > 4) {
                    return;
                }
                if (mine) {
                    minesDiscovered.Play();
                    mineCountText.text = "M";
                } else {
                    minesNotFound.Play();
                    mineCountText.text = surroundingMines.ToString();
                }
            }
        }
        else if (gameManager.CurrentState == GameManager.GameState.Battle)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                /*    if (gameManager.SelectedSquareSafe != null)
                    {
                        Vector2Int pos1 = gameManager.GetPosition(gameObject);
                        Vector2Int pos2 = gameManager.GetPosition(gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().LastWaypoint());

                        if (Utils.AreAdjacent(pos1, pos2))
                        {
                            gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().AddWaypoint(gameObject);
                        }
                    } */
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                if (gameObject == gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().LastWaypoint())
                {
                    GameObject lastWaypoint = gameManager.SelectedSquareSafe.GetComponent<SquareSafe>().RemoveWaypoint();
                    lastWaypoint.ApplyOriginalMaterial();
                }
            }
        }
    }

    public void AddMine(int newMineId)
    {
        if (!HasMine())
        {
            GameObject newMine = Instantiate(GameManager.Instance.minePrefab0, transform.position, Quaternion.identity);
            newMine.transform.parent = transform;
            mine = newMine;
            MineId = newMineId;
            if (GameManager.Instance.CurrentState == GameManager.GameState.Minesweeper) {
                mine.GetComponent<SpriteRenderer>().enabled = false;
            }
            gameManager.mineCounter++;
        }
    }

    public void RemoveMine()
    {
        if (mine)
        {
            Destroy(mine);
            mineId = 0;
            gameManager.mineCounter--;
        }
    }

    public bool HasMine()
    {
        return mineId < 0;
    }

    public int MineId
    {
        get { return mineId; }
        set { mineId = value; }
    }

    public int SurroundingMines
    {
        get { return surroundingMines; }
        set { surroundingMines = value; }
    }


}
