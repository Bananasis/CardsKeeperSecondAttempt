using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;


public enum Rotation
{
    Left,
    Up,
    Right,
    Down,
}

public enum FullRotation
{
    LeftDown,
    Left,
    LeftUp,
    Up,
    RightUp,
    Right,
    RightDown,
    Down,
}


//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2008 Solutions Design. All rights reserved.
// http://www.sd.nl
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2008 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//      - Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////


/// <summary>
/// Extension to the normal Dictionary. This class can store more than one value for every key. It keeps a HashSet for every Key value.
/// Calling Add with the same Key and multiple values will store each value under the same Key in the Dictionary. Obtaining the values
/// for a Key will return the HashSet with the Values of the Key. 
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class MultiValueDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiValueDictionary&lt;TKey, TValue&gt;"/> class.
    /// </summary>
    public MultiValueDictionary()
        : base()
    {
    }


    /// <summary>
    /// Adds the specified value under the specified key
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(TKey key, TValue value)
    {
        HashSet<TValue> container = null;
        if (!this.TryGetValue(key, out container))
        {
            container = new HashSet<TValue>();
            base.Add(key, container);
        }

        container.Add(value);
    }


    /// <summary>
    /// Determines whether this dictionary contains the specified value for the specified key 
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if the value is stored for the specified key in this dictionary, false otherwise</returns>
    public bool ContainsValue(TKey key, TValue value)
    {
        bool toReturn = false;
        HashSet<TValue> values = null;
        if (this.TryGetValue(key, out values))
        {
            toReturn = values.Contains(value);
        }

        return toReturn;
    }


    /// <summary>
    /// Removes the specified value for the specified key. It will leave the key in the dictionary.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public bool Remove(TKey key, TValue value)
    {
        HashSet<TValue> container = null;
        bool removed = false;
        if (this.TryGetValue(key, out container))
        {
            removed = container.Remove(value);
            if (container.Count <= 0)
            {
                this.Remove(key);
            }
        }

        return removed;
    }


    /// <summary>
    /// Merges the specified multivaluedictionary into this instance.
    /// </summary>
    /// <param name="toMergeWith">To merge with.</param>
    public void Merge(MultiValueDictionary<TKey, TValue> toMergeWith)
    {
        if (toMergeWith == null)
        {
            return;
        }

        foreach (KeyValuePair<TKey, HashSet<TValue>> pair in toMergeWith)
        {
            foreach (TValue value in pair.Value)
            {
                this.Add(pair.Key, value);
            }
        }
    }


    /// <summary>
    /// Gets the values for the key specified. This method is useful if you want to avoid an exception for key value retrieval and you can't use TryGetValue
    /// (e.g. in lambdas)
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="returnEmptySet">if set to true and the key isn't found, an empty hashset is returned, otherwise, if the key isn't found, null is returned</param>
    /// <returns>
    /// This method will return null (or an empty set if returnEmptySet is true) if the key wasn't found, or
    /// the values if key was found.
    /// </returns>
    public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
    {
        HashSet<TValue> toReturn = null;
        if (!base.TryGetValue(key, out toReturn) && returnEmptySet)
        {
            toReturn = new HashSet<TValue>();
        }

        return toReturn;
    }
}

public struct PreviewData
{
    public Sprite sprite;
    public Sprite additionalSprite;
}

public class ExecutorWithPreview : WaitQueue<(Action, PreviewData)>
{
    private readonly List<(Action, PreviewData)> currentBatch = new List<(Action, PreviewData)>();

    public IEnumerable<PreviewData> GetPreview()
    {
        return heap.PeekAllSorted(true).Select(p => p.Item2).Concat(currentBatch.Select(p => p.Item2));
    }

    public bool ExecuteNext(float deltaTime)
    {
        if (!TryMakeBatch(deltaTime)) return false;
        var (action, _) = currentBatch[^1];
        currentBatch.RemoveAt(currentBatch.Count - 1);
        action.Invoke();
        return true;
    }


    public void Execute(float deltaTime)
    {
        if (currentBatch.Count != 0)
        {
            foreach (var (action, previewData) in ((IEnumerable<(Action, PreviewData)>) currentBatch).Reverse())
            {
                action.Invoke();
            }

            currentBatch.Clear();
            return;
        }

        foreach (var (action, previewData) in Tick(deltaTime))
        {
            action.Invoke();
        }
    }

