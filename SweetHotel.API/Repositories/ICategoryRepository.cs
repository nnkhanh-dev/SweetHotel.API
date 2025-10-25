using SweetHotel.API.Entities.Entities;

namespace SweetHotel.API.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetByMaxPeopleAsync(int maxPeople);
    }
}
