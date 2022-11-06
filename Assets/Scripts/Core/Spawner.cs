using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Shape[] _allShapes;

        [SerializeField] private Transform[] _queueForms;
        private Shape[] _shapeQueue = new Shape[3];     
        private readonly float _queueShapeSize = .5f;

        [SerializeField] private ParticleSystem _spawnFx;

        // ----

        private void Awake()
        {
            InitQueue();
        }

        // ----

        private Shape GetRandomShape()
        {
            var i = Random.Range(0, _allShapes.Length);
            if (_allShapes[i]) return _allShapes[i];
            else
            {
                Debug.LogWarning("Invalid Shape!");
                return null;
            }
        }

        public Shape SpawnShape()
        {
            var shape = GetShapeFromQueue();
            shape.transform.position = transform.position;
            shape.transform.localScale = Vector3.one;

            if (_spawnFx) _spawnFx.Play();
            GrowShape(shape, .5f);

            if (shape) return shape;
            else
            {
                Debug.LogWarning("Invalid Shape!");
                return null;
            }
        }

        private void InitQueue()
        {
            for (byte i = 0; i < _shapeQueue.Length; i++) _shapeQueue[i] = null;
            FillQueue();
        }

        private void FillQueue()
        {
            for (byte i = 0; i < _shapeQueue.Length; i++)
            {
                if (_shapeQueue[i]) continue;

                var shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity);
                shape.transform.position = _queueForms[i].position + shape._queueOffset;
                shape.transform.localScale = new Vector3(_queueShapeSize, _queueShapeSize, _queueShapeSize);

                _shapeQueue[i] = shape;
            }
        }

        private Shape GetShapeFromQueue()
        {
            var topMostShape = _shapeQueue[0];
            _shapeQueue[0] = null;
            UpdateShapeQueuePositions();
            SpawnNewShapeAtBottom();

            return topMostShape;
        }

        private void UpdateShapeQueuePositions()
        {
            for (byte i = 1; i < _shapeQueue.Length; i++)
            {
                _shapeQueue[i - 1] = _shapeQueue[i];
                _shapeQueue[i - 1].transform.position = _queueForms[i - 1].position + _shapeQueue[i - 1]._queueOffset;
            }
        }

        private void SpawnNewShapeAtBottom()
        {
            _shapeQueue[_shapeQueue.Length - 1] = null;
            FillQueue();
        }

        private void GrowShape(Shape shape, float growTime)
        {
            shape.transform.DOScale(1, growTime).From(0).SetEase(Ease.OutBack);
        }
    }
}
