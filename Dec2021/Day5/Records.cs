namespace Dec2021.Day5.Records;

public record struct Point(int x, int y);

public record LineEndPoints(Point Start, Point End);
