namespace TARge25Shop.Models.Kindergarden
{
    public class KindergardenImageViewModel
    {
        public Guid ImageId { get; set; }
        public string? ImageTitle { get; set; }
        public byte[]? ImageData { get; set; }
        public string? Image { get; set; }
        public Guid? KindergardenId { get; set; }
    }
}