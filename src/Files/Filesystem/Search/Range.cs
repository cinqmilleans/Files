namespace Files.Filesystem.Search
{
    public enum RangeDirections : ushort
    {
        None,
        EqualTo,
        LessThan,
        GreaterThan,
        Between,
    }

    public interface IRange<out T>
    {
        RangeDirections Direction { get; }

        T MinValue { get; }
        T MaxValue { get; }
    }

    public class RangeLabel : IRange<string>
    {
        public RangeDirections Direction { get; }

        public string MinValue { get; }
        public string MaxValue { get; }

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
    }
}
