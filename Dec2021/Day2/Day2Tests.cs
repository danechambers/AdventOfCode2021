using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static Dec2021.Helpers;

namespace Dec2021.Day2
{
    [TestFixture]
    public class Day2Tests
    {
        [Test]
        public async Task Part1()
        {
            var directions = await GetDataUri("Day2/day2input").GetDataAsync();

            var (horizontalMovement, verticalMovement, _) =
                directions
                    .Select(ParseLine)
                    .Aggregate((0, 0, 0), (tracker, movement) =>
                          Update(movement.direction, tracker, movement.distance));

            TestContext.WriteLine($"The answer is {horizontalMovement * verticalMovement}");
        }

        [Test]
        public async Task Part2()
        {
            var directions = await GetDataUri("Day2/day2input").GetDataAsync();

            var (horizontalMovement, verticalMovement, _) =
                directions
                    .Select(ParseLine)
                    .Aggregate((0, 0, 0), (tracker, movement) =>
                          Update(movement.direction, tracker, movement.distance));

            TestContext.WriteLine($"The answer is {horizontalMovement * verticalMovement}");
        }

        private static (string direction, int distance) ParseLine(string line)
        {
            var splitLine = line.Split(' ');
            return (splitLine[0], int.Parse(splitLine[1]));
        }

        private static int Move(string direction, int current, int amount) => direction switch
        {
            "up" => current - amount,
            _ => current + amount
        };

        private static (int horizontal, int depth, int aim) MoveForward(
            (int horizontal, int depth, int aim) tracker, int amount) =>
            (tracker.horizontal + amount, tracker.depth + tracker.aim * amount, tracker.aim);

        private static (int horizontal, int vertical, int aim) Update(
            string direction,
            (int horizontal, int depth, int aim) tracker,
            int amount) => direction switch
            {
                "forward" => MoveForward(tracker, amount),
                _ => (tracker.horizontal, tracker.depth, Move(direction, tracker.aim, amount))
            };
    }
}