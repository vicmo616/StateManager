using DbLibrary;
using Microsoft.EntityFrameworkCore;
using StateManagerModels;

namespace StateDataService
{
    public class StatesService : IStatesService
    {
        private readonly StateDbContext _context;
            public StatesService(StateDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddOrUpdateAsync(State state)
        {
            if (state.Id == 0)
            {
            return await AddAsync(state);
            }
            else
            {
                return await UpdateAsync(state); 
            }
        }
        private async Task<int> AddAsync(State state)
        {
            await _context.States.AddAsync(state);
            await _context.SaveChangesAsync();
            return state.Id;
        }
        private async Task<int> UpdateAsync(State state)
        {
            var existingState = await _context.States.FirstOrDefaultAsync(x => x.Id == state.Id);
            if (existingState == null)
            {
                throw new Exception("State not found");
            }
            existingState.Abbreviation = state.Abbreviation;
            existingState.Name = state.Name;
            await _context.SaveChangesAsync();
            return state.Id;
        }
        public async Task<int> DeleteAsync(State state)
        {
            return await DeleteAsync(state.Id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            var existingState = await _context.States.FirstOrDefaultAsync(x => x.Id == id);
            if (existingState == null)
            {
                throw new Exception("State not found");
            }
            await Task.Run(() => { _context.States.Remove(existingState); });
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var state = await _context.States.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
            return state != null;
        }

        public async Task<IList<State>> GetAllAsync()
        {
            return await _context.States.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<State?> GetAsync(int id)
        {
                var state = await _context.States
                .FirstOrDefaultAsync(m => m.Id == id);
                return state;
        }
    }
}