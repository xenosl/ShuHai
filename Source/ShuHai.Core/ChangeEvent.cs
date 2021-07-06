namespace ShuHai
{
    public class ChangeEventArgs<T>
    {
        public T PreviousValue { get; }

        public T CurrentValue { get; }

        public ChangeEventArgs(T previousValue, T currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    public class ChangeEventArgs<TSender, TValue>
    {
        public TSender Sender { get; }

        public TValue PreviousValue { get; }

        public TValue CurrentValue { get; }

        public ChangeEventArgs(TSender sender, TValue previousValue, TValue currentValue)
        {
            Sender = sender;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }
}
