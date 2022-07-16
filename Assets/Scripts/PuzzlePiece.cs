using UnityEngine;
using UnityEngine.EventSystems;

namespace Jempu.HentaiPuzzle
{
    public class PuzzlePiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        internal int id { get; private set; }
        public void SetID(int id) => this.id = id;
        internal int columnPlace;
        internal int rowPlace;

        public PuzzleControls puzzleControls;
        public RectTransform Pointer;
        
        private void Start()
        {
            Pointer = transform.Find("pointer").GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            puzzleControls.SelectPiece(this);
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            puzzleControls.UnselectPiece(this);
        }
    }
}