public class Aircraft
{
    public string Callsign { get; set; }
    public string ADEP { get; set; }
    public string ADES { get; set; }
    public string ATYP { get; set; }
    public string RouteString { get; set; }
    public Route? Route { get; set; }
    public double TrackMileage { get; set; }
    private bool _passedStart = false;
    private bool _passedEnd = false;
    private List<Position> _trail;

    public Aircraft(string callsign, string adep, string ades, string atyp, string route, Position pos)
    {
        Callsign = callsign;
        ADEP = adep;
        ADES = ades;
        ATYP = atyp;
        RouteString = route;
        TrackMileage = 0;
        _trail = new() { pos };
    }

    public void AddNextPosition(Position pos)
    {
        _trail.Add(pos);

        if (Route is not null)
        {
            if (!_passedStart)
            {
                if (HasPassedPoint(Route.Startfix[0], Route.Startfix[1]))
                {
                    _passedStart = true;
                }
            }
            else if (!_passedEnd)
            {
                TrackMileage += pos.GetDistanceToPosition(_trail[^2]);
                if (HasPassedPoint(Route.Endfix[0], Route.Endfix[1]))
                {
                    _passedEnd = true;
                }
            }
        }
    }

    private double GetCurrentBearing()
    {
        if (_trail.Count() < 2) return 0;

        return _trail[^2].GetBearingToPositon(_trail[^1]);
    }

    private bool HasPassedPoint(double lat, double lon)
    {
        // find angle between current track and direct track to point
        // then if this angle is right or obtuse, AND the position is within 3nm? of the point
        // the point has been passed
        // ngl it would probably be more robust if i just made them vectors and used the dot product but i cba

        double diff = Math.Abs(GetCurrentBearing() - _trail[^1].GetBearingToPositon(new(lat, lon, 0, 0)));
        diff = Math.Min(diff, 360 - diff);

        if (diff > 90 && _trail[^1].GetDistanceToPosition(new(lat, lon, 0, 0)) <= 3)
        {
            return true;
        }

        return false;
    }
}