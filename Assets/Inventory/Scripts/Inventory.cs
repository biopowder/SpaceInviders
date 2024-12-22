using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

// ReSharper disable NotResolvedInText

namespace Inventories
{
    public sealed class Inventory : IEnumerable<Item>
    {
        public event Action<Item, Vector2Int> OnAdded;
        public event Action<Item, Vector2Int> OnRemoved;
        public event Action<Item, Vector2Int> OnMoved;
        public event Action OnCleared;

        public int Width => _width;
        public int Height => _height;
        public int Count => _itemsPositions.Count;

        private int _width;
        private int _height;

        private readonly Item[,] _items;
        private readonly Dictionary<Item, Vector2Int> _itemsPositions;
        // private readonly Dictionary<Vector2Int, Item> _items;

        public Inventory(in int width, in int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width), nameof(height));

            _width = width;
            _height = height;

            _items = new Item[width, height];
            _itemsPositions = new Dictionary<Item, Vector2Int>();
        }

        public Inventory(
            in int width,
            in int height,
            params KeyValuePair<Item, Vector2Int>[] items
        ) : this(width, height)
        {
            if (items == null || items.Length == 0)
                throw new ArgumentNullException(nameof(items));

            foreach (KeyValuePair<Item, Vector2Int> pair in items)
            {
                if (!CanAddItem(pair.Key, pair.Value))
                    throw new ArgumentException($"Cannot add item {pair.Key.Name} at position {pair.Value}.");

                AddItem(pair.Key, pair.Value);
            }
        }

        public Inventory(
            in int width,
            in int height,
            params Item[] items
        ) : this(width, height)
        {
            if (items == null || items.Length == 0) throw new ArgumentNullException(nameof(items));
        }

        public Inventory(
            in int width,
            in int height,
            in IEnumerable<KeyValuePair<Item, Vector2Int>> items
        ) : this(width, height)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
        }

        public Inventory(
            in int width,
            in int height,
            in IEnumerable<Item> items
        ) : this(width, height)
        {
            if (items == null || !Enumerable.Any(items)) throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Checks for adding an item on a specified position
        /// </summary>
        public bool CanAddItem(Item item, Vector2Int position)
        {
            if (item == null)
            {
                return false;
            }

            if (item.Size.x <= 0 || item.Size.y <= 0)
            {
                throw new ArgumentException("Item size must be greater than zero.", nameof(item));
            }

            if (_itemsPositions.ContainsKey(item))
            {
                return false;
            }

            if (position.x < 0 || position.y < 0 ||
                position.x + item.Size.x > _width || position.y + item.Size.y > _height)
            {
                return false;
            }

            for (int y = 0; y < item.Size.y; y++)
            {
                for (int x = 0; x < item.Size.x; x++)
                {
                    if (_items[position.x + x, position.y + y] != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanAddItem(in Item item, in int posX, in int posY)
        {
            return CanAddItem(item, new Vector2Int(posX, posY));
        }

        /// <summary>
        /// Adds an item on a specified position if not exists
        /// </summary>
        public bool AddItem(in Item item, in Vector2Int position)
        {
            if (item == null)
            {
                return false;
            }

            if (item.Size.x <= 0 || item.Size.y <= 0)
            {
                throw new ArgumentException("Item size must be greater than zero.", nameof(item));
            }

            if (_itemsPositions.ContainsKey(item))
            {
                return false;
            }

            bool result = AddItemInternal(item, position);
            if (result)
            {
                OnAdded?.Invoke(item, position);
            }

            return result;
        }

        private bool AddItemInternal(in Item item, in Vector2Int position)
        {
            if (item == null) return false;
            if (!CanAddItem(item, position)) return false;

            for (int y = 0; y < item.Size.y; y++)
            {
                for (int x = 0; x < item.Size.x; x++)
                {
                    _items[position.x + x, position.y + y] = item;
                }
            }

            _itemsPositions[item] = position;
            return true;
        }


        public bool AddItem(in Item item, in int posX, in int posY)
        {
            return AddItem(item, new Vector2Int(posX, posY));
        }

        /// <summary>
        /// Checks for adding an item on a free position
        /// </summary>
        public bool CanAddItem(in Item item)
        {
            if (item == null) return false;
            if (item.Size.x <= 0 || item.Size.y <= 0) throw new ArgumentException(nameof(item));

            return FindFreePosition(item.Size, out Vector2Int freePosition) && CanAddItem(item, freePosition);
        }

        /// <summary>
        /// Adds an item on a free position
        /// </summary>
        public bool AddItem(in Item item)
        {
            if (item == null) return false;

            if (item.Size.x <= 0 || item.Size.y <= 0) throw new ArgumentException(nameof(item));

            if (_itemsPositions.ContainsKey(item))
            {
                return false;
            }

            if (FindFreePosition(item.Size, out Vector2Int freePosition))
            {
                return AddItem(item, freePosition);
            }

            return false;
        }

        /// <summary>
        /// Returns a free position for a specified item
        /// </summary>
        public bool FindFreePosition(in Vector2Int size, out Vector2Int freePosition)
        {
            if (size.x <= 0 || size.y <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            for (int y = 0; y <= _height - size.y; y++)
            {
                for (int x = 0; x <= _width - size.x; x++)
                {
                    Vector2Int position = new(x, y);

                    bool canPlace = true;
                    for (int checkY = 0; checkY < size.y; checkY++)
                    {
                        for (int checkX = 0; checkX < size.x; checkX++)
                        {
                            if (IsOccupied(position.x + checkX, position.y + checkY))
                            {
                                canPlace = false;
                                break;
                            }
                        }

                        if (!canPlace)
                            break;
                    }

                    if (canPlace)
                    {
                        freePosition = position;
                        return true;
                    }
                }
            }

            freePosition = Vector2Int.zero;
            return false;
        }

        /// <summary>
        /// Checks if a specified item exists
        /// </summary>
        public bool Contains(Item item)
        {
            return item != null && _itemsPositions.ContainsKey(item);
        }

        /// <summary>
        /// Checks if a specified position is occupied
        /// </summary>
        public bool IsOccupied(in Vector2Int position)
        {
            return IsOccupied(position.x, position.y);
        }

        public bool IsOccupied(in int x, in int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new IndexOutOfRangeException("Position is out of bounds.");

            return _items[x, y] != null;
        }

        /// <summary>
        /// Checks if a position is free
        /// </summary>
        public bool IsFree(in Vector2Int position)
        {
            return IsFree(position.x, position.y);
        }

        public bool IsFree(in int x, in int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new IndexOutOfRangeException("Position is out of bounds.");

            return _items[x, y] == null;
        }

        /// <summary>
        /// Removes a specified item if exists
        /// </summary>
        public bool RemoveItem(in Item item)
            => throw new NotImplementedException();

        public bool RemoveItem(in Item item, out Vector2Int position)
        {
            bool result = RemoveItemInternal(item, out position);
            if (result)
            {
                OnRemoved?.Invoke(item, position);
            }

            return result;
        }

        private bool RemoveItemInternal(in Item item, out Vector2Int position)
        {
            if (item == null || !_itemsPositions.TryGetValue(item, out position))
            {
                position = Vector2Int.zero;
                return false;
            }

            for (int y = 0; y < item.Size.y; y++)
            {
                for (int x = 0; x < item.Size.x; x++)
                {
                    _items[position.x + x, position.y + y] = null;
                }
            }

            _itemsPositions.Remove(item);

            return true;
        }

        /// <summary>
        /// Returns an item at specified position 
        /// </summary>
        public Item GetItem(in Vector2Int position)
        {
            return GetItem(position.x, position.y);
        }

        public Item GetItem(in int x, in int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                throw new IndexOutOfRangeException("Position is out of bounds.");

            Item item = _items[x, y];
            if (item == null)
                throw new NullReferenceException("No item found at the specified position.");

            return item;
        }

        public bool TryGetItem(in Vector2Int position, out Item item)
        {
            return TryGetItem(position.x, position.y, out item);
        }

        public bool TryGetItem(in int x, in int y, out Item item)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                item = null;
                return false;
            }

            try
            {
                item = GetItem(x, y);
            }
            catch (NullReferenceException)
            {
                item = null;
                return false;
            }

            return item != null;
        }

        /// <summary>
        /// Returns matrix positions of a specified item 
        /// </summary>
        public Vector2Int[] GetPositions(Item item)
        {
            if (item == null) throw new NullReferenceException(nameof(item));

            if (!_itemsPositions.TryGetValue(item, out Vector2Int startPosition))
                throw new KeyNotFoundException($"Item not found in inventory: {nameof(item)}");

            List<Vector2Int> positions = new();

            for (int x = 0; x < item.Size.x; x++)
            {
                for (int y = 0; y < item.Size.y; y++)
                {
                    positions.Add(new Vector2Int(startPosition.x + x, startPosition.y + y));
                }
            }

            return positions.ToArray();
        }

        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        {
            if (item == null || !_itemsPositions.TryGetValue(item, out Vector2Int startPosition))
            {
                positions = null;
                return false;
            }

            List<Vector2Int> allPositions = new();

            for (int x = 0; x < item.Size.x; x++)
            {
                for (int y = 0; y < item.Size.y; y++)
                {
                    allPositions.Add(new Vector2Int(startPosition.x + x, startPosition.y + y));
                }
            }

            positions = allPositions.ToArray();
            return true;
        }

        /// <summary>
        /// Clears all inventory items
        /// </summary>
        public void Clear()
        {
            if (_itemsPositions.Count <= 0) return;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _items[x, y] = null;
                }
            }

            _itemsPositions.Clear();

            OnCleared?.Invoke();
        }

        /// <summary>
        /// Returns a count of items with a specified name
        /// </summary>
        // public int GetItemCount(string name)
        // {
        //     if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        //
        //     return Enumerable.Count(_itemsPositions.Keys, item => item.Name == name);
        // }
        public int GetItemCount(string name)
        {
            int count = 0;

            foreach (Item item in _itemsPositions.Keys)
            {
                if ((name == null && item.Name == null) || (name != null && item.Name == name))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Moves a specified item to a target position if it exists
        /// </summary>
        public bool MoveItem(in Item item, in Vector2Int newPosition)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (newPosition.x < 0 || newPosition.x >= _width || newPosition.y < 0 || newPosition.y >= _height)
                return false;

            if (!TryGetPositions(item, out Vector2Int[] positions))
                return false;

            Vector2Int currentPosition = positions[0];

            RemoveItemInternal(item, out _);

            if (CanAddItem(item, newPosition))
            {
                AddItemInternal(item, newPosition);
                OnMoved?.Invoke(item, newPosition);
                return true;
            }

            AddItemInternal(item, currentPosition);
            return false;
        }


        /// <summary>
        /// Reorganizes inventory space to make the free area uniform
        /// </summary>
        public void ReorganizeSpace()
        {
            List<(Item Item, Vector2Int OriginalPosition)> itemsWithPositions = new();
            foreach (KeyValuePair<Item, Vector2Int> entry in _itemsPositions)
            {
                itemsWithPositions.Add((entry.Key, entry.Value));
            }

            Clear();

            itemsWithPositions.Sort((a, b) =>
            {
                int areaA = a.Item.Size.x * a.Item.Size.y;
                int areaB = b.Item.Size.x * b.Item.Size.y;
                return areaB.CompareTo(areaA);
            });

            foreach (var entry in itemsWithPositions)
            {
                if (FindFreePosition(entry.Item.Size, out Vector2Int newPosition))
                {
                    AddItem(entry.Item, newPosition);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Failed to reorganize space: no free position found for an item.");
                }
            }
        }

        /// <summary>
        /// Copies inventory items to a specified matrix
        /// </summary>
        public void CopyTo(in Item[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            if (matrix.GetLength(0) != _height || matrix.GetLength(1) != _width)
                throw new ArgumentException("Matrix dimensions must match inventory dimensions.");

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    matrix[y, x] = null;
                }
            }

            foreach (KeyValuePair<Item, Vector2Int> itemPosition in _itemsPositions)
            {
                var item = itemPosition.Key;
                var position = itemPosition.Value;

                for (int y = 0; y < item.Size.y; y++)
                {
                    for (int x = 0; x < item.Size.x; x++)
                    {
                        matrix[position.x + x, position.y + y] = item;
                    }
                }
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            foreach (Item item in _itemsPositions.Keys)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}