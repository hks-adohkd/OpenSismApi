using DBContext.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class ContentsController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ContentsController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Contents
        public async Task<IActionResult> Index()
        {
            var slides = await _context.Contents.Where(c => c.Name == "slider").ToListAsync();
            try
            {
                int count = int.Parse(_context.Conditions.Where(c => c.Name == "slides_number").FirstOrDefault().Value);
                if (slides == null || slides.Count() < count)
                {
                    ViewBag.Closed = false;
                }
                else
                {
                    ViewBag.Closed = true;
                }
            }
            catch (Exception e) { }
            return View(slides);
        }

        // GET: Contents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // GET: Contents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Content content)
        {
            try
            {
                if (content.file != null)
                {
                    FileInfo fi = new FileInfo(content.file.FileName);
                    var newFilename = "P" + content.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\slider\" + newFilename);
                    var pathToSave = @"/images/slider/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        content.file.CopyTo(stream);
                    }
                    content.ImageUrl = pathToSave;
                }
                _context.Add(content);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(content);
            }
        }

        // GET: Contents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Content content)
        {
            if (id != content.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (content.file != null)
                    {
                        FileInfo fi = new FileInfo(content.file.FileName);
                        var newFilename = "P" + content.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\slider\" + newFilename);
                        var pathToSave = @"/images/slider/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            content.file.CopyTo(stream);
                        }
                        content.ImageUrl = pathToSave;
                    }
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(content);
        }
        
        // GET: Contents/Edit/5
        public async Task<IActionResult> IntroVidoe()
        {
            var content = await _context.Contents.Where(c => c.Name == "intro_video").FirstOrDefaultAsync();
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("IntroVidoe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IntroVidoe(Content content)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (content.file != null)
                    {
                        FileInfo fi = new FileInfo(content.file.FileName);
                        var newFilename = "P" + content.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\slider\" + newFilename);
                        var pathToSave = @"/images/slider/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            content.file.CopyTo(stream);
                        }
                        content.ImageUrl = pathToSave;
                    }
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return View(content);
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(content);
        }

        // GET: Contents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            try
            {
                _context.Contents.Remove(content);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(content);
            }
        }

        public async Task<IActionResult> IntroIndex()
        {
            var intros = await _context.Contents.Where(c => c.Name == "intro").ToListAsync();
            try
            {
                if (intros == null || intros.Count() < 3)
                {
                    ViewBag.Closed = false;
                }
                else
                {
                    ViewBag.Closed = true;
                }
            }
            catch (Exception e) { }
            return View(intros);
        }

        // GET: Contents/Details/5
        public async Task<IActionResult> IntroDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // GET: Contents/Create
        public IActionResult IntroCreate()
        {
            return View();
        }

        // POST: Contents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IntroCreate(Content content)
        {
            try
            {
                if (content.file != null)
                {
                    FileInfo fi = new FileInfo(content.file.FileName);
                    var newFilename = "P" + content.Id + "_" + string.Format("{0:d}",
                                      (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                    var webPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine("", webPath + @"\images\slider\" + newFilename);
                    var pathToSave = @"/images/slider/" + newFilename;
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        content.file.CopyTo(stream);
                    }
                    content.ImageUrl = pathToSave;
                }
                _context.Add(content);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(IntroIndex));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(content);
            }
        }

        // GET: Contents/Edit/5
        public async Task<IActionResult> IntroEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IntroEdit(int id, Content content)
        {
            if (id != content.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (content.file != null)
                    {
                        FileInfo fi = new FileInfo(content.file.FileName);
                        var newFilename = "P" + content.Id + "_" + string.Format("{0:d}",
                                          (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                        var webPath = _hostingEnvironment.WebRootPath;
                        var path = Path.Combine("", webPath + @"\images\slider\" + newFilename);
                        var pathToSave = @"/images/slider/" + newFilename;
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            content.file.CopyTo(stream);
                        }
                        content.ImageUrl = pathToSave;
                    }
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(IntroIndex));
            }
            HttpContext.Session.SetString("FailedMsg", FailedMsg);
            return View(content);
        }

        // GET: Contents/Delete/5
        public async Task<IActionResult> IntroDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("IntroDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IntroDeleteConfirmed(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            try
            {
                _context.Contents.Remove(content);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return RedirectToAction(nameof(IntroIndex));
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(content);
            }
        }

        public async Task<IActionResult> AboutUs()
        {
            var content = await _context.Contents.Where(c => c.Name == "about_us").FirstOrDefaultAsync();
            if (content == null)
            {
                return NotFound();
            }
            return View(content);
        }

        // POST: Contents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<IActionResult> AboutUs(Content content, IFormCollection formCollection)
        {
            var newContent = await _context.Contents.Where(c => c.Name == "about_us").FirstOrDefaultAsync();
            try
            {
                newContent.Script = formCollection["Script"].ToString();
                newContent.ScriptAr = formCollection["ScriptAr"].ToString();
                _context.Update(newContent);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("SuccessMsg", SuccessMsg);
                return View(newContent);
            }
            catch (Exception e)
            {
                HttpContext.Session.SetString("FailedMsg", FailedMsg);
                return View(content);
            }
        }

        private bool ContentExists(int id)
        {
            return _context.Contents.Any(e => e.Id == id);
        }
    }
}