    public void ExecuteAll(float deltaTime, float executionTime)
    {
        if (executionTime <= 0) return;
        if (currentBatch.Count != 0)
        {
            foreach (var (action, _) in ((IEnumerable<(Action, PreviewData)>) currentBatch).Reverse())
            {
                action.Invoke();
            }

            currentBatch.Clear();
            return;
        }

        while (executionTime > 0 && !heap.IsEmpty())
        {
            executionTime -= deltaTime;
            foreach (var (action, _) in Tick(deltaTime))
            {
                action.Invoke();
            }
        }
    }

    public bool TryMakeBatch(float deltaTime)
    {
        if (currentBatch.Count == 0)
            currentBatch.AddRange(Tick(deltaTime).Reverse());
        if (currentBatch.Count == 0) return false;
        return true;
    }
}

public class Executor : WaitQueue<Action>
{
    public void Execute(float deltaTime)
    {
        foreach (var action in Tick(deltaTime))
        {
            action.Invoke();
        }
    }

    public void ExecuteAll(float deltaTime, float executionTime)
    {
        while (executionTime > 0 && !heap.IsEmpty())
        {
            executionTime -= deltaTime;
            foreach (var action in Tick(deltaTime))
            {
                action.Invoke();
            }
        }
    }
}

public class WaitQueue<T>
{
    protected readonly MinHeap<T> heap = new MinHeap<T>();
    protected float time;

    public IEnumerable<T> Tick(float deltaTime)
    {
        time += deltaTime;
        return heap.PopAll(time);
    }

    public void Add(float duration, T element)
    {
        heap.Add(duration + time, element);
    }

    public void Clear()
    {
        time = 0;
        heap.Clear();
    }
}


public class MinHeap<T>
{
    private readonly List<((float, int), T)> _elements = new();
    private int _size;


    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    private ((float, int), T) GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
    private ((float, int), T) GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
    private ((float, int), T) GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

    private void Swap(int firstIndex, int secondIndex)
    {
        (_elements[firstIndex], _elements[secondIndex]) = (_elements[secondIndex], _elements[firstIndex]);
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public (float, T) Peek()
    {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        var ((time, _), element) = _elements[0];
        return (time, element);
    }

    public (float, T) Pop()
    {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        var result = _elements[0];
        _elements[0] = _elements[_size - 1];
        _size--;

        ReCalculateDown();

        var ((time, _), element) = result;
        return (time, element);
    }

    public void Clear()
    {
        _elements.Clear();
        _ordering = 0;
        _size = 0;
    }

    public IEnumerable<T> PopAll(float maxPriority)
    {
        List<T> elements = new List<T>();
        while (_size > 0 && _elements[0].Item1.Item1 <= maxPriority)
        {
            elements.Add(_elements[0].Item2);
            _elements[0] = _elements[_size - 1];
            _size--;
            ReCalculateDown();
        }

        return elements;
    }

    private int _ordering = 0;

    public void Add(float priority, T element)
    {
        if (_elements.Count == _size)
            _elements.Add(((priority, _ordering++), element));
        else
            _elements[_size] = ((priority, _ordering++), element);
        _size++;

        ReCalculateUp();
    }


    bool Less((float, int) prior1, (float, int) prior2)
    {
        return prior1.Item1 < prior2.Item1 || prior1.Item1 == prior2.Item1 && prior1.Item2 < prior2.Item2;
    }

    private void ReCalculateDown()
    {
        int index = 0;
        while (HasLeftChild(index))
        {
            var smallerIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && Less(GetRightChild(index).Item1, GetLeftChild(index).Item1))
            {
                smallerIndex = GetRightChildIndex(index);
            }

            if (!Less(_elements[smallerIndex].Item1, _elements[index].Item1))
            {
                break;
            }

            Swap(smallerIndex, index);
            index = smallerIndex;
        }
    }

