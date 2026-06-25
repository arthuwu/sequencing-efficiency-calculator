using Newtonsoft.Json;

public class Parser
{
    public Dictionary<string, Aircraft> Aircrafts { get; set; } // i am fully aware that the plural form of "aircraft" is "aircraft", but we already have an Aircraft class, so...
    private readonly List<Route> _routes;
    private string[] _logfile;

    public Parser(string pathToLogfile)
    {
        Aircrafts = new();

        using (var fs = new FileStream("prefs.json", FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var sr = new StreamReader(fs))
        {
            string json = sr.ReadToEnd();
            _routes = JsonConvert.DeserializeObject<Prefs>(json)!.Routes;
        }

        _logfile = File.ReadAllLines(pathToLogfile);

        for (int l = 2; l < _logfile.Count(); l += 3)
        {
            string line = _logfile[l];
            if (line.Count() == 0) continue;

            if (line[0] == '@') // position update
            {
                HandlePositionUpdate(line);
            }
            else if (line.Contains("$FP")) // FPL update
            {
                HandleFPLUpdate(line);
            }
        }

        foreach (Aircraft a in Aircrafts.Values)
        {
            if (a.Route is not null) Console.WriteLine($"{a.Callsign} used {a.TrackMileage} miles to get from {a.Route.FplHas} to LIMES");
        }
    }

    private void HandlePositionUpdate(string line)
    {
        string[] vals = line.Split(':');
        string callsign = vals[1];

        int altitude;
        double lat, lon;
        if (!double.TryParse(vals[4], out lat) || !double.TryParse(vals[5], out lon) || !int.TryParse(vals[6], out altitude))
        {
            throw new Exception("Coordinates must be numbers; Altitude must be integer");
        }

        if (!Aircrafts.ContainsKey(callsign))
        {
            Aircrafts.Add(callsign, new(callsign, "", "", "", "", new()));
        }
        else
        {
            Aircrafts[callsign].AddNextPosition(new(lat, lon, altitude, 0));
        }
    }

    private void HandleFPLUpdate(string line)
    {
        string[] vals = line.Split(':');
        string callsign = vals[0].Substring(3);
        string atyp = vals[3].Split('/')[0];
        string adep = vals[5];
        string ades = vals[9];
        string route = vals[^1];

        if (!Aircrafts.ContainsKey(callsign))
        {
            Aircrafts.Add(callsign, new(callsign, adep, ades, atyp, route, new()));

            foreach (Route r in _routes)
            {
                if (Aircrafts[callsign].RouteString.Contains(r.FplHas))
                {
                    Aircrafts[callsign].Route = r;
                    return;
                }
            }
        }
        else
        {
            Aircrafts[callsign].ADEP = adep;
            Aircrafts[callsign].ADES = ades;
            Aircrafts[callsign].ATYP = atyp;

            if (Aircrafts[callsign].RouteString != route)
            {
                Aircrafts[callsign].RouteString = route;
                foreach (Route r in _routes)
                {
                    if (Aircrafts[callsign].RouteString.Contains(r.FplHas))
                    {
                        Aircrafts[callsign].Route = r;
                        return;
                    }
                }
            }
        }
    }
}