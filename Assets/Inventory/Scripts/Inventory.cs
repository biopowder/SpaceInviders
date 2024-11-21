using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public int Count => _items.Count;

        private int _width;
        private int _height;

        private readonly Dictionary<Vector2Int, Item> _items;

        public Inventory(in int width, in int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException(nameof(width), nameof(height));

            _width = width;
            _height = height;

            _items = new Dictionary<Vector2Int, Item>();
        }

        public Inventory(
            in int width,
            in int height,
            params KeyValuePair<Item, Vector2Int>[] items
        ) : this(width, height)
        {
            if (items == null || items.Length == 0) throw new ArgumentNullException(nameof(items));

            foreach (KeyValuePair<Item, Vector2Int> pair in items)
            {
                _items.Add(pair.Value, pair.Key);
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
            if (items == null || !items.Any()) throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Checks for adding an item on a specified position
        /// </summary>
        public bool CanAddItem(Item item, in Vector2Int position)
        {
            if (item == null) return false;
            if (item.Size.x <= 0 || item.Size.y <= 0) throw new ArgumentException(nameof(item));

            if (_items.Values.Any(existingItem => existingItem.Equals(item)))
            {
                return false;
            }

            if (position.x < 0 || position.y < 0 ||
                position.x + item.Size.x > _width ||
                position.y + item.Size.y > _height)
            {
                return false;
            }

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                Vector2Int existingPosition = kvp.Key;
                Item existingItem = kvp.Value;

                int existingLeft = existingPosition.x;
                int existingRight = existingPosition.x + existingItem.Size.x;
                int existingTop = existingPosition.y;
                int existingBottom = existingPosition.y + existingItem.Size.y;

                int newLeft = position.x;
                int newRight = position.x + item.Size.x;
                int newTop = position.y;
                int newBottom = position.y + item.Size.y;

                if (newLeft < existingRight &&
                    newRight > existingLeft &&
                    newTop < existingBottom &&
                    newBottom > existingTop)
                {
                    return false;
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

            _items.Add(position, item);
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
            return item != null && _items.Any(pair => pair.Value.Equals(item));
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
            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                Vector2Int itemPosition = kvp.Key;
                Item item = kvp.Value;

                if (x >= itemPosition.x && x < itemPosition.x + item.Size.x &&
                    y >= itemPosition.y && y < itemPosition.y + item.Size.y)
                {
                    return true;
                }
            }

            return false;
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
            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                Vector2Int itemPosition = kvp.Key;
                Item item = kvp.Value;

                if (x >= itemPosition.x && x < itemPosition.x + item.Size.x &&
                    y >= itemPosition.y && y < itemPosition.y + item.Size.y)
                {
                    return false;
                }
            }

            return true;
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
            if (item == null)
            {
                position = Vector2Int.zero;
                return false;
            }

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                if (kvp.Value.Equals(item))
                {
                    position = kvp.Key;

                    _items.Remove(position);

                    return true;
                }
            }

            position = Vector2Int.zero;
            return false;
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
                throw new IndexOutOfRangeException();

            Vector2Int position = new(x, y);

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                Vector2Int itemPosition = kvp.Key;
                Item item = kvp.Value;

                if (position.x >= itemPosition.x && position.x < itemPosition.x + item.Size.x &&
                    position.y >= itemPosition.y && position.y < itemPosition.y + item.Size.y)
                {
                    return item;
                }
            }

            throw new NullReferenceException();
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

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                if (kvp.Value.Equals(item))
                {
                    Vector2Int startPosition = kvp.Key;
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
            }

            throw new KeyNotFoundException(nameof(item));
        }


        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        {
            if (item == null)
            {
                positions = null;
                return false;
            }

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                if (kvp.Value.Equals(item))
                {
                    Vector2Int startPosition = kvp.Key;
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
            }

            positions = null;
            return false;
        }


        /// <summary>
        /// Clears all inventory items
        /// </summary>
        public void Clear()
        {
            if (_items.Count <= 0) return;

            _items.Clear();
            OnCleared?.Invoke();
        }

        /// <summary>
        /// Returns a count of items with a specified name
        /// </summary>
        public int GetItemCount(string name)
        {
            return _items.Select(pair => pair.Value).Count(item => item.Name == name);
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
            var itemsWithPositions = _items
                .Select(kvp => new { Item = kvp.Value, OriginalPosition = kvp.Key })
                .ToList();

            Clear();

            itemsWithPositions = itemsWithPositions
                .OrderByDescending(i => i.Item.Size.x * i.Item.Size.y)
                .ThenBy(i => i.OriginalPosition.y)
                .ThenBy(i => i.OriginalPosition.x)
                .ToList();

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

            foreach (KeyValuePair<Vector2Int, Item> kvp in _items)
            {
                Vector2Int position = kvp.Key;
                Item item = kvp.Value;

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
            HashSet<Item> uniqueItems = new(_items.Values);
            foreach (var item in uniqueItems)
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