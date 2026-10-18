using TARge25Shop.Core.Dto;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;


namespace TARge25Shop.Core.ServiceInterface
{
    public interface IKindergardenServices
    {
        Task<Kindergarden> Create(KindergardenDto dto);
        Task<Kindergarden> Update(KindergardenDto dto);
        Task<Kindergarden> DetailAsync(Guid id);
        Task<Kindergarden> Delete(Guid id);
    }
}
