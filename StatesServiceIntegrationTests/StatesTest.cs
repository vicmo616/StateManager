using DbLibrary;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StateDataService;
using StateManagerModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StatesServiceIntegrationTests
{
    public class StatesTest 
    {
        private IStatesService _service;
        DbContextOptions<StateDbContext> _options;
        private const int NUMSTATES = 20;
        public StatesTest()
        {
            SetUpOptions();
            BuildDefaults();
        }
        private void SetUpOptions()
        {
            _options = new DbContextOptionsBuilder<StateDbContext>().UseInMemoryDatabase(databaseName: "StatesTestDB").Options;
        }
        private void BuildDefaults()
        {
            using(var context = new StateDbContext(_options))
            {
                var existingStates = Task.Run(() => context.States.ToListAsync()).Result;
                if (existingStates == null || existingStates.Count < 10)
                {
                    var states = GetStatesTestData();
                    context.States.AddRange(states);
                    context.SaveChanges();
                }
            }
        }
        private List<State> GetStatesTestData()
        {
            return new List<State>()
            {
                new State() { Id = 1, Name = "Alabama", Abbreviation = "AL" },
                new State() { Id = 2, Name = "Alaska", Abbreviation = "AK" },
                new State() { Id = 3, Name = "Arizona", Abbreviation = "AZ" },
                new State() { Id = 4, Name = "Arkansas", Abbreviation = "AR" },
                new State() { Id = 5, Name = "California", Abbreviation = "CA" },
                new State() { Id = 6, Name = "Colorado", Abbreviation = "CO" },
                new State() { Id = 7, Name = "Connecticut", Abbreviation = "CT" },
                new State() { Id = 8, Name = "Delaware", Abbreviation = "DE" },
                new State() { Id = 9, Name = "District of Columbia", Abbreviation = "DC" },
                new State() { Id = 10, Name = "Florida", Abbreviation = "FL" },
                new State() { Id = 11, Name = "Georgia", Abbreviation = "GA" },
                new State() { Id = 12, Name = "Hawaii", Abbreviation = "HI" },
                new State() { Id = 13, Name = "Idaho", Abbreviation = "ID" },
                new State() { Id = 14, Name = "Illinois", Abbreviation = "IL" },
                new State() { Id = 15, Name = "Indiana", Abbreviation = "IN" },
                new State() { Id = 16, Name = "Iwoa", Abbreviation = "IA" },
                new State() { Id = 17, Name = "Kansas", Abbreviation = "KS" },
                new State() { Id = 18, Name = "Kentucky", Abbreviation = "KY" },
                new State() { Id = 19, Name = "Louisiana", Abbreviation = "LA" },
                new State() { Id = 20, Name = "Maine", Abbreviation = "ME" }
            };
        }

        [Theory]
        [InlineData("Alabama", "AL", 0)]
        [InlineData("Iwoa", "IA", 15)]
        [InlineData("Maine", "ME", 19)]
        public async Task TestGetAllStates(string name, string abbreviation, int index)
        {
            using (var context = new StateDbContext(_options))
            {
                _service = new StatesService(context);
                var states = await _service.GetAllAsync();
                Assert.Equal(NUMSTATES, states.Count);
                Assert.Equal(name, states[index].Name);

                states.Count.ShouldBe(NUMSTATES);
                states[index].Name.ShouldBe(name, StringCompareShould.IgnoreCase);
                states[index].Abbreviation.ShouldBe(abbreviation, StringCompareShould.IgnoreCase);
            }
        }
        [Theory]
        [InlineData("Indiana", 15)]
        [InlineData("Maine", 20)]
        [InlineData("Idaho", 13)]
        public async Task TestGetOneState(string name, int id)
        {
            using (var context = new StateDbContext(_options))
            {
                _service = new StatesService(context);
                var state = await _service.GetAsync(id);
                state.Name.ShouldBe(name, StringCompareShould.IgnoreCase);
            }
        }
        public async Task UpdateState()
        {
            using (var context = new StateDbContext(_options))
            {
                _service = new StatesService(context);
                var state = await _service.GetAsync(16);
                state.ShouldNotBeNull();
                state.Name.ShouldBe("Iwoa", StringCompareShould.IgnoreCase);
                state.Name = "Iowa";// update name
                _service.AddOrUpdateAsync(state);// save change
                var newState = await _service.GetAsync(16);// get state back
                newState.Name.ShouldBe("Iowa", StringCompareShould.IgnoreCase);// check new name
                newState.Name = "Iwoa";// update back to incorrect spelling
                _service.AddOrUpdateAsync(newState);//get state back again
                var finalState = await _service.GetAsync(16);
                finalState.Name.ShouldBe("Iwoa", StringCompareShould.IgnoreCase);
            }
        }
        [Fact]
        public async Task AddAndDelete()
        {
            using (var context = new StateDbContext(_options))
            {
                _service = new StatesService(context);
                var state = new State() { Id = 0, Name = "Minnesota", Abbreviation = "MN" };//add a new state
                var newId = await _service.AddOrUpdateAsync(state); // update database
                var createdState = await _service.GetAsync(newId); //call state
                createdState.ShouldNotBeNull();
                createdState.Name.ShouldBe("Minnesota", StringCompareShould.IgnoreCase);
                await _service.DeleteAsync(createdState); // deleted state
                var deletedState = await _service.GetAsync(newId);
                deletedState.ShouldBeNull();
                //update database
                //check if state exists
            }
        }
    }
}