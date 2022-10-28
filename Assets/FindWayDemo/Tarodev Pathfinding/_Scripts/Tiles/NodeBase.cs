using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Tiles {
    public abstract class NodeBase : MonoBehaviour, IComparable<NodeBase>
    {
        [Header("References")] [SerializeField]
        private Color _obstacleColor;

        [SerializeField] private Gradient _walkableColor;
        [SerializeField] protected SpriteRenderer _renderer;
     
        public ICoords Coords;
        public float GetDistance(NodeBase other) => Coords.GetDistance(other.Coords); // Helper to reduce noise in pathfinding
        public bool Walkable { get; private set; }
        private bool _selected;
        private Color _defaultColor;

        public virtual void Init(bool walkable, ICoords coords) {
            Walkable = walkable;

            _renderer.color = walkable ? _walkableColor.Evaluate(Random.Range(0f, 1f)) : _obstacleColor;
            _defaultColor = _renderer.color;

            OnHoverTile += OnOnHoverTile;

            Coords = coords;
            transform.position = Coords.Pos;
        }

        public static event Action<NodeBase> OnHoverTile;
        private void OnEnable() => OnHoverTile += OnOnHoverTile;
        private void OnDisable() => OnHoverTile -= OnOnHoverTile;
        private void OnOnHoverTile(NodeBase selected) => _selected = selected == this;

        protected virtual void OnMouseDown() {
            if (!Walkable) return;
            OnHoverTile?.Invoke(this);
        }

        #region Pathfinding

        [Header("Pathfinding")] [SerializeField]
        private TextMeshPro _fCostText;

        [SerializeField] private TextMeshPro _gCostText, _hCostText;
        public List<NodeBase> Neighbors { get; protected set; }//周围的邻居
        public NodeBase Connection { get; private set; }//上一个节点
        public float G { get; private set; } //起点到当前：每次找邻居会更新, 取当前到这个邻居，和原本设定中的最小
        public float H { get; private set; }//当前到达终点：乐观估计，会绕过障碍物，可能比真实到达的要小
        public float F => G + H; //两者之和

        public abstract void CacheNeighbors();

        public void SetConnection(NodeBase nodeBase) {
            Connection = nodeBase;
        }

        public void SetG(float g) {
            G = g;
            SetText();
        }

        public void SetH(float h) {
            H = h;
            SetText();
        }

        private void SetText() {
            if (_selected) return;
            _gCostText.text = G.ToString();
            _hCostText.text = H.ToString();
            _fCostText.text = F.ToString();
        }

        public void SetColor(Color color) => _renderer.color = color;

        public void RevertTile() {
            _renderer.color = _defaultColor;
            _gCostText.text = "";
            _hCostText.text = "";
            _fCostText.text = "";
        }

        public int CompareTo(NodeBase other)
        {
            if (this.F < other.F || (this.F == other.F && this.H < other.H))
            {
                return -1;
            }
            return 1;
        }

        #endregion
    }
}


public interface ICoords {
    float GetDistance(ICoords other);
    Vector2 Pos { get; set; }
}