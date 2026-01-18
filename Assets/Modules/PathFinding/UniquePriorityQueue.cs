using System;
using System.Collections.Generic;

namespace PathFinding
{
	public class UniquePriorityQueue<TElement, TPriority>
		where TPriority : IComparable<TPriority>
		where TElement : IComparable<TElement>
	{
		private readonly List<(TElement element, TPriority priority)> _elements = new();

		public int Count => _elements.Count;

		#region Operations

		public void Enqueue(TElement item, TPriority priority)
		{
			int index = _elements.FindIndex(e => e.element.Equals(item));

			// If item already exists
			if (index != -1)
			{
				TPriority oldPriority = _elements[index].priority;

				// Priority higher, skip
				if (priority.CompareTo(oldPriority) >= 0)
					return;

				_elements[index] = (item, priority);
			} else
			{
				_elements.Add((item, priority));
				index = Count - 1;
			}

			for (int i = index - 1; i >= 0; i--)
			{
				if (Compare(i, index) <= 0)
					break;

				Swap(i, index);
				index = i;
			}
		}

		public TElement Dequeue() => Dequeue(out _);

		public TElement Dequeue(out TPriority priority)
		{
			if (Count == 0)
				throw new InvalidOperationException("Queue is empty.");

			(TElement element, TPriority priority) element = _elements[0];
			_elements[0] = _elements[Count - 1];
			_elements.RemoveAt(Count - 1);

			int index = 0;

			while (true)
			{
				int leftChild = 2 * index + 1;

				if (leftChild >= Count)
					break;

				int rightChild = leftChild + 1;

				int minChild = rightChild < Count && Compare(rightChild, leftChild) < 0
					? rightChild
					: leftChild;

				if (Compare(index, minChild) <= 0)
					break;

				Swap(index, minChild);
				index = minChild;
			}

			priority = element.priority;
			return element.element;
		}

		#endregion

		#region Helpers

		private int Compare(int i1, int i2) => _elements[i1].priority.CompareTo(_elements[i2].priority);

		private void Swap(int i, int j)
		{
			(_elements[j], _elements[i]) = (_elements[i], _elements[j]);
		}

		#endregion
	}
}