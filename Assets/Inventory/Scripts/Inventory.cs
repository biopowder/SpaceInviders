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

            for (int y = 0; y < item.Size.y; y++)
            {
                for (int x = 0; x < item.Size.x; x++)
                {
                    Vector2Int pos = new(position.x + x, position.y + y);

                    if (_items.ContainsKey(pos))
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
            if (item == null) return false;
            if (!CanAddItem(item, position)) return false;

            _items.Add(position, item);
            OnAdded?.Invoke(item, position);
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
            => throw new NotImplementedException();

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

            Vector2Int position = new Vector2Int(x, y);

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

            return null;
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

            item = GetItem(x, y);
            return item != null;
        }

        /// <summary>
        /// Returns matrix positions of a specified item 
        /// </summary>
        public Vector2Int[] GetPositions(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Vector2Int[] positions = _items.Where(kvp => kvp.Value.Equals(item)).Select(kvp => kvp.Key).ToArray();

            if (positions.Length == 0)
                throw new InvalidOperationException("The specified item is not in the inventory.");

            return positions;
        }


        public bool TryGetPositions(Item item, out Vector2Int[] positions)
        {
            if (item == null)
            {
                positions = null;
                return false;
            }

            positions = _items.Where(kvp => kvp.Value.Equals(item)).Select(kvp => kvp.Key).ToArray();

            return positions.Length > 0;
        }


        /// <summary>
        /// Clears all inventory items
        /// </summary>
        public void Clear()
        {
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
            => throw new NotImplementedException();

        /// <summary>
        /// Reorganizes inventory space to make the free area uniform
        /// </summary>
        public void ReorganizeSpace()
            => throw new NotImplementedException();

        /// <summary>
        /// Copies inventory items to a specified matrix
        /// </summary>
        public void CopyTo(in Item[,] matrix)
            => throw new NotImplementedException();

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