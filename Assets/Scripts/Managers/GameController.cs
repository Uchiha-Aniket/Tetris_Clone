using Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        private Board _board;
        private Spawner _spawner;
        private GhostHandler _ghostHandler;
        private SoundManager _soundManager;
        private ScoreManager _scoreManager;
        private ShapeHolder _shapeHolder;

        [SerializeField] private GameObject _replayUIPopup;
        [SerializeField] private GameObject _pauseUIPopup;
        [SerializeField] private IconToggle _rotateToggle;

        [Header("Speed")]
        [SerializeField, Range(.1f, 1f)] private float _fallingSpeed = .5f; // drop interval
        private float _timeToFall;
        [SerializeField, Range(.1f, 1f)] private float _horizontalMoveSpeed = .5f;
        private float _timeToMoveSideway;
        [SerializeField, Range(.01f, 1f)] private float _downMoveSpeed = .5f;
        private float _timeToMoveDown;
        [SerializeField, Range(.1f, 1f)] private float _rotateSpeed = .1f;
        private float _timeToRotate;

        [Header("Input Keys")]
        [SerializeField] private KeyCode _moveRightInput;
        [SerializeField] private KeyCode _moveLeftInput;
        [SerializeField] private KeyCode _moveDownInput;
        [SerializeField] private KeyCode _rotateInput;
        [SerializeField] private KeyCode _rotateToggleInput;
        [SerializeField] private KeyCode _holdInput;

        private Shape _activeShape;
        private bool _gameOver;
        private bool _isRotClockwise;
        private bool _isPaused;

        private enum SwipeDirection { none, left, right, up, down }
        private SwipeDirection _dragDir;
        private SwipeDirection _swipeDir;

        [Header("Mobile Inputs")]
        private float _timeToNextDrag;
        private float _timeToNextSwipe;
        [SerializeField, Range(.05f, 1f)] private float _minDragTime = .1f;
        [SerializeField, Range(.05f, 1f)] private float _minSwipeTime = .3f;
        private bool _didTap;

        // ----

        private void Awake()
        {
            _board = FindObjectOfType<Board>();
            _spawner = FindObjectOfType<Spawner>();
            _ghostHandler = FindObjectOfType<GhostHandler>();
            _soundManager = FindObjectOfType<SoundManager>();
            _scoreManager = FindObjectOfType<ScoreManager>();
            _shapeHolder = FindObjectOfType<ShapeHolder>();
        }

        private void OnEnable()
        {
            TouchController.DragEvent += DragHandler;
            TouchController.SwipeEvent += SwipeHandler;
            TouchController.TapEvent += TapHandler;
        }

        private void OnDisable()
        {
            TouchController.DragEvent -= DragHandler;
            TouchController.SwipeEvent -= SwipeHandler;
            TouchController.TapEvent -= TapHandler;
        }

        private void Start()
        {
            if (!_board) Debug.LogError(" Board is not defined!");
            if (!_spawner) Debug.LogError("Spawner is not defined!");
            else _activeShape = _spawner.SpawnShape();
        }

        private void Update()
        {
            if (!_board || !_spawner || !_activeShape || _gameOver || !_soundManager) return;
            PlayerInput();
        }

        private void LateUpdate()
        {
            _ghostHandler.SpawnGhost(_activeShape, _board);
        }

        // ----

        private void PlayerInput()
        {
            if (!_spawner || !_board) return;

#if UNITY_EDITOR
            if ((Input.GetKeyDown(_moveRightInput)))
#else
            if ((_dragDir == SwipeDirection.right && Time.time >= _timeToNextDrag) || 
                (_swipeDir == SwipeDirection.right && Time.time >= _timeToNextSwipe))
#endif
            {
                MoveRight();                
            }
#if UNITY_EDITOR
            else if ((Input.GetKeyDown(_moveLeftInput)))
#else
            else if ((_dragDir == SwipeDirection.left && Time.time >= _timeToNextDrag) || 
                (_swipeDir == SwipeDirection.left && Time.time >= _timeToNextSwipe))
#endif
            {
                MoveLeft();
            }
#if UNITY_EDITOR
            else if (Input.GetKeyDown(_rotateInput) && Time.time >= _timeToRotate)
#else
            else if ((_swipeDir == SwipeDirection.up && Time.time >= _timeToNextSwipe) || _didTap)
#endif
            {
                Rotate();
            }
#if UNITY_EDITOR
            else if ((Input.GetKey(_moveDownInput) && Time.time >= _timeToMoveDown) || (Time.time >= _timeToFall))
#else
            else if (_dragDir == SwipeDirection.down && Time.time >= _timeToNextDrag || Time.time >= _timeToFall)
#endif
            {
                MoveDown();
            }
            else if (Input.GetKeyDown(_rotateToggleInput)) OnClick_RotationToggleButton();
            else if (Input.GetKeyDown(_holdInput)) OnClick_HoldShapeButton();

#if PLATFORM_ANDROID
            _dragDir = SwipeDirection.none;
            _swipeDir = SwipeDirection.none;
            _didTap = false;
#endif
        }

        private void MoveDown()
        {
            _timeToMoveDown = Time.time + _downMoveSpeed;
            _timeToFall = Time.time + _fallingSpeed;
            _activeShape.MoveDown();

            if (!_board.IsValidPosition(_activeShape))
            {
                if (_board.IsGameOver(_activeShape))
                {
                    GameOver();
                }
                else LandShape();
            }
        }

        private void Rotate()
        {
#if PLATFORM_ANDROID
            _didTap = false;
            _timeToNextSwipe = Time.time + _minSwipeTime;
#else
            _timeToRotate = Time.time + _rotateSpeed;
#endif
            _activeShape.RotateClockwise(_isRotClockwise);
            if (!_board.IsValidPosition(_activeShape))
            {
                _activeShape.RotateClockwise(!_isRotClockwise);
                _soundManager.PlaySoundFx(_soundManager._errorSound, .5f);
            }
            else _soundManager.PlaySoundFx(_soundManager._moveSound, .7f);
        }

        private void MoveLeft()
        {
#if PLATFORM_ANDROID
            _timeToNextDrag = Time.time + _minDragTime;
            _timeToNextSwipe = Time.time + _minSwipeTime;
#else
            _timeToMoveSideway = Time.time + _horizontalMoveSpeed;
#endif
            _activeShape.MoveLeft();
            if (!_board.IsValidPosition(_activeShape))
            {
                _activeShape.MoveRight();
                _soundManager.PlaySoundFx(_soundManager._errorSound, .5f);
            }
            else _soundManager.PlaySoundFx(_soundManager._moveSound, .7f);
        }

        private void MoveRight()
        {
#if PLATFORM_ANDROID
            _timeToNextDrag = Time.time + _minDragTime;
            _timeToNextSwipe = Time.time + _minSwipeTime;
#else
            _timeToMoveSideway = Time.time + _horizontalMoveSpeed;
#endif
            _activeShape.MoveRight();
            if (!_board.IsValidPosition(_activeShape))
            {
                _activeShape.MoveLeft();
                _soundManager.PlaySoundFx(_soundManager._errorSound, .5f);
            }
            else _soundManager.PlaySoundFx(_soundManager._moveSound, .7f);
        }

        private void GameOver()
        {
            _activeShape.MoveUp();
            _gameOver = true;
            _replayUIPopup.SetActive(true);
            _soundManager.PlaySoundFx(_soundManager._gameOverSound);
            _soundManager.PlaySoundFx(_soundManager.gameOverVocal);
            if (_soundManager.musicEnabled) _soundManager.ToggleBackgroundMusic();
        }

        private void LandShape()
        {
            _soundManager.PlaySoundFx(_soundManager._dropSound, .7f);
            _ghostHandler.Reset();
            _shapeHolder.CanRelease = true;

            _activeShape.MoveUp();
            _board.StoreShapeToGrid(_activeShape);
            _board.PlayLandShapeFX(_activeShape);

            _activeShape = _spawner.SpawnShape();

            _timeToRotate = Time.time;
            _timeToMoveDown = Time.time;
            _timeToMoveSideway = Time.time;

            _board.StartCoroutine("ClearAllRows");
            if (_board.completedRows > 0)
            {
                _scoreManager.ScoreLines(_board.completedRows);
                if (_board.completedRows > 1) _soundManager.PlayRowClearingVocal();
                _soundManager.PlaySoundFx(_soundManager._clearRowSound);
            }
        }

        public void OnClick_ReplayButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnClick_PauseButton()
        {
            _isPaused = !_isPaused;
            _pauseUIPopup.SetActive(_isPaused);
            Time.timeScale = (_isPaused) ? 0f : 1f;
        }

        public void OnClick_RotationToggleButton()
        {
            _isRotClockwise = !_isRotClockwise;
            _rotateToggle.Toggle(_isRotClockwise);
        }

        public void OnClick_HoldShapeButton()
        {
            if (!_shapeHolder.IsShapeHeld())
            {
                _shapeHolder.Hold(_activeShape);
                _activeShape = _spawner.SpawnShape();
                _ghostHandler.Reset();

                _soundManager.PlaySoundFx(_soundManager._holdSound);
            }
            else if (_shapeHolder.CanRelease)
            {
                var shape = _activeShape;
                _activeShape = _shapeHolder.Release();
                _activeShape.transform.position = _spawner.transform.position;
                _shapeHolder.Hold(shape);
                _ghostHandler.Reset();

                _soundManager.PlaySoundFx(_soundManager._holdSound);
            }
            else _soundManager.PlaySoundFx(_soundManager._errorSound);
        }

        private void DragHandler(Vector2 dragMovement)
        {
            _dragDir = GetSwipeDirection(dragMovement);
        }

        private void SwipeHandler(Vector2 swipeMovement)
        {
            _swipeDir = GetSwipeDirection(swipeMovement);
        }

        private void TapHandler(Vector2 tapMovement)
        {
            _didTap = true;
        }

        private SwipeDirection GetSwipeDirection(Vector2 swipeMovement)
        {
            SwipeDirection swipeDir = SwipeDirection.none;

            if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            {
                swipeDir = swipeMovement.x >= 0 ? SwipeDirection.right : SwipeDirection.left;
            }
            else
            {
                swipeDir = swipeMovement.y >= 0 ? SwipeDirection.up : SwipeDirection.down;
            }

            return swipeDir;
        }
    }
}