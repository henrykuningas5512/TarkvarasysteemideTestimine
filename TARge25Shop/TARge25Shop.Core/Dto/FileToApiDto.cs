namespace TARge25Shop.Core.Dto
{
    public class FileToApiDto
    {
        public Guid Id { get; set; }
        public string? ExistingFilePath { get; set; }
        public Guid? KindergardenId { get; set; }
    }
}