using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.Abstractions;

namespace Controllers
{
    public class ReferenceController : Controller
    {
        private readonly IResourceService _resourceService;
        private readonly IUnitService _unitService;

        public ReferenceController(IResourceService resourceService, IUnitService unitService)
        {
            _resourceService = resourceService;
            _unitService = unitService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new ReferenceViewModel
            {
                Resources = await _resourceService.GetAllAsync(),
                Units = await _unitService.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResource(string name)
        {
            await _resourceService.AddAsync(name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResource(int id)
        {
            await _resourceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUnit(string name)
        {
            await _unitService.AddAsync(name);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            await _unitService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}