using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Web;

public class lastapi
{
    private string key;
    private WebClient wc;

    public lastapi(string key)
    {
        this.key = key;
        this.wc = new WebClient();
        return;
    }

    public Dictionary<string, string> GetNowPlaying(string user)
    {
        Dictionary<string, string> returns = new Dictionary<string, string> { };
        string data = wc.DownloadString("https://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=" + user + "&format=xml&api_key=" + this.key);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(data);
        XmlNode track = doc.DocumentElement.SelectSingleNode("//track[@nowplaying='true']");
        if (track != null)
        {
            XmlNode totalScrobbles = doc.DocumentElement.SelectSingleNode("//@total");
            returns.Add("totalScrobbles", totalScrobbles != null ? totalScrobbles.Value.ToString() : "0");
            XmlNode userName = doc.DocumentElement.SelectSingleNode("//@user");
            returns.Add("userName", userName != null ? userName.Value : "LastFMUser");
            XmlNode name = track.SelectSingleNode("//name");
            returns.Add("name", name != null ? name.InnerText : "TrackName");
            XmlNode artist = track.SelectSingleNode("//artist");
            returns.Add("artist", artist != null ? artist.InnerText : "TrackArtist");
            XmlNode album = track.SelectSingleNode("//album");
            returns.Add("album", album != null ? album.InnerText : "");
            XmlNode cover = track.SelectSingleNode("//image[@size='large']");
            returns.Add("cover", cover != null ? cover.InnerText : "https://last.fm/");
            XmlNode link = track.SelectSingleNode("//url");
            returns.Add("link", link != null ? link.InnerText : "https://last.fm/");
        }
        return returns;
    }

    public Dictionary<string, dynamic> GetTrackInfo(string name, string artist)
    {
        Dictionary<string, dynamic> returns = new Dictionary<string, dynamic> { };
        string data = wc.DownloadString("https://ws.audioscrobbler.com/2.0/?method=track.getInfo&user=afoxie&api_key=320f18851f50af9b8a14122c832d3042&format=xml&artist=" + HttpUtility.UrlEncode(artist) + "&track=" + HttpUtility.UrlEncode(name));
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(data);
        XmlNode track = doc.DocumentElement.SelectSingleNode("//track");
        if (track != null)
        {
            XmlNode userloved = track.SelectSingleNode("//userloved");
            returns.Add("isFavorite", userloved != null ? Int64.Parse(userloved.InnerText) == 1 : false);
            XmlNode scrobbles = track.SelectSingleNode("//userplaycount");
            returns.Add("scrobbles", scrobbles != null ? Int64.Parse(scrobbles.InnerText) : 0);
            XmlNode duration = track.SelectSingleNode("//duration");
            returns.Add("duration", duration != null ? Int64.Parse(duration.InnerText) : 0);
        }
        return returns;
    }
}