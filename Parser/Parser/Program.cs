using HtmlAgilityPack;
using System.Net;

public class ParsedPage
{
	public string Id { get; set; }
	public string Title { get; set; }
	public string About { get; set; }
	public string Calories;
	public string Protein;
	public string Fat;
	public string Carbohydrate;
	public string Portions;
	public string Time;
	public string Likes;
	public string Dislikes;
	public string Region;
	public string Image;
	public string Href;
	public List<(string, string)> Ingredients = new();
	public List<string> Steps = new();
	public List<string> Headings = new();
}

public class Program
{
	static List<string> links = new();
	static List<Thread> threads = new();
	static List<ParsedPage> ParsedPages = new();

	private static void Main(string[] args)
	{
		for (int x = 0; x < 1; x++)
		{
			Console.WriteLine($"from {x * 50 + 1} to {x * 50 + 50}");
			for (int i = x * 50 + 1; i < 2; i++) // x * 50 + 50
			{
				Thread thread = new Thread(() => ScrapLinks(i));
				thread.Start();
				threads.Add(thread);
			}

			foreach (Thread t in threads)
			{
				t.Join();
			}
			Console.WriteLine("wait");
			Thread.Sleep(2000);

		}

		for (int x = 0; x < links.Count; x++)
		{
			ScrapPage(x);
		}
		ParsedPages.ForEach(v =>
		{
			File.AppendAllText("out.xlsx", $"{v.Id}\t" +
				$"{v.Title}\t" +
				$"{v.About}\t" +
				$"{v.Image}\t" +
				$"{string.Join(',', v.Ingredients.Select(x => x.Item1))}\t" +
				$"{string.Join(',', v.Headings)}\t" +
				$"{v.Region}\t" +
				$"{v.Portions}\t" +
				$"{v.Time}\t" +
				$"{v.Id}\t" +
				$"{v.Likes}\t" +
				$"{v.Dislikes}\t" +
				$"{v.Calories}\t" +
				$"{v.Fat}\t" +
				$"{v.Protein}\t" +
				$"{v.Carbohydrate}\t" +
				$"{v.Href}\t");
		});
	}
	static void ScrapPage(int i)
	{
		using (var client = new WebClient())
		{
			var link = $"https://eda.ru{links[i]}";
            string html = client.DownloadString(link);

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			var id = link.Split("/").Last();
			var title = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'emotion-gl52ge')]").InnerHtml;
			var about = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'emotion-aiknw3')]").InnerHtml;
			var calories = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'calories')]").InnerHtml;
			var protein = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'proteinContent')]").InnerHtml;
			var fat = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'fatContent')]").InnerHtml;
			var carbohydrate = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'carbohydrateContent')]").InnerHtml;
			var time = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'emotion-my9yfq')]").InnerHtml;
			var portions = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'recipeYield')]")
				.SelectSingleNode("//span").InnerHtml;
			var headings = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'emotion-1kcflwj')]")
				.First()
				.SelectNodes("//span[contains(@class, 'emotion-1h6i17m')]")
				.Select(v => v.InnerHtml)
				.ToList();
			var ingredientsCount = doc.DocumentNode.SelectNodes("//span[contains(@class, 'emotion-bsdd3p')]")
				.Select(v => v.InnerHtml)
				.ToList();
			var ingredients = doc.DocumentNode.SelectNodes("//span[contains(@itemprop, 'recipeIngredient')]")
				.Select((v, index) => (v.InnerHtml, ingredientsCount[index]))
				.ToList();
			var likesDislikes = doc.DocumentNode.SelectNodes("//span[contains(@class, 'emotion-a07nxg')]")
				.Select(v => v.InnerHtml)
				.ToList();
			var steps = doc.DocumentNode.SelectNodes("//span[contains(@class, 'emotion-wdt5in')]")
				.Select(v => v.SelectSingleNode("//span[contains(@itemprop, 'text')]").InnerHtml)
				.ToList();
			var image = doc.DocumentNode.SelectSingleNode("//img[contains(@alt, 'Превью фото')]").Attributes["src"].Value.Replace("c88x88", "-x900");

			ParsedPages.Add(new()
			{
				Id = id,
				Title = title,
				About = about,
				Calories = calories,
				Carbohydrate = carbohydrate,
				Fat = fat,
				Protein = protein,
				Dislikes = likesDislikes[1],
				Likes = likesDislikes[0],
				Ingredients = ingredients,
				Portions = portions,
				Steps = steps,
				Time = time,
				Headings = headings,
				Region = headings[2],
				Image = image,
				Href = link
			});

		}
	}
	static void ScrapLinks(int page)
	{
		using (var client = new WebClient())
		{
			string html = client.DownloadString($"https://eda.ru/recepty?page={page}");

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[contains(@class, 'emotion-18hxz5k')]"))
			{
				HtmlAttribute href = link.Attributes["href"];
				if (href != null)
				{
					links.Add(href.Value);
				}
			}
		}
	}
}