using System.Collections.Generic;
using MSCorp.FirstResponse.AzureSearch.Commands.Helper;
using Newtonsoft.Json;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Model
{
    public class Person
    {
        private readonly string[] _hairColors = { "Brown", "Blonde", "Black", "Red" };
        private readonly string[] _eyeColors = { "Brown", "Green", "Blue", "Hazel" };
        private readonly string[] _sex = { "Male", "Female" };
        private readonly string[] _regions = { "King", "King", "King", "King", "King", "King", "Snohomish", "Snohomish", "Snohomish", "Snohomish", "Pierce", "Pierce", "Pierce", "Pierce" };
        private readonly string[] _kingCities = { "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Seattle", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Bellevue", "Kent", "Renton", "Federal Way", "Kirkland", "Kirkland", "Kirkland", "Kirkland", "Kirkland", "Kirkland", "Redmond", "Redmond", "Redmond", "Redmond", "Redmond", "Redmond", "Shoreline", "Sammamish", "Burien", "Issaquah", "Des Moines", "SeaTac", "Maple Valley", "Mercer Island", "Kenmore", "Tukwila", "Covington", "Lake Forest Park", "Snoqualmie" };
        private readonly string[] _snohomishCities = { "Everett", "Marysville", "Edmonds", "Lynnwood", "Lake Stevens", "Mukilteo", "Mountlake Terrace", "Mill Creek", "Arlington", "Monroe", "Snohomish", "Stanwood", "Brier", "Sultan", "Granite Falls", "Gold Bar", "Woodway" };
        private readonly string[] _pierceCities = { "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Tacoma", "Lakewood", "Puyallup", "University Place", "Bonney Lake", "Edgewood", "Sumner", "Fife", "DuPont", "Gig Harbor", "Orting", "Fircrest", "Buckley", "Roy", "Ruston" };

        // King 47.798666, -122.409016 to 47.463651, -121.799275

        // pierce 47.303719, -122.521626 to 46.890506, -121.274678

        // snohomish 47.809735, -122.398030 to 48.309042, -121.101643

        public Person()
        {
            HairColor = RandomValueOfArray(_hairColors);
            EyeColor = RandomValueOfArray(_eyeColors);
            Sex = RandomValueOfArray(_sex);
            YearOfBirth = IntUtil.Random(1940, 2005);
            SearchAction = "mergeOrUpload";
            Region = RandomValueOfArray(_regions);
            switch (Region)
            {
                case "King":
                    City = RandomValueOfArray(_kingCities);
                    //var kingLatitude = 47.463651 + ((double) IntUtil.Random(0, 335015)/1000000);
                    //var kingLongitude = -121.799275 + ((double)IntUtil.Random(0, 609741) / 1000000);

                    var kingLatitude = 47.49 + ((double)IntUtil.Random(0, 200) / 1000); 
                    var kingLongitude = -122.4 + ((double)IntUtil.Random(0, 400) / 1000); 
                    //47.615330, -122.146812

                    HomeLocation = new GeoPoint(kingLatitude, kingLongitude);
                    break;

                case "Snohomish":
                    City = RandomValueOfArray(_snohomishCities);
                    //var snoLatitude = 47.809735 + ((double)IntUtil.Random(0, 499307) / 1000000);
                    //var snoLongitude = -122.398030 + ((double)IntUtil.Random(0, 1296387) / 10000000);

                    var snoLatitude = 47.7 + ((double)IntUtil.Random(0, 500) / 1000);
                    var snoLongitude = -122.3 + ((double)IntUtil.Random(0, 500) / 1000); 
                    //47.916760, -122.102074
                    HomeLocation = new GeoPoint(snoLatitude, snoLongitude);
                    break;

                case "Pierce":
                    City = RandomValueOfArray(_pierceCities);
                    //var pierceLatitude = 46.890506 + ((double)IntUtil.Random(0, 413213) / 10000000);
                    //var pierceLongitude = -121.274678 + ((double)IntUtil.Random(0, 1246948) / 10000000);
                    var pierceLatitude = 46.93 + ((double)IntUtil.Random(0, 400) / 1000); 
                    var pierceLongitude = -122.6 + ((double)IntUtil.Random(0, 500) / 1000); 
                    //47.218698, -122.416958
                    HomeLocation = new GeoPoint(pierceLatitude, pierceLongitude);
                    break;
            }
            State = "WA";
        }

        private T RandomValueOfArray<T>(IReadOnlyList<T> array)
        {
            return array[IntUtil.Random(0, array.Count)];
        }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string EyeColor { get; set; }
        public string HairColor { get; set; }

        public string Sex { get; set; }
        public int YearOfBirth { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Region { get; set; }

        [JsonProperty(PropertyName = "@search.action")]
        public string SearchAction { get; set; }
        public GeoPoint HomeLocation { get; set; }
    }
}