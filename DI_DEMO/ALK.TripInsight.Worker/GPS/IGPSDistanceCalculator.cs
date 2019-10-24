namespace ALK.TripInsight.Worker.GPS
{
    internal interface IGpsDistanceCalculator
    {
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}
