using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    public class IncomingDocumentsController : Controller
    {
        private readonly IIncomingDocumentService _service;

        public IncomingDocumentsController(IIncomingDocumentService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(
            DateTime? startDate,
            DateTime? endDate,
            List<int> documentNumbers,
            List<int> resourceIds,
            List<int> unitIds)
        {
            var viewModel = await _service.GetFilteredDocumentsAsync(
                startDate, endDate, documentNumbers, resourceIds, unitIds);

            viewModel.Documents = viewModel.Documents
                .OrderByDescending(d => d.Date)
                .ThenBy(d => d.Number)
                .ThenBy(d => d.IncomingResources.Select(ir => ir.Resource.Name).DefaultIfEmpty(string.Empty).Min())
                .ThenBy(d => d.IncomingResources.Select(ir => ir.UnitOfMeasurement.Name).DefaultIfEmpty(string.Empty).Min())
                .ToList();

            return View(viewModel);
        }



        public IActionResult Create()
        {
            return View(new IncomingDocument { Date = DateTime.UtcNow });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,Date")] IncomingDocument document)
        {
            if (!ModelState.IsValid) return View(document);

            try
            {
                await _service.AddDocumentAsync(document);
                return RedirectToAction("Edit", new { id = document.Id });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Number", "Документ с таким номером уже существует.");
                return View(document);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var document = await _service.GetDocumentByIdAsync(id);
            if (document == null) return NotFound();

            ViewData["Resources"] = new SelectList(
                await _service.GetActiveResourcesAsync(), "Id", "Name");

            ViewData["Units"] = new SelectList(
                await _service.GetActiveUnitsAsync(), "Id", "Name");

            return View(document);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Date")] IncomingDocument document)
        {
            if (id != document.Id) return NotFound();

            if (await _service.DocumentNumberExistsAsync(document.Number, document.Id))
                ModelState.AddModelError("Number", "Документ с таким номером уже существует.");

            if (ModelState.IsValid)
            {
                await _service.UpdateDocumentAsync(document);
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> AddResourceToDocument(int documentId, int resourceId, int unitId, decimal quantity)
        {
            var resource = await _service.AddResourceAsync(documentId, resourceId, unitId, quantity);
            if (resource == null) return BadRequest("Количество должно быть больше нуля.");
            return PartialView("_IncomingResourceRow", resource);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveResourceFromDocument(int incomingResourceId)
        {
            if (await _service.RemoveResourceAsync(incomingResourceId)) return Ok();
            return NotFound();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var document = await _service.GetDocumentByIdAsync(id);
            if (document == null) return NotFound();
            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteDocumentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
