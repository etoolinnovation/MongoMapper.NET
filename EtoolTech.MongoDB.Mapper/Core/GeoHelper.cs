using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EtoolTech.MongoDB.Mapper.Core
{
    public class GeoHelper
    {
        public static GeoJsonPolygon<GeoJson2DGeographicCoordinates> ListOfLocationsToPolygon(List<double[]> Locations)
        {
            List<GeoJson2DGeographicCoordinates> geographicList = new List<GeoJson2DGeographicCoordinates>();
            foreach (var location in Locations)
            {
                geographicList.Add(GeoJson.Geographic(location[0], location[1]));
            }
            return GeoJson.Polygon(geographicList.ToArray());
        }

        public static List<double[]> PolygonToListOfLocations(GeoJsonPolygon<GeoJson2DGeographicCoordinates> Poligon)
        {
            List<double[]> result = new List<double[]>();
            foreach (var c in Poligon.Coordinates.Exterior.Positions)
            { 
               var c2 = c;
               var loc = new double[] { c.Longitude, c.Latitude };
               result.Add(loc);
            }
            return result;
        }
    }
}
