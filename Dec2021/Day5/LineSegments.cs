using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dec2021.Day5.Records;

namespace Dec2021.Day5.LineSegments;

// A positive direction is away from the origin
// A negative direction is towards the origin

public interface ILineSegment
{
    IEnumerable<Point> Points => Enumerable.Empty<Point>();
}

public sealed class NoLineSegment : ILineSegment
{
    public static NoLineSegment Data => SingletonInstance.Value;

    private static Lazy<NoLineSegment> SingletonInstance =>
        new Lazy<NoLineSegment>(() => new NoLineSegment());

    private NoLineSegment()
    { }
}

public class LineSegment<T> : ILineSegment where T : LineSegmentCoveredPoints
{
    public LineSegment(T segmentDirection)
    {
        Points = segmentDirection;
    }

    public IEnumerable<Point> Points { get; }

    public static LineSegment<T> Factory(LineEndPoints lineEndPonts) =>
        new LineSegment<T>(
            Activator.CreateInstance(typeof(T), lineEndPonts) as T
            ?? throw new InvalidOperationException());
}

public abstract class LineSegmentCoveredPoints : IEnumerable<Point>
{
    protected LineSegmentCoveredPoints(LineEndPoints lineEndPoints)
    {
        (StartPoint, EndPoint) = lineEndPoints;
    }

    protected Point StartPoint { get; }
    protected Point EndPoint { get; }

    protected abstract IEnumerable<Point> LineSegmentPoints { get; }

    public IEnumerator<Point> GetEnumerator() => LineSegmentPoints.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class XAxisPositiveDirection : LineSegmentCoveredPoints
{
    public XAxisPositiveDirection(LineEndPoints linePoints) : base(linePoints)
    {
    }

    protected override IEnumerable<Point> LineSegmentPoints =>
        Enumerable
            .Range(StartPoint.x, EndPoint.x - StartPoint.x + 1)
            .Select(x => new Point(x, StartPoint.y));
}

public class XAxisNegativeDirection : LineSegmentCoveredPoints
{
    public XAxisNegativeDirection(LineEndPoints linePoints) : base(linePoints)
    {
    }

    protected override IEnumerable<Point> LineSegmentPoints =>
        Enumerable
            .Range(EndPoint.x, StartPoint.x - EndPoint.x + 1)
            .Select(x => new Point(x, StartPoint.y));
}

public class YAxisPositiveDirection : LineSegmentCoveredPoints
{
    public YAxisPositiveDirection(LineEndPoints linePoints) : base(linePoints)
    {
    }

    protected override IEnumerable<Point> LineSegmentPoints =>
        Enumerable
            .Range(StartPoint.y, EndPoint.y - StartPoint.y + 1)
            .Select(y => new Point(StartPoint.x, y));
}

public class YAxisNegativeDirection : LineSegmentCoveredPoints
{
    public YAxisNegativeDirection(LineEndPoints linePoints) : base(linePoints)
    {
    }

    protected override IEnumerable<Point> LineSegmentPoints =>
        Enumerable
            .Range(EndPoint.y, StartPoint.y - EndPoint.y + 1)
            .Select(y => new Point(StartPoint.x, y));
}

public class DiagonalDirection : LineSegmentCoveredPoints
{
    private readonly int xIncrement;
    private readonly int yIncrement;

    public DiagonalDirection(LineEndPoints lineEndPoints) : base(lineEndPoints)
    {
        xIncrement = StartPoint.x > EndPoint.x ? -1 : 1;
        yIncrement = StartPoint.y > EndPoint.y ? -1 : 1;
    }

    protected override IEnumerable<Point> LineSegmentPoints => LineSegmentIterator();

    private IEnumerable<Point> LineSegmentIterator()
    {
        var currentPoint = StartPoint;

        do
        {
            yield return currentPoint;
            currentPoint = currentPoint with
            { x = currentPoint.x + xIncrement, y = currentPoint.y + yIncrement };
        } while (currentPoint != EndPoint);

        yield return EndPoint;
    }
}