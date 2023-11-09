using System;
using System.Threading;
using System.Threading.Tasks;

namespace hamsterbyte.DeveloperConsole;

public static class MathCommands{
    private const double DegToRad = Math.PI / 180;
    private const double RadToDeg = 180 / Math.PI;

    [ConsoleCommand(Prefix = "Math", Description = "Print value of x raised to the power of y")]
    private static double Pow(double x, double y) => Math.Pow(x, y);

    [ConsoleCommand(Prefix = "Math", Description = "Print the square root of a value")]
    private static double Sqrt(double val) => Math.Sqrt(val);

    [ConsoleCommand(Prefix = "Math", Description = "Print sine of angle from degrees")]
    private static double Sin(double value) => Math.Round(Math.Sin(value * DegToRad), 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print arc sine in degrees")]
    private static double Asin(double value) => Math.Round(Math.Asin(value) * RadToDeg, 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print cosine of angle from degrees")]
    private static double Cos(double value) => Math.Round(Math.Cos(value * DegToRad), 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print arc cosine in degrees")]
    private static double Acos(double value) => Math.Round(Math.Acos(value) * RadToDeg, 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print tangent of angle from degrees")]
    private static double Tan(double value) => Math.Round(Math.Tan(value * DegToRad), 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print arc tangent in degrees")]
    private static double Atan(double value) => Math.Round(Math.Atan(value) * RadToDeg, 3);

    [ConsoleCommand(Prefix = "Math", Description = "Print angle whose tangent is a quotient of two specified numbers")]
    private static double Atan2(double x, double y) => Math.Round(Math.Atan2(x, y), 3);

    [ConsoleCommand(Prefix = "Math", Description = "Check if a number is prime")]
    private static Task<bool> IsPrime(long number){
        CancellationToken ct = Command.Token;
        return Task.Run(() => {
                ct.ThrowIfCancellationRequested();
                if (number == 2) return true;
                if (number < 2 || number % 2 == 0) return false;
                int squareRoot = (int)Math.Sqrt(number);
                for (long i = 3; i <= squareRoot; i += 2){
                    ct.ThrowIfCancellationRequested();
                    if (number % i == 0) return false;
                }

                return true;
            }, ct
        );
    }

    [ConsoleCommand(Prefix = "Math",
        Description = "Print a list of all prime numbers from start to end. Start is always greater than 2.")]
    private static async Task ListPrimes(long start, long end){
        CancellationToken ct = Command.Token;
        start = start < 2 ? 2 : start;
        for (long i = start; i <= end; i++){
            ct.ThrowIfCancellationRequested();
            if (await IsPrime(i)){
                DC.Print(i);
            }
        }
    }
}