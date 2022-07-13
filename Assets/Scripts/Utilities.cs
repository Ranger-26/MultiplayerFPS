using UnityEngine;
using Random = UnityEngine.Random;

public static class Utilities
{
    public static string[] GreekLetter = new string[24]
        {
            "Alpha",
            "Beta",
            "Gamma",
            "Delta",
            "Epsilon",
            "Zeta",
            "Eta",
            "Theta",
            "Iota",
            "Kappa",
            "Lambda",
            "Mu",
            "Nu",
            "Xi",
            "Omicron",
            "Pi",
            "Rho",
            "Sigma",
            "Tau",
            "Upsilon",
            "Phi",
            "Chi",
            "Psi",
            "Omega"
        };

    public static string RandomGreekLetter(int a = 0, int b = 0)
    {
        bool specifiedRange = a != 0 && b != 0;
        return specifiedRange ? GreekLetter[Random.Range(a, b)] : GreekLetter[Random.Range(0, GreekLetter.Length - 1)];
    }
}
