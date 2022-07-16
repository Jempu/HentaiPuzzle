using UnityEngine;

namespace Jempu.HentaiPuzzle
{
    [CreateAssetMenu(menuName = "HentaiPuzzle/Level")]
    public class Level : ScriptableObject
    {
        public Sprite Image;

        [Min(2)]
        [Range(2, 69)]
        public int PuzzleWidth = 2;
        [Min(2)]
        [Range(2, 69)]
        public int PuzzleHeight = 2;

        public float Timer = -1;
    }
}