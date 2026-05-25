public struct Position
{
    public double Lat { get; set; }
    public double Lon { get; set; }
    public int Altitude { get; set; }
    public double Groundspeed { get; set; }

    public Position(double latitude, double longitude, int altitude, double groundspeed)
    {
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            throw new ArgumentOutOfRangeException("Invalid coordinate");
        }
        Lat = latitude;
        Lon = longitude;
        Altitude = altitude;
        Groundspeed = groundspeed;
    }

    public double GetDistanceToPosition(Position pos)
    {
        //haversine formula
        const double R = 6371e+003;
        const double M_TO_NM = 0.000539957;

        double lat1 = this.Lat * Math.PI / 180;
        double lat2 = pos.Lat * Math.PI / 180;

        double latDiff = (pos.Lat - this.Lat) * Math.PI / 180;
        double lonDiff = (pos.Lon - this.Lon) * Math.PI / 180;

        double a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c * M_TO_NM;
    }

    public double GetBearingToPositon(Position pos)
    {
        //haversine formula
        double lat1 = this.Lat * Math.PI / 180;
        double lon1 = this.Lon * Math.PI / 180;

        double lat2 = pos.Lat * Math.PI / 180;
        double lon2 = pos.Lon * Math.PI / 180;

        double a = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
        double b = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1);

        double radians = Math.Atan2(a, b);
        double brng = (radians * (180 / Math.PI) + 360) % 360;

        return brng;
    }
}