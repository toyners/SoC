
namespace Jabberwocky.SoC.Library
{
  using System;

  /// <summary>
  /// Provides relative difference between two production factors. Used with List.Sort. This
  /// comparison will return the production factors in  descending order from 8 to 2  
  /// </summary>
  public static class ProductionFactorComparison
  {
    private static int[] normalisedValues = { 0, 2, 4, 6, 8, 0, 9, 7, 5, 3, 1 };

    public static Int32 Compare(int pf1, int pf2)
    {
      if (pf1 < 2 || pf1 > 12 || pf1 == 7)
      {
        throw new ArgumentOutOfRangeException(nameof(pf1), "Value cannot be 7 or less than 2 or greater than 12.");
      }

      if (pf2 < 2 || pf2 > 12 || pf2 == 7)
      {
        throw new ArgumentOutOfRangeException(nameof(pf2), "Value cannot be 7 or less than 2 or greater than 12.");
      }

      if (pf1 == pf2)
      {
        return 0;
      }

      var normalised1 = normalisedValues[pf1 - 2];
      var normalised2 = normalisedValues[pf2 - 2];

      if (normalised1 > normalised2)
      {
        return -1; // Descending order
      }

      return 1;
    }

    internal static bool WithinRange(int production, int brickProductionRangeLower, int brickProductionRangeUpper)
    {
      throw new NotImplementedException();
    }
  }
}
