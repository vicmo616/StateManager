using StateManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateDataService
{
    public interface IStatesService 
    {
        Task<IList<State>> GetAllAsync();
        Task<State?> GetAsync(int id);
        Task<int> AddOrUpdateAsync(State state);
        Task<int> DeleteAsync(State state);
        Task<int> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

    }
}
