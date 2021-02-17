using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.Edit
{
    public sealed class UndoRedo
    {
        // Change object
        // Do: Record (state 1) Change (to state 2), Record (state 2)
        // Undo: Restore (to state 1)
        // Redo: Restore (to state 2)

        public static UndoRedo Default { get; } = new UndoRedo();

        public interface ICommand
        {
            /// <summary>
            ///     Performs the operation that need to undo/redo.
            /// </summary>
            /// <param name="objects">Objects that is going to change during the operation.</param>
            /// <returns>Objects that has changed by the operation.</returns>
            IEnumerable<object> Do(IEnumerable<object> objects);

            void Redo();

            void Undo();
        }

//        public sealed class Command<TArgs> : ICommand
//        {
//            public static Command<TArgs> Empty { get; } = new Command<TArgs>(null, default);
//
//            public Action<TArgs> Action { get; }
//
//            public TArgs Arguments { get; }
//
//            public Command(Action<TArgs> action, TArgs args)
//            {
//                Action = action;
//                Arguments = args;
//            }
//
//            public IEnumerable<object> Execute()
//            {
//                Action?.Invoke(Arguments);
//                return Array.Empty<object>();
//            }
//        }

        public int UndoCount => _undoStack.Count;

        public int RedoCount => _redoStack.Count;

//        public void Do<TRedoArgs, TUndoArgs>(
//            Action<TRedoArgs> redoAction, TRedoArgs redoArgs, Action<TUndoArgs> undoAction, TUndoArgs undoArgs,
//            IEnumerable<object> contexts = null)
//        {
//            var redoCmd = new Command<TRedoArgs>(redoAction, redoArgs);
//            var undoCmd = new Command<TUndoArgs>(undoAction, undoArgs);
//            Do(redoCmd, undoCmd, contexts);
//        }

        public void Do(ICommand command, IEnumerable<object> objects = null)
        {
            _redoStack.Clear();

            var op = new Operation(command);
            op.Do(objects);
            _undoStack.Push(op);
        }

        public bool Undo()
        {
            if (_undoStack.Count == 0)
                return false;

            var op = _undoStack.Pop();
            op.Undo();
            _redoStack.Push(op);

            return true;
        }

        public bool Redo()
        {
            if (_redoStack.Count == 0)
                return false;

            var op = _redoStack.Pop();
            op.Redo();
            _undoStack.Push(op);

            return true;
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        private readonly Stack<Operation> _undoStack = new Stack<Operation>();
        private readonly Stack<Operation> _redoStack = new Stack<Operation>();

        private sealed class Operation
        {
            public Operation(ICommand command)
            {
                _command = command;

                _dataConvertSettings = new ObjectDataConvertSettings();
                _dataSession = new ObjectDataConvertSession();
            }

            public void Do(IEnumerable<object> objects)
            {
                _stateBeforeDo = new State(objects);
                var changedObjects = _command.Do(objects);
                _stateAfterDo = new State(changedObjects);
                _currentObjects = changedObjects.ToArray();
            }

            public void Undo() { _command.Undo(); }

            public void Redo() { _command.Redo(); }

            private readonly ICommand _command;

            private ObjectDataConvertSettings _dataConvertSettings;
            private ObjectDataConvertSession _dataSession;

            private object[] _currentObjects;
            private State _stateBeforeDo;
            private State _stateAfterDo;
        }

        private sealed class State : IEquatable<State>
        {
            public IReadOnlyList<Data> DataList { get; }

            public IReadOnlyDictionary<object, Data> DataDict { get; }

            public State(IEnumerable<object> objects)
            {
                var dataList = new List<Data>();
                var dataDict = new Dictionary<object, Data>();
                foreach (var obj in objects)
                {
                    var data = Data.ToData(obj);
                    dataList.Add(data);
                    dataDict.Add(obj, data);
                }
                DataList = dataList;
                DataDict = dataDict;

                _hashCode = CalculateHashCode(DataList);
            }

            public State(IEnumerable<KeyValuePair<object, Data>> dataPairs)
            {
                DataList = dataPairs.Select(p => p.Value).ToList();
                DataDict = dataPairs.ToDictionary(p => p.Key, p => p.Value);

                _hashCode = CalculateHashCode(DataList);
            }

            public void Exclude(State other) { }

            public bool Equals(State other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return DataList.SequenceEqual(other.DataList);
            }

            public override bool Equals(object obj) { return obj is State other && Equals(other); }

            public override int GetHashCode() { return _hashCode; }

            private readonly int _hashCode;

            private static int CalculateHashCode(IReadOnlyList<Data> dataList)
            {
                return HashCode.Combine(dataList.Select(d => d.GetHashCode()));
            }
        }
    }
}