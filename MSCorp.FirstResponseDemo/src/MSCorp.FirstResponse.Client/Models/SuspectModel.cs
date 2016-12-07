namespace MSCorp.FirstResponse.Client.Models
{
    public class SuspectModel
    {
        public string Name { get; set; }
        public string HairColor { get; set; }
        public string EyeColor { get; set; }
        public string SkinColor { get; set; }
        public string Sex { get; set; }
        public string SuspectSearchImage { get; set; }

        public string SuspectSearchImagePath => string.Format("/Assets/suspect/{0}", SuspectSearchImage);
    }
}
