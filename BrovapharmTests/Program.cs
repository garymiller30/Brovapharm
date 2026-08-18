using System;
using System.Linq;
using models.Service;

static class Program
{
    static int Main()
    {
        try
        {
            ShouldParseValidRowsAndSkipInvalidOnes();
            ShouldReturnEmptyForBlankInput();
            Console.WriteLine("All tests passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void ShouldParseValidRowsAndSkipInvalidOnes()
    {
        var rawText = "1\tAspirin\tN-1\r\nabc\tBroken\tN-2\r\n2\tParacetamol\tN-3";
        var result = PreparatParser.Parse(rawText);

        if (result.Count != 2)
            throw new Exception($"Expected 2 items, got {result.Count}.");

        if (result[0].Id != 1 || result[0].Name != "Aspirin" || result[0].Number != "N-1")
            throw new Exception("First parsed item does not match expected values.");

        if (result[1].Id != 2 || result[1].Name != "Paracetamol" || result[1].Number != "N-3")
            throw new Exception("Second parsed item does not match expected values.");
    }

    private static void ShouldReturnEmptyForBlankInput()
    {
        var result = PreparatParser.Parse(string.Empty);
        if (result.Any())
            throw new Exception("Expected empty result for blank input.");
    }
}
