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

public class NoLineSegment : ILineSegment
{
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
    public DiagonalDirection(LineEndPoints lineEndPoints) : base(lineEndPoints)
    {
    }

    protected override IEnumerable<Point> LineSegmentPoints => DiagonalIterator();

    private IEnumerable<Point> DiagonalIterator()
    {
        yield break;
        // int xDirectionChange(){
        //     if(StartPoint.x > EndPoint.x)
        //         return StartPoint.x+1;
        //     else
        //         return 
        // }
        // while(true)
        // {
            
        // }
    }
}