    private void ReCalculateUp()
    {
        var index = _size - 1;
        while (!IsRoot(index) && Less(_elements[index].Item1, GetParent(index).Item1))
        {
            var parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    public IEnumerable<T> PeekAllSorted(bool reversed = false)
    {
        if (reversed)
            return _elements.Take(_size).OrderByDescending((t) => t.Item1.Item1).ThenByDescending((t => t.Item1.Item2))
                .Select((t) => t.Item2);
        return _elements.Take(_size).OrderBy((t) => t.Item1.Item1).ThenBy((t => t.Item1.Item2)).Select((t) => t.Item2);
    }
}

public class StablePriorityComparer : IComparer<(float, int)>
{
    public int Compare((float, int) x, (float, int) y)
    {
        var item1Comparison = x.Item1.CompareTo(y.Item1);
        return item1Comparison != 0 ? item1Comparison : x.Item2.CompareTo(y.Item2);
    }
}

[Serializable]
public struct GridDirection : IEquatable<GridDirection>
{
    public DVector2 start;
    public Rotation rotation;
    [JsonIgnore]
    public DVector2 end => start + RotationUtils.rotations[(int) rotation];
    [JsonIgnore]
    public GridDirection forward => new(end * 2 - start, rotation);


    public GridDirection Rotate(Rotation rot)
    {
        return new GridDirection(start * rot, rot.Add(rotation));
    }

    public GridDirection Translate(DVector2 translation)
    {
        return new GridDirection(start + translation, rotation);
    }

    public GridDirection Reverse()
    {
        return new GridDirection(end, start);
    }

    public GridDirection(DVector2 start, DVector2 end)
    {
        this.start = start;
        rotation = (end - start).ToRotation();
    }

    public GridDirection(DVector2 start, Rotation rotation)
    {
        this.start = start;
        this.rotation = rotation;
    }

    public GridDirection(DVector2 start)
    {
        this.start = start;
        rotation = Rotation.Left;
    }

    public static bool operator ==(GridDirection first, GridDirection second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(GridDirection first, GridDirection second)
    {
        return !(first == second);
    }

    public bool Equals(GridDirection other)
    {
        return start.Equals(other.start) && rotation.Equals(other.rotation);
    }

    public override bool Equals(object obj)
    {
        return obj is GridDirection other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(start, rotation);
    }

    public static GridDirection operator -(GridDirection first, GridDirection second)
    {
        return new GridDirection(first.start - second.start, first.rotation.Subtract(second.rotation));
    }

    public static GridDirection operator /(GridDirection first, GridDirection second)
    {
        return new GridDirection((first.start - second.start) * second.rotation.Reverse(),
            first.rotation.Subtract(second.rotation));
    }

    public static GridDirection operator +(GridDirection first, GridDirection second)
    {
        return new GridDirection(first.start + second.start, first.rotation.Add(second.rotation));
    }

    public static GridDirection operator *(GridDirection first, GridDirection second)
    {
        return new GridDirection(first.start * second.rotation + second.start, first.rotation.Add(second.rotation));
    }

    public static GridDirection operator *(GridDirection first, int multiplier)
    {
        return new GridDirection(first.start * multiplier, first.rotation);
    }
}

[Serializable]
public struct DVector2 : IEquatable<DVector2>
{
    [JsonIgnore]
    public int Magnitude => Mathf.Abs(x) + Mathf.Abs(y);
    public int x;
    public int y;
    [JsonIgnore]
    public Vector2 vector => new(x, y);

    public DVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public DVector2(Rotation rot)
    {
        var dir = RotationUtils.rotations[(int) rot];
        x = dir.Item1;
        y = dir.Item2;
    }

    public DVector2(FullRotation fRot)
    {
        var dir = RotationUtils.fullRotations[(int) fRot];
        x = dir.Item1;
        y = dir.Item2;
    }

    public DVector2((int, int) tuple)
    {
        x = tuple.Item1;
        y = tuple.Item2;
    }

    public DVector2(Vector3 worldPosition)
    {
        x = Mathf.FloorToInt(worldPosition.x + 0.5f);
        y = Mathf.FloorToInt(worldPosition.y + 0.5f);
    }

    public static DVector2 operator /(DVector2 first, GridDirection gd)
    {
        return (first - gd.start) * gd.rotation.Reverse();
    }

    public static DVector2 operator *(DVector2 first, GridDirection gd)
    {
        return first * gd.rotation + gd.start;
    }

    public static DVector2 operator *(DVector2 first, int multiplier)
    {
        return new DVector2(first.x * multiplier, first.y * multiplier);
    }

    public static DVector2 operator -(DVector2 first, DVector2 second)
    {
        return new DVector2(
            first.x - second.x,
            first.y - second.y
        );
    }

    public static DVector2 operator +(DVector2 first, DVector2 second)
    {
        return new DVector2(
            first.x + second.x,
            first.y + second.y
        );
    }

    public static DVector2 operator -(DVector2 first, (int, int) second)
    {
        return new DVector2(
            first.x - second.Item1,
            first.y - second.Item2
        );
    }

    public static bool operator ==(DVector2 first, DVector2 second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(DVector2 first, DVector2 second)
    {
        return !(first == second);
    }

    public static DVector2 operator +(DVector2 first, (int, int) second)
    {
        return new DVector2(
            first.x + second.Item1,
            first.y + second.Item2
        );
    }

    public static DVector2 operator *(DVector2 dir, Rotation rotation)
    {
        return rotation switch
        {
            Rotation.Left => new DVector2(dir.x, dir.y),
            Rotation.Up => new DVector2(dir.y, -dir.x),
            Rotation.Right => new DVector2(-dir.x, -dir.y),
            Rotation.Down => new DVector2(-dir.y, dir.x),
            _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
        };
    }

    public Rotation ToRotation()
    {
        return (x, y) switch
        {
            (-1, 0) => Rotation.Left,
            (0, 1) => Rotation.Up,
            (1, 0) => Rotation.Right,
            (0, -1) => Rotation.Down,
            _ => throw new Exception()
        };
    }

    public Rotation ToRotationOrLeft()
    {
        return (x, y) switch
        {
            (-1, 0) => Rotation.Left,
            (0, 1) => Rotation.Up,
            (1, 0) => Rotation.Right,
            (0, -1) => Rotation.Down,
            _ => Rotation.Left
        };
    }

    public FullRotation ToRotationFullOrLeft()
    {
        var xySign = (Math.Sign(x), Math.Sign(y));
        return xySign switch
        {
            (-1, -1) => FullRotation.LeftDown,
            (-1, 0) => FullRotation.Left,
            (-1, 1) => FullRotation.LeftUp,
            (0, 1) => FullRotation.Up,
            (1, 1) => FullRotation.RightUp,
            (1, 0) => FullRotation.Right,
            (1, -1) => FullRotation.RightDown,
            (0, -1) => FullRotation.Down,
            _ => FullRotation.LeftDown
        };
    }

    public FullRotation ToRotationFull()
    {
        return (x, y) switch
        {
            (-1, -1) => FullRotation.LeftDown,
            (-1, 0) => FullRotation.Left,
            (-1, 1) => FullRotation.LeftUp,
            (0, 1) => FullRotation.Up,
            (1, 1) => FullRotation.RightUp,
            (1, 0) => FullRotation.Right,
            (1, -1) => FullRotation.RightDown,
            (0, -1) => FullRotation.Down,
            _ => throw new Exception()
        };
    }

    public bool Equals(DVector2 other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        return obj is DVector2 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}

public static class RotationUtils
{
    public static Vector2 Rotate(this Vector2 position, Rotation rotation)
    {
        return rotation switch
        {
            Rotation.Left => new Vector2(position.x, position.y),
            Rotation.Up => new Vector2(position.y, -position.x),
            Rotation.Right => new Vector2(-position.x, -position.y),
            Rotation.Down => new Vector2(-position.y, position.x),
            _ => throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null)
        };
    }

    public static readonly (int, int)[] fullRotations =
        {(-1, -1), (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1)};

    public static readonly (int, int)[] rotations = {(-1, 0), (0, 1), (1, 0), (0, -1)};

    public static readonly Quaternion[] rotationsQuaternion =
    {
        Quaternion.Euler(0, 0, -180),
        Quaternion.Euler(0, 0, -270),
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 0, -90),
    };

    public static readonly Quaternion[] fullRotationsQuaternion =
    {
        Quaternion.Euler(0, 0, -135),
        Quaternion.Euler(0, 0, -180),
        Quaternion.Euler(0, 0, -225),
        Quaternion.Euler(0, 0, -270),
        Quaternion.Euler(0, 0, -315),
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 0, -45),
        Quaternion.Euler(0, 0, -90),
    };


    public static readonly (int, int)[] rotationsCross =
        {(-1, 0), (0, 1), (1, 0), (0, -1), (0, 0), (-2, 0), (0, 2), (2, 0), (0, -2)};

    public static Rotation Reverse(this Rotation rot)
    {
        return (Rotation) ((4 - (int) rot) % 4);
    }

    public static Vector2 GetVector(this Rotation rot)
    {
        var xy = rotations[(int) rot];
        return new Vector2(xy.Item1, xy.Item2);
    }

    public static Vector2 GetVector(this FullRotation rot)
    {
        var xy = fullRotations[(int) rot];
        return new Vector2(xy.Item1, xy.Item2);
    }

    public static Rotation Add(this Rotation a, Rotation b)
    {
        return (Rotation) (((int) a + (int) b) % 4);
    }

    public static Rotation Subtract(this Rotation a, Rotation b)
    {
        return (Rotation) (((int) a - (int) b + 4) % 4);
    }

    public static Rotation Add(this Rotation a, int b)
    {
        return (Rotation) (((int) a + b) % 4);
    }

    public static Rotation Subtract(this Rotation a, int b)
    {
        return (Rotation) (((int) a - b + 4) % 4);
    }
}