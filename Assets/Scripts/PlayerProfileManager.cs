using Steamworks;
using UnityEngine;

public static class PlayerProfileManager
{
    // This is for storing and returning some player data, such as name

    public static string PlayerName;

    public static string GetPlayerName()
    {
        // Check if PlayerName is not set

        if (string.IsNullOrEmpty(PlayerName))
        {
            // Check if Steam is running

            if (SteamManager.Initialized)
            {
                // Steam is running

                PlayerName = SteamFriends.GetPersonaName(); // Set it to the Steam account name

            }
            else
            {
                // Steam is not running

                PlayerName = Utilities.RandomGreekLetter(0, 23) + "-" + Random.Range(0, 99); // Give a random name (ie. "Epsilon-18")

            }
        }

        // Return the PlayerName value

        return PlayerName;
    }
}
