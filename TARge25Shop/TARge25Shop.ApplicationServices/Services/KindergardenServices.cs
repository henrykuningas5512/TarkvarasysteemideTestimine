using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class KindergardenServices : IKindergardenServices
    {
        public readonly TARge25ShopContext _context;
        private readonly IFileServices _fileServices;

        public KindergardenServices
            (
                TARge25ShopContext context,
                IFileServices fileServices
            )
        {
            _context = context;
            _fileServices = fileServices;
        }
        public async Task<Kindergarden> Create(KindergardenDto dto)
        {
            //Vaheinstants dto ja domain vahel
            Kindergarden kindergarden = new Kindergarden();

            kindergarden.Id = Guid.NewGuid();
            kindergarden.GroupName = dto.GroupName;
            kindergarden.ChildrenCount = dto.ChildrenCount;
            kindergarden.KindergardenName = dto.KindergardenName;
            kindergarden.TeacherName = dto.TeacherName;
            kindergarden.CreatedAt = DateTime.Now;
            kindergarden.UpdatedAt = DateTime.Now;

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, kindergarden);
            }

            //Andmete salvestamine andmebaasi
            await _context.Kindergarden.AddAsync(kindergarden);
            await _context.SaveChangesAsync();

            return kindergarden;
        }

        public async Task<Kindergarden> Update(KindergardenDto dto)
        {
            Kindergarden kindergarden = new();

            kindergarden.Id = dto.Id;
            kindergarden.GroupName = dto.GroupName;
            kindergarden.ChildrenCount = dto.ChildrenCount;
            kindergarden.KindergardenName = dto.KindergardenName;
            kindergarden.TeacherName = dto.TeacherName;
            kindergarden.CreatedAt = dto.CreatedAt;
            kindergarden.UpdatedAt = DateTime.Now;

            if (dto.Files != null)
            {
                _fileServices.UploadFilesToDatabase(dto, kindergarden);
            }

            _context.Kindergarden.Update(kindergarden);
            await _context.SaveChangesAsync();

            return kindergarden;
        }

        //Meetod aitab vastuvõtta dto ja uuendab olemasolevat
        public async Task<Kindergarden> DetailAsync(Guid id)
        {
            var kindergarden = await _context.Kindergarden
                .FirstOrDefaultAsync(x => x.Id == id);

            return kindergarden;
        }

        public async Task<Kindergarden> Delete(Guid id)
        {
            var result = await _context.Kindergarden
                .FirstOrDefaultAsync(x => x.Id == id);

            var images = await _context.FileToDatabase
                .Where(x => x.KindergardenId == id)
                .Select(y => new FileToDatabaseDto
                {
                    Id = y.Id
                }).ToArrayAsync();

            await _fileServices.RemoveImagesFromDatabase(images);

            _context.Kindergarden.Remove(result);
            await _context.SaveChangesAsync();

            return result;
        }
    }
}