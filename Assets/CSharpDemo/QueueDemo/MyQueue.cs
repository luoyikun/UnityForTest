// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
/*=============================================================================


































using System;
using System.Security.Permissions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;


namespace System.Collections.My
{

    // A simple Queue of objects.  Internally it is implemented as a circular
    // buffer, so Enqueue can be O(n).  Dequeue is O(1).
    //[DebuggerTypeProxy(typeof(System.Collections.Queue.QueueDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [System.Runtime.InteropServices.ComVisible(true)]
    {
        private int _tail;       // Last valid element in the queue
        private int _size;       // Number of elements.
        private int _growFactor; // 100 == 1.0, 130 == 1.3, 200 == 2.0
        private int _version;

        private const int _MinimumGrow = 4;

        // Creates a queue with room for capacity objects. The default initial
        // capacity and grow factor are used.
        public Queue()
: this(32, (float)2.0)
        {

        // Creates a queue with room for capacity objects. The default grow factor
        // is used.
        //
        public Queue(int capacity)
: this(capacity, (float)2.0)
        {

        // Creates a queue with room for capacity objects. When full, the new
        // capacity is set to the old capacity * growFactor.
        //
        public Queue(int capacity, float growFactor)
        {

            _array = new Object[capacity];

        // Fills a Queue with the elements of an ICollection.  Uses the enumerator
        // to get each of the elements.
        //
        public Queue(ICollection col) : this((col == null ? 32 : col.Count))


        public virtual int Count
        {

        public virtual Object Clone()
        {

            int numToCopy = _size;

            q._version = _version;

        public virtual bool IsSynchronized
        {

        public virtual Object SyncRoot
        {
            {
                if (_syncRoot == null)
                {
                }
            }

        // Removes all Objects from the queue.
        public virtual void Clear()
        {
            {

            _head = 0;

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
        // 
        public virtual void CopyTo(Array array, int index)

            int numToCopy = _size;

        // Adds obj to the tail of the queue.
        //
        public virtual void Enqueue(Object obj)
        {
            {
                {

            _array[_tail] = obj;

        // GetEnumerator returns an IEnumerator over this Queue.  This
        // Enumerator will support removing.
        // 
        public virtual IEnumerator GetEnumerator()

        // Removes the object at the head of the queue and returns it. If the queue
        // is empty, this method simply returns null.
        public virtual Object Dequeue()
        {

            Object removed = _array[_head];

        // Returns the object at the head of the queue. The object remains in the
        // queue. If the queue is empty, this method throws an 
        // InvalidOperationException.
        public virtual Object Peek()
        {

            return _array[_head];

        // Returns a synchronized Queue.  Returns a synchronized wrapper
        // class around the queue - the caller must not use references to the
        // original queue.
        // 
        //[HostProtection(Synchronization = true)]

        // Returns true if the queue contains at least one object equal to obj.
        // Equality is determined using obj.Equals().
        //
        // Exceptions: ArgumentNullException if obj == null.
        public virtual bool Contains(Object obj)
        {

            while (count-- > 0)
            {
                {
                else if (_array[index] != null && _array[index].Equals(obj))
                {

            return false;

        internal Object GetElement(int i)

        // Iterates over the objects in the queue, returning an array of the
        // objects in the Queue, or an empty array if the queue is empty.
        // The order of elements in the array is first in to last in, the same
        // order produced by successive calls to Dequeue.
        public virtual Object[] ToArray()

            if (_head < _tail)
            {
            else
            {

            return arr;


        // PRIVATE Grows or shrinks the buffer to hold capacity objects. Capacity
        // must be >= _size.
        private void SetCapacity(int capacity)
        {
            {
                //���ͷ��ǰ��
                if (_head < _tail)
                {
                //β��ǰ��ͷ�ں��м���null
                else
                {

            _array = newarray;
            _version++;

        public virtual void TrimToSize()


        // Implements a synchronization wrapper around a queue.
        [Serializable]

            internal SynchronizedQueue(Queue q)
            {

            public override bool IsSynchronized
            {

            public override Object SyncRoot
            {
                {

            public override int Count
            {
                get
                {
                    lock (root)
                    {
                }

            public override void Clear()
            {
                {

            public override Object Clone()
            {
                {

            public override bool Contains(Object obj)
            {
                {

            public override void CopyTo(Array array, int arrayIndex)
            {
                {

            public override void Enqueue(Object value)
            {
                {

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override Object Dequeue()
            {
                {

            public override IEnumerator GetEnumerator()
            {
                {

            [SuppressMessage("Microsoft.Contracts", "CC1055")]  // Thread safety problems with precondition - can't express the precondition as of Dev10.
            public override Object Peek()
            {
                {

            public override Object[] ToArray()
            {
                {

            public override void TrimToSize()
            {
                lock (root)
                {


        // Implements an enumerator for a Queue.  The enumerator uses the
        // internal version number of the list to ensure that no modifications are
        // made to the list while an enumeration is in progress.
        [Serializable]

            internal QueueEnumerator(Queue q)
            {

            public Object Clone()

            public virtual bool MoveNext()
            {

                if (_index < 0)
                {
                    currentElement = _q._array;

                currentElement = _q.GetElement(_index);

                if (_index == _q._size)

            public virtual Object Current
            {
                {

            public virtual void Reset()
            {
                else
                    _index = 0;

        internal class QueueDebugView
        {

            public QueueDebugView(Queue queue)
            {

                this.queue = queue;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            {
                get
                {
    }