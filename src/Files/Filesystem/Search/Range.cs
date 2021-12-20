namespace Files.Filesystem.Search
{
    public enum RangeDirections : ushort
    {
        None,
        EqualTo,
        GreaterThan,
        LessThan,
        Between,
    }

    public interface IRange<out T>
    {
        RangeDirections Direction { get; }

        T MinValue { get; }
        T MaxValue { get; }
    }

    public struct RangeLabel : IRange<string>
    {
        public static RangeLabel None { get; } = new(string.Empty, string.Empty);

        public RangeDirections Direction { get; }

        public string MinValue { get; }
        public string MaxValue { get; }

        public RangeLabel(string value) : this(value, value) {}
        public RangeLabel(string minValue, string maxValue)
        {
            MinValue = (minValue ?? string.Empty).Trim();
            MaxValue = (maxValue ?? string.Empty).Trim();

            Direction = (MinValue, MaxValue) switch
            {
                ("", "") => RangeDirections.None,
                (_, "") => RangeDirections.GreaterThan,
                ("", _) => RangeDirections.LessThan,
                _ when MinValue == MaxValue => RangeDirections.EqualTo,
                _ => RangeDirections.Between,
            };
        }

        public void Deconstruct(out RangeDirections direction, out string minValue, out string maxValue)
            => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);
    }
}
