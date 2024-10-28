using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Qwiki.Models;

namespace Qwiki.Controllers
{
    //[Authorize]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IClock _clock;

        public ArticlesController(ApplicationDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        // GET & FILTER: Articles
        public async Task<IActionResult> Index(DateTime? exactDate = null)
        {
            if (_context.Articles == null)
                return Problem("Entity set 'ApplicationDbContext.Articles'  is null.");

            if (exactDate == null || !exactDate.HasValue)
                return View(await _context.Articles.OrderByDescending(a => a.PublishedUtc).Take(50).ToListAsync());

            var articles = from d in _context.Articles select d;
            articles = articles.Where(a => a.PublishedUtc.Date == exactDate.Value.Date);

            if (articles == null)
                return NotFound();

            return View(await articles.ToListAsync());
        }

        public async Task<IActionResult> Search(string? searchArticle, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (_context.Articles == null)
                return NotFound();
            var articles = from d in _context.Articles select d;

            if (startDate.HasValue && endDate.HasValue)
            {
                articles = articles.Where(a => a.PublishedUtc >= startDate && a.PublishedUtc <= endDate);
            } else if (endDate.HasValue)
            {
                articles = articles.Where(a => a.PublishedUtc <= endDate);
            } else if (startDate.HasValue)
            {
                articles = articles.Where(a => a.PublishedUtc >= startDate);
            }

            if (!string.IsNullOrEmpty(searchArticle))
            {
                searchArticle = searchArticle.ToLower().Trim();
                articles = articles.Where(d => d.Title.ToLower().Contains(searchArticle) || d.Content.ToLower().Contains(searchArticle));
            }

            if (articles == null)
                return NotFound();

            return View(await articles.ToListAsync());
        }

        // GET: Articles/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Articles == null)
        //    {
        //        return NotFound();
        //    }

        //    var article = await _context.Articles
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (article == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(article);
        //}

        // GET: Articles/Details/title
        [Route("article/{title?}")]
        public async Task<IActionResult> Details(string? title)
        {
            if (_context.Articles == null)
                return NotFound();

            if (string.IsNullOrEmpty(title))
                return RedirectToAction(nameof(Index));

            title = title.ToLower().Trim();
            var article = await _context.Articles.FirstOrDefaultAsync(d => d.Title.ToLower().Contains(title) || d.Content.ToLower().Contains(title));

            if (article == null)
                return NotFound();

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create() => View();

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Thumbnail,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Published = _clock.GetCurrentInstant();
                if ( _context.Articles.Any(d => d.Title.ToLower() == article.Title.ToLower()) )
                {
                    ModelState.AddModelError("Title", $"The title '{article.Title}' already exists. Please choose another name");
                    return View(article);
                }
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            /*if (article.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }*/
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Thumbnail,Content")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
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
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            /*if (article.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }*/

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Articles == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Articles'  is null.");
            }
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id) => (_context.Articles?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
