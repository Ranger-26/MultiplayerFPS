using UnityEngine;

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

    public static string RandomGreekLetter(int a, int b)
    {
        return GreekLetter[Random.Range(a, b)];
    }
}
