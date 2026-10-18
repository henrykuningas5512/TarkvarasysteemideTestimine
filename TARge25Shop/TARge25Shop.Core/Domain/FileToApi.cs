namespace TARge25Shop.Core.Domain
{
    public class FileToApi
    {
        public Guid Id { get; set; }
        public string? ImageTitle { get; set; }
        public byte[]? ImageData { get; set; }
        public Guid? RealEstateId { get; set; }
        public Guid? KindergardenId { get; set; }
    }
}