using InstPlusEntityFr.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Linq;

namespace InstPlusEntityFr.Pages.MainPage
{
	public class PostWithComments
	{
        public string Image { get; set; }
        public string Opis { get; set; }
        public string ImageAvatar { get; set; }
        public string Nazwa { get; set; }
        public List<Komentarz> Komentarze { get; set; }
        public List<String> KomentarzeTresc { get; set; }
        public List<String> KomentarzeZDJ { get; set; }
        public List<String> KomentarzeAutor { get; set; }
        public List<String> KomentarzeData { get; set; }
        public List<String> Tagi { get; set; }
        public DateTime Data { get; set; }
        public int IloscPolubien { get; set; }
    }
	public class IndexModel : PageModel
	{
		private readonly IWebHostEnvironment _environment;
		DbInstagramPlus db = new DbInstagramPlus();
		public String getPosty()
		{
            

            return JsonConvert.SerializeObject(postsWithComments);
		}

		private List<PostWithComments> postsWithComments = new List<PostWithComments>();
		private Dictionary<String, int> mapaTagowSesji = new Dictionary<String, int>();
		public void OnGet()
		{
			//sprawdzenie czy jest kto� zalogowany
            var zalogowany = db.Uzytkownicy.Where(u => u.UzytkownikId == (int?)HttpContext.Session.GetInt32("UzytkownikId")).FirstOrDefault();
			if (zalogowany!=null)
			{
				//zliczenie tag�w
				foreach (var pol in db.PolubieniaPostow.Where(u => u.UzytkownikId == zalogowany.UzytkownikId))
				{
					var post = db.Posty.Where(u => u.PostId == pol.PostId).Select(u => u.Tagi);
					foreach (var p in post)
					{
						foreach (var v in p)
						{
							if (mapaTagowSesji.ContainsKey(v.Nazwa))
							{
								mapaTagowSesji[v.Nazwa]++;
							}
							else
							{
								mapaTagowSesji.Add(v.Nazwa, 1);
							}
						}
					}
				}
				//pomocnicze do test�w zliczania tagow
				/*foreach (var m in mapaTagowSesji)
				{
					Console.WriteLine($"{m.Key}: {m.Value}");
				}*/

				//dodawanie postu do listy (startowe)
                foreach (var idpost in db.Posty)
                {
                    var post = new PostWithComments();
					post.Tagi = new List<String>();
                    post.Komentarze = new List<Komentarz>();
                    post.KomentarzeTresc = new List<String>();
                    post.KomentarzeZDJ = new List<String>();
                    post.KomentarzeAutor = new List<String>();
                    post.Image = @Url.Content(idpost.Zdjecie);
                    post.Opis = idpost.Opis;
                    post.ImageAvatar = @Url.Content(db.Uzytkownicy.Where(u => u.UzytkownikId == idpost.UzytkownikId).FirstOrDefault().Zdjecie);
                    post.Nazwa = db.Uzytkownicy.Where(u => u.UzytkownikId == idpost.UzytkownikId).FirstOrDefault().Nazwa;
                    post.IloscPolubien = db.PolubieniaPostow.Count(u => u.PostId == idpost.PostId);
                    var tempKom = db.Komentarze.Where(u => u.PostId == idpost.PostId);
                    foreach (var kom in tempKom)
                    {
                        post.Komentarze.Add(kom);
                        post.KomentarzeTresc.Add(kom.Tresc);
                        var zdj = db.Uzytkownicy.Where(u => u.UzytkownikId == kom.UzytkownikId).Select(u => u.Zdjecie).FirstOrDefault();
                        post.KomentarzeZDJ.Add(@Url.Content(zdj));
                        var autor = db.Uzytkownicy.Where(u => u.UzytkownikId == kom.UzytkownikId).Select(u => u.Nazwa).FirstOrDefault();
                        post.KomentarzeAutor.Add(autor);
                    }
					var tempTagi = db.Posty.Where(u=>u.PostId==idpost.PostId).Select(s=>s.Tagi);
					foreach(var tag in tempTagi)
					{
						foreach(var x in tag)
						{
							post.Tagi.Add(x.Nazwa);
                        }
					}
					postsWithComments.Add(post);
                }
				//stworzenie mieszanej listy
                var mieszanaLista = new List<PostWithComments>();
                var random = new Random();
				//przepisanie losow do mieszanej listy
                while (postsWithComments.Count > 0)
                {
                    var losowyIndex = random.Next(postsWithComments.Count);
                    var post = postsWithComments[losowyIndex];
                    mieszanaLista.Add(post);
                    postsWithComments.RemoveAt(losowyIndex);
					Console.WriteLine(post.Nazwa);
					Console.WriteLine(mieszanaLista[mieszanaLista.Count-1].Nazwa);
                }
				//przepisanie do koncowej listy (algorytm)
				foreach (var post in mieszanaLista)
				{
					foreach (var s in post.Tagi)
					{
						if (mapaTagowSesji.ContainsKey(s))
						{
							postsWithComments.Insert(0, post);
							break;
						}
						else
						{ 
							postsWithComments.Insert(postsWithComments.Count/2,post);
							break;
						}
					}
                    Console.WriteLine(post.Nazwa);
                }
            }
			//to kiedy niezalogowany uzytkownik
			else
			{
				foreach (var idpost in db.Posty)
				{
					var post = new PostWithComments();
					post.Komentarze = new List<Komentarz>();
					post.KomentarzeTresc = new List<String>();
					post.KomentarzeZDJ = new List<String>();
					post.KomentarzeAutor = new List<String>();
					post.Image = @Url.Content(idpost.Zdjecie);
					post.Opis = idpost.Opis;
					post.Data = idpost.DataPublikacji;
					post.ImageAvatar = @Url.Content(db.Uzytkownicy.Where(u => u.UzytkownikId == idpost.UzytkownikId).FirstOrDefault().Zdjecie);
					post.Nazwa = db.Uzytkownicy.Where(u => u.UzytkownikId == idpost.UzytkownikId).FirstOrDefault().Nazwa;
					post.IloscPolubien = db.PolubieniaPostow.Count(u => u.PostId == idpost.PostId);
					var tempKom = db.Komentarze.Where(u => u.PostId == idpost.PostId);
					foreach (var kom in tempKom)
					{
						post.Komentarze.Add(kom);
						post.KomentarzeTresc.Add(kom.Tresc);
						var zdj = db.Uzytkownicy.Where(u => u.UzytkownikId == kom.UzytkownikId).Select(u => u.Zdjecie).FirstOrDefault();
						post.KomentarzeZDJ.Add(@Url.Content(zdj));
						var autor = db.Uzytkownicy.Where(u => u.UzytkownikId == kom.UzytkownikId).Select(u => u.Nazwa).FirstOrDefault();
						post.KomentarzeAutor.Add(autor);
					}

					postsWithComments.Add(post);
				}
				postsWithComments = postsWithComments.OrderByDescending(x => x.Data).ToList();
            }
        }

            [BindProperty]
		public IFormFile UploadedFile { get; set; }

		[BindProperty]
		public string FilePath { get; set; }

		public IndexModel(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		public IActionResult OnPost()
		{
			if (UploadedFile != null)
			{
				var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var filePath = Path.Combine(uploadsFolder, UploadedFile.FileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					UploadedFile.CopyTo(stream);
				}

				FilePath = filePath;
				var uz = db.Uzytkownicy.Where(s => s.UzytkownikId == 1);
				foreach (Uzytkownik u in uz)
				{
					u.Zdjecie = filePath;
				}
				db.SaveChanges();
				Console.WriteLine(filePath);
			}

			return Page();
		}

	}
	
}
