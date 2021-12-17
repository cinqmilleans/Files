using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface IRange<out T>
    {
        T MinValue { get; }
        T MaxValue { get; }
    }

    public interface IRangeLabel : IRange<string>
    {
        RangeLabelDirections Direction { get; }

        new string MinValue { get; }
        new string MaxValue { get; }
    }

    public enum RangeLabelDirections : ushort
    {
        Empty,
        EqualTo,
        LessThan,
        GreaterThan,
        Between,
    }

    public interface IRangeFormatter<T>
    {
        bool CanFormat(IRange<T> range);
        IRangeLabel Format(IRange<T> range);
    }

    public class RangeLabel : IRangeLabel
    {
        public RangeLabelDirections Direction { get; }

        public string MinValue { get; }
        public string MaxValue { get; }

        public RangeLabel(string minValue, string maxValue)
        {
            MinValue = (minValue ?? string.Empty).Trim();
            MaxValue = (maxValue ?? string.Empty).Trim();

            Direction = (MinValue, MaxValue) switch
            {
                ("", "") => RangeLabelDirections.Empty,
                (_, "") => RangeLabelDirections.GreaterThan,
                ("", _) => RangeLabelDirections.LessThan,
                _ when MinValue == MaxValue => RangeLabelDirections.EqualTo,
                _ => RangeLabelDirections.Between,
            };
        }
    }

    public class RangeFormatterCollection<T> : Collection<IRangeFormatter<T>>, IRangeFormatter<T>
    {
        public RangeFormatterCollection() : base()
        {
        }
        public RangeFormatterCollection(IList<IRangeFormatter<T>> formatters) : base(formatters)
        {
        }

        public bool CanFormat(IRange<T> range) => this.Any(formatter => formatter.CanFormat(range));
        public IRangeLabel Format(IRange<T> range) => this.First(formatter => formatter.CanFormat(range)).Format(range);
    }
}
