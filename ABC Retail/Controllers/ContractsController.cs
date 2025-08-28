using Microsoft.AspNetCore.Mvc;

namespace ABC_Retail.Controllers
{
    public class ContractsController : Controller
    {
        private readonly FileShareStorageService _files;

        public ContractsController(FileShareStorageService files) => _files = files;

        public async Task<IActionResult> Index()
        {
            var names = await _files.ListFilesAsync();
            var items = names.Select(n => (Name: n, Url: _files.GetFileSasUri(n, 60))).ToList();
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file is { Length: > 0 })
            {
                using var s = file.OpenReadStream();
                await _files.UpLoadFile(file.FileName, s);
            }
            return RedirectToAction(nameof(Index));
        }

        // Optional delete (only if you added the delete button in the view)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                await _files.DeleteFileAsync(name);
            return RedirectToAction(nameof(Index));
        }
    }
}
