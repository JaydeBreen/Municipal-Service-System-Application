using MunicipalServices.Models;

namespace MunicipalServices.Utils
{
    /// <summary>
    /// A Priority Queue for ServiceRequest objects, implemented using a max-heap.
    /// Higher priority items are dequeued first.
    /// </summary>
    public class ServiceRequestPriorityQueue
    {
        private readonly List<ServiceRequest> _heap = new List<ServiceRequest>();

        public int Count => _heap.Count;

        /// <summary>
        /// Adds a service request to the queue based on its priority.
        /// </summary>
        /// <param name="request">The service request to add.</param>
        public void Enqueue(ServiceRequest request)
        {
            // Add the new element to the end of the list
            _heap.Add(request);

            // "Sift up" the new element to its correct position
            SiftUp(_heap.Count - 1);
        }

        /// <summary>
        /// Removes and returns the highest-priority service request from the queue.
        /// </summary>
        /// <returns>The highest-priority service request.</returns>
        public ServiceRequest Dequeue()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            // The highest-priority item is always at the root (index 0)
            var highestPriorityItem = _heap[0];

            // Move the last element to the root
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            // "Sift down" the new root to its correct position
            if (_heap.Count > 0)
            {
                SiftDown(0);
            }

            return highestPriorityItem;
        }

        /// <summary>
        /// Returns the highest-priority item without removing it.
        /// </summary>
        public ServiceRequest Peek()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }
            return _heap[0];
        }

        private void SiftUp(int index)
        {
            if (index == 0) return;

            int parentIndex = (index - 1) / 2;
            // If the child is higher priority than the parent, swap them
            if (Compare(_heap[index], _heap[parentIndex]) > 0)
            {
                Swap(index, parentIndex);
                SiftUp(parentIndex);
            }
        }

        private void SiftDown(int index)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int largest = index;

            // If the left child is larger than the current largest
            if (leftChildIndex < _heap.Count && Compare(_heap[leftChildIndex], _heap[largest]) > 0)
            {
                largest = leftChildIndex;
            }

            // If the right child is larger than the current largest
            if (rightChildIndex < _heap.Count && Compare(_heap[rightChildIndex], _heap[largest]) > 0)
            {
                largest = rightChildIndex;
            }

            // If the largest is not the current index, swap and continue sifting down
            if (largest != index)
            {
                Swap(index, largest);
                SiftDown(largest);
            }
        }

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }

        /// <summary>
        /// Compares two service requests.
        /// Returns > 0 if r1 is higher priority, < 0 if r2 is higher, and 0 if equal.
        /// </summary>
        private int Compare(ServiceRequest r1, ServiceRequest r2)
        {
            var priority1 = GetPriority(r1);
            var priority2 = GetPriority(r2);

            if (priority1 != priority2)
            {
                return priority1.CompareTo(priority2);
            }

            // If priorities are the same, older requests get higher priority
            return r2.SubmittedDate.CompareTo(r1.SubmittedDate);
        }

        private enum RequestPriority { Low = 0, Medium = 1, High = 2, Critical = 3 }

        private static RequestPriority GetPriority(ServiceRequest request)
        {
            return request.Category switch
            {
                "Water Leak" => RequestPriority.Critical,
                "Illegal Dumping" => RequestPriority.High,
                "Street Light Out" => RequestPriority.Medium,
                "Pothole" => RequestPriority.Medium,
                "Noise Complaint" => RequestPriority.Low,
                _ => RequestPriority.Low,
            };
        }
    }
}