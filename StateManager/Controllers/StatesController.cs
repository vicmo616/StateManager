#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DbLibrary;
using StateManagerModels;
using StateDataService;
using Microsoft.AspNetCore.Authorization;
using StateManager.Data;
using Microsoft.Extensions.Caching.Memory;
using StateManager.Models;

namespace StateManager.Controllers
{
    [Authorize]
    public class StatesController : Controller
    {
        private readonly IMemoryCache _memoryCache;  
        private readonly IStatesService _service;
        public StatesController(IStatesService service, IMemoryCache memorycache)
        {
            _service = service;
            _memoryCache = memorycache;
        }

        // GET: States
        public async Task<IActionResult> Index()
        {
            var statesdata = new List<State>();
            if (!_memoryCache.TryGetValue(StateManagerConstants.AllStatesCache, out statesdata))
            {
                statesdata = await _service.GetAllAsync() as List<State>;
                _memoryCache.Set(StateManagerConstants.AllStatesCache, statesdata, TimeSpan.FromDays(1));
            }
            return View(statesdata);
        }

        // GET: States/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _service.GetAsync((int)id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        // GET: States/Create
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public async Task<IActionResult> Create([Bind("Id,Name,Abbreviation")] State state)
        {
            if (ModelState.IsValid)
            {
                await _service.AddOrUpdateAsync(state);
                _memoryCache.Remove(StateManagerConstants.AllStatesCache);
                return RedirectToAction(nameof(Index));
            }
            return View(state);
        }

        // GET: States/Edit/5
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var state = await _service.GetAsync((int)id);
            if (state == null)
            {
                return NotFound();
            }
            return View(state);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Abbreviation")] State state)
        {
            if (id != state.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.AddOrUpdateAsync(state);
                    _memoryCache.Remove(StateManagerConstants.AllStatesCache);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var existingState = await StateExists(state.Id);
                    if (existingState == null)
                    {
                    return NotFound();
                    }
                    else
                    {
                    throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(state);
        }

        // GET: States/Delete/5
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var state = await _service.GetAsync((int)id);
            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRolesService.ADMIN_ROLE_NAME)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            _memoryCache.Remove(StateManagerConstants.AllStatesCache);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> StateExists(int id)
        {
            return await _service.ExistsAsync(id);
        }
    }
}
