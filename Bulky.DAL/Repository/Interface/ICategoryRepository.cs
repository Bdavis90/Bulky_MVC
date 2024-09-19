using Bulky.BAL.Models;

namespace Bulky.DAL.Repository.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
    }
}
