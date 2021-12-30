using System;
using System.Collections.Generic;
using System.Linq;
using Dec2021.Day5.LineSegments;
using Dec2021.Day5.Records;

namespace Dec2021.Day5;

public static class Day5Extensions
{
    public static Func<LineEndPoints, bool> AnyConsideration(params Func<LineEndPoints, bool>[] filters) =>
        segment => filters.Any(filter => filter(segment));

    public static bool ConsiderLinesOnXOrYAxis(LineEndPoints segment) =>
        segment.Start != segment.End &&
        (segment.Start.x == segment.End.x || segment.Start.y == segment.End.y);

    public static bool Consider45DegreeAxis(LineEndPoints segment) =>
        segment.Start != segment.End
        && (segment.Start.x == segment.Start.y 
        && segment.End.x == segment.End.y
        || segment.Start.x == segment.End.y
        && segment.Start.y == segment.End.x);

    public static IEnumerable<LineEndPoints> ReadLinePoints(
            this IEnumerable<string> data,
            Func<string, string[]> splitDataLine,
            Func<string[], LineEndPoints> createLineSegment) =>
        data.Select(splitDataLine).Select(createLineSegment);

    public static string[] SplitLineOnArrow(this string line)
    {
        var charIterator = line.GetEnumerator();
        var pointChars = new List<char>();
        var lineEndPoints = new string[2];

        while (charIterator.MoveNext())
        {
            if (charIterator.Current == '-')
            {
                lineEndPoints[0] = new string(pointChars.ToArray()).Trim();
                pointChars = new List<char>();
                continue;
            }

            if (charIterator.Current == '>')
                continue;

            pointChars.Add(charIterator.Current);
        }

        lineEndPoints[1] = new string(pointChars.ToArray()).Trim();

        return lineEndPoints;
    }

    public static LineEndPoints CreateLineEndPoints(
        this IReadOnlyList<string> data,
        Func<string, Point> createLinePoint) =>
        new(createLinePoint(data[0]), createLinePoint(data[1]));

    public static Point CreateLinePoint(this string dataPoint)
    {
        var dataPoints = dataPoint.Split(',').Select(point => int.Parse(point)).ToArray();
        return new(dataPoints[0], dataPoints[1]);
    }

    public static ILineSegment CreateLineSegment(this LineEndPoints endPoints) => endPoints switch
    {
        var (startPoint, endPoint)
            when startPoint.x == endPoint.x && endPoint.y > startPoint.y =>
                LineSegment<YAxisPositiveDirection>.Factory(endPoints),
        var (startPoint, endPoint) 
            when startPoint.x == endPoint.x && startPoint.y > endPoint.y =>
                LineSegment<YAxisNegativeDirection>.Factory(endPoints),
        var (startPoint, endPoint) 
            when startPoint.y == endPoint.y && endPoint.x > startPoint.x =>
                LineSegment<XAxisPositiveDirection>.Factory(endPoints),
        var (startPoint, endPoint) 
            when startPoint.y == endPoint.y && startPoint.x > endPoint.x =>
                LineSegment<XAxisNegativeDirection>.Factory(endPoints),
        _ => new NoLineSegment()
    };
}