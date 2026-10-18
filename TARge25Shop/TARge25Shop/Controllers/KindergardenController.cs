using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Models.Kindergarden;
using TARge25Shop.Data;

namespace TARge25Shop.Controllers
{
    public class KindergardenController : Controller
    {
        private readonly IKindergardenServices _kindergardenServices;
        private readonly TARge25ShopContext _context;

        private readonly IFileServices _fileServices;

        public KindergardenController
            (
            IKindergardenServices kindergardenServices,
            TARge25ShopContext context,
            IFileServices fileServices
            )
        {
            _kindergardenServices = kindergardenServices;
            _context = context;
            _fileServices = fileServices;
        }
        public IActionResult Index()
        {
            var result = _context.Kindergarden
                .Select(x => new KindergardenIndexViewModel
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    ChildrenCount = x.ChildrenCount,
                    KindergardenName = x.KindergardenName,
                    TeacherName = x.TeacherName,
                    CreatedAt = x.CreatedAt,
                });
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            KindergardenCreateUpdateViewModel result = new();

            return View("CreateUpdate", result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KindergardenCreateUpdateViewModel vm)
        {
            var dto = new KindergardenDto
            {
                GroupName = vm.GroupName,
                ChildrenCount = vm.ChildrenCount,
                KindergardenName = vm.KindergardenName,
                TeacherName = vm.TeacherName,

                // Failide lisamine
                Files = vm.Files,
                Image = vm.Image
                    .Select(x => new FileToDatabaseDto
                    {
                        Id = x.ImageId,
                        KindergardenId = x.KindergardenId,
                        ImageTitle = x.ImageTitle,
                        ImageData = x.ImageData,
                    }).ToArray()
            };

            //Teenuse väljakutsumine, et luua uus Kindergarden
            var result = await _kindergardenServices.Create(dto);

            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var kindergarden = await _kindergardenServices.DetailAsync(id);

            if (kindergarden == null)
            { return NotFound(); }

            KindergardenImageViewModel[] images = await FileFromDatabase(id);

            var vm = new KindergardenCreateUpdateViewModel();

            vm.Id = kindergarden.Id;
            vm.GroupName = kindergarden.GroupName;
            vm.ChildrenCount = kindergarden.ChildrenCount;
            vm.KindergardenName = kindergarden.KindergardenName;
            vm.KindergardenName = kindergarden.KindergardenName;
            vm.TeacherName = kindergarden.TeacherName;
            vm.CreatedAt = kindergarden.CreatedAt;
            vm.UpdatedAt = kindergarden.UpdatedAt;
            vm.Image.AddRange(images);

            return View("CreateUpdate", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(KindergardenCreateUpdateViewModel vm)
        {
            var dto = new KindergardenDto
            {
                Id = vm.Id,
                GroupName = vm.GroupName,
                ChildrenCount = vm.ChildrenCount,
                KindergardenName = vm.KindergardenName,
                TeacherName = vm.TeacherName,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = vm.UpdatedAt,

                Files = vm.Files,
                Image = vm.Image
                    .Select(x => new FileToDatabaseDto
                    {
                        Id = x.ImageId,
                        KindergardenId = x.KindergardenId,
                        ImageTitle = x.ImageTitle,
                        ImageData = x.ImageData,
                    }).ToArray()
            };

            var result = await _kindergardenServices.Update(dto);

            // Vaja, et pärast suunata objekti detail lehe vaatele 
            var kindergardenId = result.Id;

            // If vajalik, juhuks kui update ei õnnestu ja result tagastab null.
            if (result == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Details), new { id = kindergardenId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var kindergarden = await _kindergardenServices.DetailAsync(Id);

            if (kindergarden == null)
            {
                return NotFound();
            }

            KindergardenImageViewModel[] images = await FileFromDatabase(Id);

            //See on vaheinstants domaini ja vm vahel
            var vm = new KindergardenDeleteViewModel();

            vm.Id = kindergarden.Id;
            vm.GroupName = kindergarden.GroupName;
            vm.ChildrenCount = kindergarden.ChildrenCount;
            vm.KindergardenName = kindergarden.KindergardenName;
            vm.TeacherName = kindergarden.TeacherName;
            vm.CreatedAt = kindergarden.CreatedAt;
            vm.UpdatedAt = kindergarden.UpdatedAt;
            vm.Image.AddRange(images);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var kindergarden = await _kindergardenServices.Delete(id);

            if (kindergarden == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid Id)
        {
            var kindergarden = await _kindergardenServices.DetailAsync(Id);

            if (kindergarden == null)
            {
                return NotFound();
            }

            KindergardenImageViewModel[] images = await FileFromDatabase(Id);

            //See on vaheinstants domaini ja vm vahel
            var vm = new KindergardenDetailsViewModel();

            vm.Id = kindergarden.Id;
            vm.GroupName = kindergarden.GroupName;
            vm.ChildrenCount = kindergarden.ChildrenCount;
            vm.KindergardenName = kindergarden.KindergardenName;
            vm.TeacherName = kindergarden.TeacherName;
            vm.CreatedAt = kindergarden.CreatedAt;
            vm.UpdatedAt = kindergarden.UpdatedAt;
            vm.Image.AddRange(images);

            return View(vm);
        }

        private async Task<KindergardenImageViewModel[]> FileFromDatabase(Guid id)
        {
            return await _context.FileToDatabase
                .Where(x => x.KindergardenId == id)
                .Select(y => new KindergardenImageViewModel
                {
                    ImageId = y.Id,
                    KindergardenId = y.KindergardenId,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();
        }


        [HttpPost]
        public async Task<IActionResult> RemoveImage(KindergardenImageViewModel vm)
        {
            var dto = new FileToDatabaseDto()
            {
                Id = vm.ImageId
            };

            var image = await _fileServices.RemoveImageFromDatabase(dto);

            var kindergardenId = image.KindergardenId;

            if (image != null)
            {
                return RedirectToAction(nameof(Index));
            }

            // ID on vajalik, et teaks milline Update lehekülg tuleb kuvada, muidu läheb index lehele.
            return RedirectToAction(nameof(Update), new { id = kindergardenId });
        }
    }
}