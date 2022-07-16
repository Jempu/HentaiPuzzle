using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jempu.HentaiPuzzle
{
    public class PuzzleControls : MonoBehaviour
    {
        public AssetSO Assets;
        private Sprite[] _sprites;

        public GameObject PuzzlePiece;
        public Transform PuzzleGrid;
        private RectTransform _puzzleGridRect;
        public Transform RevealedImage;

        public int ColumnCount = 3;
        public int RowCount = 3;
        public float Gap = 6;
        public float RevealAtStartTime = 2.5f;

        private PuzzlePiece _selectedPiece;
        private Vector3 _pieceLineStartPosition;

        private bool _settingPuzzle;
        private void SetPuzzle() => StartCoroutine(SetPuzzleCoroutine());
        private IEnumerator SetPuzzleCoroutine()
        {
            if (_settingPuzzle) yield break;
            _settingPuzzle = true;
            foreach (Transform t in PuzzleGrid)
            {
                Destroy(t.gameObject);
            }
            _puzzleGridRect = PuzzleGrid.GetComponent<RectTransform>();
            var grid = PuzzleGrid.GetComponent<GridLayoutGroup>();
            grid.spacing = new Vector2(Gap, Gap);
            grid.cellSize = new Vector2(
                (_puzzleGridRect.rect.width / ColumnCount) - ((grid.spacing.x / ColumnCount) * (ColumnCount - 1)),
                (_puzzleGridRect.rect.height / RowCount) - ((grid.spacing.y / RowCount) * (RowCount - 1))
            );
            var targetSprite = _sprites[Random.Range(0, _sprites.Length)];
            if (RevealAtStartTime > 0f)
            {
                RevealedImage.GetComponent<Image>().sprite = targetSprite;
                RevealedImage.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(RevealAtStartTime);
            }
            RevealedImage.gameObject.SetActive(false);
            var id = 0;
            for (var row = 0; row < RowCount;)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    var obj = Instantiate(PuzzlePiece, PuzzleGrid).transform;
                    var image = obj.Find("pointer/image");
                    var imageComponent = image.GetComponent<Image>();
                    imageComponent.sprite = targetSprite;
                    obj.name = $"{row.ToString()}: {column.ToString()}";
                    var or = obj.GetComponent<RectTransform>();
                    var ir = imageComponent.rectTransform;
                    ir.sizeDelta = new Vector2(_puzzleGridRect.rect.width, _puzzleGridRect.rect.height);
                    ir.anchoredPosition = new Vector2(
                        ir.rect.width / 2 - (ir.rect.width / ColumnCount) * column,
                        -ir.rect.height / 2 + (ir.rect.height / RowCount) * row
                    );
                    var idComponent = obj.GetComponent<PuzzlePiece>();
                    idComponent.puzzleControls = this;
                    idComponent.columnPlace = column;
                    idComponent.rowPlace = row;
                    idComponent.SetID(id);
                    id++;
                    yield return new WaitForSecondsRealtime(0.12f);
                }
                row++;
            }
            yield return new WaitForSecondsRealtime(1.1f);
            ShufflePuzzle();
            yield return new WaitForSecondsRealtime(1f);
            _settingPuzzle = false;
        }

        private void ShufflePuzzle()
        {
            for (var i = 0; i < RowCount * ColumnCount; i++)
            {
                SetPuzzlePiecePlace(PuzzleGrid.GetChild(i));
            }
        }

        private void SetPuzzlePiecePlace(Transform piece)
        {
            var id = piece.GetComponent<PuzzlePiece>().id;
            var targetPlace = Random.Range(0, PuzzleGrid.childCount - 1);
            
            // NO SHUFFLING Detection NOW

            // check that surrounding pieces (up, left, right and bottom) don't match
            //      X
            //     X+X
            //      X
            // var previousObjectId = targetPlace > 0 ? PuzzleGrid.GetChild(targetPlace - 1).GetComponent<PuzzlePiece>().id : -1;
            // var nextObjectId = PuzzleGrid.GetChild(targetPlace + 1).GetComponent<PuzzlePiece>().id;
            // if (previousObjectId == id - 1 || nextObjectId == id + 1)
            // {
            //     SetPuzzlePiecePlace(piece);
            // }
            piece.SetSiblingIndex(targetPlace);
        }

        private void ResetPuzzle()
        {
            // resets into original shuffle
        }

        private void CheckPuzzle()
        {
            for (var i = PuzzleGrid.childCount - 1; i > 1; i--)
            {
                if (PuzzleGrid.GetChild(i - 1).GetComponent<PuzzlePiece>().id != i - 1)
                {
                    Debug.Log("The puzzle is not solved :/");
                    return;
                }
            }
            Debug.Log("The puzzle is solved :)");
        }

        public void SelectPiece(PuzzlePiece piece)
        {
            _selectedPiece = piece;
        }

        public void UnselectPiece(PuzzlePiece piece)
        {
            _selectedPiece = null;
        }

        public LineRenderer pieceLine;

        private void ControlPiece()
        {
            if (_selectedPiece == null)
            {
                _pieceLineStartPosition = Vector3.zero;
                return;
            }
            Vector2 pos;
            var mousePosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
            pos = new Vector2(mousePosition.x, mousePosition.y);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, Camera.main, out pos);
            _selectedPiece.Pointer.anchoredPosition = transform.TransformPoint(pos);
            // Debug.Log(pos);
            if (_pieceLineStartPosition == Vector3.zero)
            {
                _pieceLineStartPosition = pos;
            }
            pieceLine.SetPositions(new Vector3[2] { _pieceLineStartPosition, pos });
        }

        private void Start()
        {
            _sprites = Assets.Sprites;
            SetPuzzle();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CheckPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                ShufflePuzzle();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetPuzzle();
            }
            ControlPiece();
        }
    }
}