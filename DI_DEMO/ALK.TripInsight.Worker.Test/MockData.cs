using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using ALK.TripInsight.Model;

namespace ALK.TripInsight.Worker.Test
{
    internal class MockData
    {
        internal static string GetMockGPSMessage()
        {
            return _serialize(GetGPSMessage());
        }

        internal static IDictionary<string, object> GetGPSArguments()
        {
            var attributes = new Dictionary<string, object>
            {
                {"ownerId", 240},
                {"type", "stop_status_message"},
            };

            return attributes;
        }

        internal static GpsMessage GetGPSMessage()
        {
            GpsMessage gpsMessage = new GpsMessage()
            {
                TripId = 1234,
                Positions = new List<Position>()
                {
                    new Position()
                    {
                        Id = 0,
                        Speed = 30,
                        Heading = null,
                        Altitude = 1000,
                        GPSCoordinate = new GPSCoordinate()
                        {
                            Latitude = "40.12345",
                            Longitude = "-79.12345"
                        },
                        DeviceTime = "2020-02-20T07:22:33-04:00"
                    }
                }
            };

            return gpsMessage;
        }

        internal static GpsMessage GetGPSMessage_MultiPosition()
        {
            GpsMessage gpsMessage = new GpsMessage()
            {
                TripId = 1234,
                Positions = new List<Position>()
                {
                    new Position()
                    {
                        Id = 0,
                        Speed = 30,
                        Heading = null,
                        Altitude = 1000,
                        GPSCoordinate = new GPSCoordinate()
                        {
                            Latitude = "40.12345",
                            Longitude = "-79.12345"
                        },
                        DeviceTime = "2020-02-20T07:22:33-04:00"
                    },
                    new Position()
                    {
                        Id = 1,
                        Speed = null,
                        Heading = null,
                        Altitude = null,
                        GPSCoordinate = new GPSCoordinate()
                        {
                            Latitude = "41.55555",
                            Longitude = "-79.11111"
                        },
                        DeviceTime = "2020-02-21T01:11:11-05:00"
                    }
                }
            };

            return gpsMessage;
        }

        internal static TripStopInfo GetTripStopInfo(bool bIncludeSite)
        {
            TripStopInfo stop = new TripStopInfo
            {
                StopStatus = StopStatus.Open,
                StopSequence = 0,
                StopName = "Test stop 0",
                StopId = 12345,
                PlannedETA = "2020-02-20T11:22:33-04:00",
                CurrentETA = "2020-02-20T12:22:33-04:00",
                ActualArrival = "2020-02-20T05:22:33-04:00",
                EarliestArrival = "2020-02-20T10:22:33-04:00",
                LatestArrival = "2020-02-20T13:22:33-04:00",
                AtRiskThreshold = 60,
                TooEarlyThreshold = 60,
                Location = new Location()
                {
                    Coords = new GPSCoordinate()
                    {
                        Latitude = "40.1235",
                        Longitude = "-79.1235"
                    }
                }
            };

            if (bIncludeSite)
            {
                stop.AssociatedSites = new List<SiteInfo>()
                {
                    new SiteInfo()
                    {
                        SiteName = "TestSite",
                        PublicSite = true,
                        SiteID = 8675309,
                        Polygon = new List<List<double[]>>()
                        {
                            new List<double[]>()
                            {
                                new double[] { -79.2235, 40.0235 },
                                new double[] { -79.0235, 40.1135 },
                                new double[] { -79.0235, 40.2235 },
                                new double[] { -79.2235, 40.2235 },
                                new double[] { -79.2235, 40.0235 }
                            }
                        }
                    }
                };
            }

            return stop;
        }

        internal static NotificationSettings GetNotificationSettings(long apiKeyId)
        {
            var notificationSettings = new NotificationSettings()
            {
                AccountId = 1,
                APIKeyId = apiKeyId,
                StopAutoUpdateEnabled = true,
                StopAutoUpdateNotificationEnabled = true
            };

            return notificationSettings;
        }

        private static string _serialize<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var serializer = new DataContractJsonSerializer(obj.GetType());
                    serializer.WriteObject(memoryStream, obj);

                    memoryStream.Position = 0;
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
