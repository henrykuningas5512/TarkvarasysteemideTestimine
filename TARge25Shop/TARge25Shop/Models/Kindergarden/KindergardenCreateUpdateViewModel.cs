using TARge25Shop.Models.Kindergarden;

namespace TARge25Shop.Models.Kindergarden
{
    public class KindergardenCreateUpdateViewModel
    {
        public Guid? Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int ChildrenCount { get; set; }
        public string KindergardenName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;

        public List<IFormFile>? Files { get; set; }
        public List<KindergardenImageViewModel> Image { get; set; }
            = new List<KindergardenImageViewModel>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
