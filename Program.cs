using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.Title = "last.rpc";
        var clientID = "879203754992349284";
        Discord.Discord discord = new Discord.Discord(Int64.Parse(clientID), (UInt64)Discord.CreateFlags.Default);
        /* ok somehow this was causing memory corruption so
        // ahahahahhahahah nvm it was discord being fucking stupid
        discord.SetLogHook(Discord.LogLevel.Debug, (level, message) =>
        {
            Console.WriteLine("[Discord][{0}] {1}", level, message);
        });*/

        var applicationManager = discord.GetApplicationManager();
        Logger.LogDiscord("Current Locale: " + applicationManager.GetCurrentLocale().ToString());
        Logger.LogDiscord("Current Branch: " + applicationManager.GetCurrentBranch().ToString());

        Discord.ActivityManager activityManager = discord.GetActivityManager();

        Dictionary<string, string>? config = ConfigManager.GetConfig();
        string api_key = config["api_key"];
        string lastfm_user = config["lastfm_user"];

        Logger.LogLast("Fetching user \"" + lastfm_user + "\" on last.fm");
        lastapi lastfmapi = new lastapi(api_key);

        Discord.ActivityManager.UpdateActivityHandler callback = result =>
        {
            Logger.LogDiscord("Update presence " + result.ToString().ToUpper());
        };

        try
        {
            string lastScrobble = "somethingRAndoMjuStInCassse8938981-0++=noSongsLLnamEdlikeThisAFoLikesCockUwu12309-==";
            while (true)
            {
                try
                {
                    discord.RunCallbacks();
                    Dictionary<string, string> nowplaying = lastfmapi.GetNowPlaying(lastfm_user);
                    if (nowplaying.Count != 0)
                    {
                        if (nowplaying["name"] != lastScrobble)
                        {
                            string nowscrobbling = "Now scrobbling: " + nowplaying["name"] + " by " + nowplaying["artist"];
                            Console.Title = "last.rpc | Connected as " + lastfm_user + " | " + nowscrobbling;
                            Logger.LogLast(nowscrobbling + (nowplaying["album"] != "" ? " (on " + nowplaying["album"] + ")" : ""));
                            lastScrobble = nowplaying["name"];
                            Logger.LogDiscord("Updating presence");
                            Dictionary<string, dynamic> extraData = lastfmapi.GetTrackInfo(nowplaying["name"], nowplaying["artist"]);
                            int endOffset = 0;
                            bool isFavorite = false;
                            if (extraData.Count > 0)
                            {
                                endOffset += extraData["duration"];
                                isFavorite = extraData["isFavorite"];
                            };
                            var albumFormatted = "";
                            if (nowplaying["album"] != "")
                            {
                                albumFormatted = " - " + nowplaying["album"];
                            };
                            var activity = new Discord.Activity
                            {
                                Type = Discord.ActivityType.Listening,
                                Details = nowplaying["name"],
                                State = nowplaying["artist"] + albumFormatted,
                                Timestamps =
                                {
                                    End = DateTimeOffset.Now.ToUnixTimeMilliseconds() + endOffset,
                                },
                                Assets =
                                {
                                    LargeImage = "lastfm",
                                    LargeText = nowplaying["userName"] + " has " + nowplaying["totalScrobbles"] + " total scrobbles",
                                },
                                Instance = true,
                            };
                            if (isFavorite)
                            {
                                activity.Assets.SmallImage = "loved";
                                activity.Assets.SmallText = "Loved Track";
                            };
                            activityManager.UpdateActivity(activity, callback);
                        }
                    }
                    else if (lastScrobble != "")
                    {
                        Console.Title = "last.rpc | Connected as " + lastfm_user + " | Scrobbling paused";
                        Logger.LogLast("Scrobbling stopped");
                        Logger.LogDiscord("Clearing presence");
                        activityManager.ClearActivity(result =>
                        {
                            Logger.LogDiscord("Clear presence " + result.ToString().ToUpper());
                        });
                        lastScrobble = "";
                    }
                }
                catch (Exception err)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(err.ToString());
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Thread.Sleep(5000);
            };
        }
        finally
        {
            discord.Dispose();
        };
    }
}
