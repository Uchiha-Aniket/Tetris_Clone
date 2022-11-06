using System.Collections;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private Transform _rowGlowFxsParent;
        private ParticlePlayer[] _rowGlowFXs = new ParticlePlayer[4];
        [SerializeField] private Transform _landShapeFxsParent;
        private ParticleSystem[] _landShapeFXs = new ParticleSystem[4];

        [Space]
        [SerializeField] private Transform _emptySquare;
        [SerializeField] private byte _height = 23;
        [SerializeField] private byte _width = 10;

        private Transform[,] _grid;

        public byte completedRows;

        // ----

        private void Awake()
        {
            _grid = new Transform[_width, _height];

            _rowGlowFXs = new ParticlePlayer[_rowGlowFxsParent.childCount];
            for (byte i = 0; i < _rowGlowFXs.Length; i++)
            {
                _rowGlowFXs[i] = _rowGlowFxsParent.GetChild(i).GetComponent<ParticlePlayer>();
            }

            _landShapeFXs = new ParticleSystem[_landShapeFxsParent.childCount];
            for (byte i = 0; i < _landShapeFXs.Length; i++)
            {
                _landShapeFXs[i] = _landShapeFxsParent.GetChild(i).GetComponent<ParticleSystem>();
            }
        }

        private void Start()
        {
            DrawEmptyCells();
        }
        
        // ----
        
        private void DrawEmptyCells()
        {
            if (!_emptySquare)
            {
                Debug.LogError("Please : Assign emptySquare", gameObject);
                return;
            }
            
            for (byte y = 0; y < _height; y++)
            {
                for (byte x = 0; x < _width; x++)
                {
                    var clone = Instantiate(_emptySquare, new Vector3(x, y, 0), Quaternion.identity);
                    clone.name = $"Board Space (x = {x}, y = {y})";
                    clone.parent = transform;
                }
            }
        }

        private bool IsWithinBoard(int x, int y) => x >= 0 && x < _width && y >= 0;

        public bool IsValidPosition(Shape shape)
        {
            foreach (Transform child in shape.transform)
            {
                var pos = Vector2Int.RoundToInt(child.position);

                if (!IsWithinBoard(pos.x, pos.y)) return false;
                if (IsOccupied(pos.x, pos.y)) return false;
            }
            return true;
        }

        private bool IsOccupied(int x, int y)
        {
            if (x >= _width || y >= _height) return false;
            return _grid[x, y] != null;
        }        
        
        public void StoreShapeToGrid(Shape shape)
        {
            if (shape == null) return;

            foreach (Transform child in shape.transform)
            {
                var pos = Vector2Int.RoundToInt(child.position);
                _grid[pos.x, pos.y] = child;
            }
        }

        private bool IsRowComplete(int y)
        {
            for (byte i = 0; i < _width; i++)
            {
                if (_grid[i, y] == null) return false;
            }
            return true;
        }

        private void ClearRow(int y)
        {
            for (byte i = 0; i < _width; i++)
            {
                if (_grid[i, y] != null)
                {
                    Destroy(_grid[i, y].gameObject);
                    _grid[i, y] = null;
                }
            }
        }

        private void ShiftRowDown(int y)
        {
            for (byte i = 0; i < _width; i++)
            {
                if (_grid[i, y] != null)
                {
                    _grid[i, y - 1] = _grid[i, y];
                    _grid[i, y] = null;
                    _grid[i, y - 1].position += Vector3.down;
                }
            }
        }

        private void ShiftRowsDown(int startY)
        {
            for (int i = startY; i < _height; i++)
            {
                ShiftRowDown(i);
            }
        }

        public IEnumerator ClearAllRows()
        {
            completedRows = 0;

            for (int i = 0; i < _height; i++)
            {
                if (IsRowComplete(i))
                {
                    PlayRowGlowFX(completedRows, i);
                    completedRows++;
                }
            }

            yield return new WaitForSeconds(.7f);

            for (int i = 0; i < _height; i++)
            {
                if (IsRowComplete(i))
                {
                    ClearRow(i);
                    ShiftRowsDown(i+1);
                    i--;
                }
            }
        }

        public bool IsGameOver(Shape shape)
        {
            foreach (Transform child in shape.transform)
            {
                if (child.position.y >= _height - 1) return true;
            }
            return false;
        }

        private void PlayRowGlowFX(int idx, int y)
        {
            if (_rowGlowFXs[idx])
            {
                _rowGlowFXs[idx].transform.position = new Vector3(0, y, -2f);
                _rowGlowFXs[idx].Play();
            }
        }

        public void PlayLandShapeFX(Shape shape)
        {
            byte i = 0;            
            foreach (Transform child in shape.transform)
            {
                _landShapeFXs[i].transform.position = child.position;
                _landShapeFXs[i].Play();

                i++;
            }
        }
    }
}