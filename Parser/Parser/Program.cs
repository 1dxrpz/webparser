using HtmlAgilityPack;
using OfficeOpenXml;
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
	static List<string> Headers = new()
	{
		"ID",
		"title",
		"about",
		"image",
		"ingredients_tags",
		"headings_text",
		"recipe_region",
		"portions",
		"recipe_time",
		"recipe_id",
		"recipe_like",
		"recipe_dislike",
		"recipe_calories",
		"recipe_fat",
		"recipe_protein",
		"recipe_carbs",
		"ingredients_name_list_0",
		"ingredients_name_list_1",
		"ingredients_name_list_2",
		"ingredients_name_list_3",
		"ingredients_name_list_4",
		"ingredients_name_list_5",
		"ingredients_name_list_6",
		"ingredients_name_list_7",
		"ingredients_name_list_8",
		"ingredients_name_list_9",
		"ingredients_name_list_10",
		"ingredients_name_list_11",
		"ingredients_name_list_12",
		"ingredients_name_list_13",
		"ingredients_name_list_14",
		"ingredients_name_list_15",
		"ingredients_name_list_16",
		"ingredients_name_list_17",
		"ingredients_name_list_18",
		"ingredients_name_list_19",
		"ingredients_name_list_20",
		"ingredients_name_list_21",
		"ingredients_name_list_22",
		"ingredients_name_list_23",
		"ingredients_name_list_24",
		"ingredients_name_list_25",
		"ingredients_name_list_26",
		"ingredients_name_list_27",
		"ingredients_value_list_0",
		"ingredients_value_list_1",
		"ingredients_value_list_2",
		"ingredients_value_list_3",
		"ingredients_value_list_4",
		"ingredients_value_list_5",
		"ingredients_value_list_6",
		"ingredients_value_list_7",
		"ingredients_value_list_8",
		"ingredients_value_list_9",
		"ingredients_value_list_10",
		"ingredients_value_list_11",
		"ingredients_value_list_12",
		"ingredients_value_list_13",
		"ingredients_value_list_14",
		"ingredients_value_list_15",
		"ingredients_value_list_16",
		"ingredients_value_list_17",
		"ingredients_value_list_18",
		"ingredients_value_list_19",
		"ingredients_value_list_20",
		"ingredients_value_list_21",
		"ingredients_value_list_22",
		"ingredients_value_list_23",
		"ingredients_value_list_24",
		"ingredients_value_list_25",
		"ingredients_value_list_26",
		"ingredients_value_list_27",
		"recipe_step_text_list_0",
		"recipe_step_text_list_1",
		"recipe_step_text_list_2",
		"recipe_step_text_list_3",
		"recipe_step_text_list_4",
		"recipe_step_text_list_5",
		"recipe_step_text_list_6",
		"recipe_step_text_list_7",
		"recipe_step_text_list_8",
		"recipe_step_text_list_9",
		"recipe_step_text_list_10",
		"recipe_step_text_list_11",
		"recipe_step_text_list_12",
		"recipe_step_text_list_13",
		"recipe_step_text_list_14",
		"recipe_step_text_list_15",
		"recipe_step_text_list_16",
		"recipe_step_text_list_17",
		"recipe_step_text_list_18",
		"recipe_step_text_list_19",
		"recipe_step_text_list_20",
		"recipe_step_text_list_21",
		"recipe_step_text_list_22",
		"recipe_step_text_list_23",
		"recipe_step_text_list_24",
		"recipe_step_text_list_25",
		"recipe_step_text_list_26",
		"recipe_step_text_list_27",
		"recipe_step_text_list_28",
		"recipe_step_text_list_29",
	};
	static List<Thread> threads = new();
	static List<ParsedPage> ParsedPages = new();
	static int id = 0;

	private static void Main(string[] args)
	{
		ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		int row = 1;
		var package = new ExcelPackage();
		ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
		for (int x = 0; x < 1; x++)
		{
			Console.WriteLine($"from {x * 50 + 1} to {x * 50 + 50}");
			for (int i = x * 50 + 1; i < 2; i++) //x * 50 + 50; i++)
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
		for (int x = 1; x < Headers.Count; x++)
		{
			worksheet.Cells[row, x].Value = Headers[x - 1];
		}
		
		ParsedPages.ForEach(v =>
		{
			row++;
			worksheet.Cells[row, 1].Value = row - 1;
			worksheet.Cells[row, 2].Value = v.Title;
			worksheet.Cells[row, 3].Value = v.About;
			worksheet.Cells[row, 4].Value = v.Image;
			worksheet.Cells[row, 5].Value = string.Join(',', v.Ingredients.Select(x => x.Item1));
			worksheet.Cells[row, 6].Value = string.Join('>', v.Headings);
			worksheet.Cells[row, 7].Value = v.Region;
			worksheet.Cells[row, 8].Value = v.Portions;
			worksheet.Cells[row, 9].Value = v.Time;
			worksheet.Cells[row, 10].Value = v.Id;
			worksheet.Cells[row, 11].Value = v.Likes;
			worksheet.Cells[row, 12].Value = v.Dislikes;
			worksheet.Cells[row, 13].Value = v.Calories;
			worksheet.Cells[row, 14].Value = v.Fat;
			worksheet.Cells[row, 15].Value = v.Protein;
			worksheet.Cells[row, 16].Value = v.Carbohydrate;
			for (int x = 0; x < v.Ingredients.Count; x++)
			{
				worksheet.Cells[row, x + 17].Value = v.Ingredients[x].Item1;
			}
			for (int x = 0; x < v.Ingredients.Count; x++)
			{
				worksheet.Cells[row, x + 45].Value = v.Ingredients[x].Item2;
			}
			for (int x = 0; x < v.Steps.Count; x++)
			{
				worksheet.Cells[row, x + 73].Value = v.Steps[x];
			}
		});
		FileInfo fileInfo = new FileInfo("out.xlsx");
		package.SaveAs(fileInfo);

	}
	static int LinkNum = 0;
	static void ScrapPage(string url)
	{
		using (var client = new WebClient())
		{
			var link = $"https://eda.ru{url}";
			string html = client.DownloadString(link);

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);
			var id = link.Split("/").Last();
			var title = doc.DocumentNode.SelectSingleNode("//meta[contains(@itemprop, 'keywords')]").GetAttributeValue("content", "");
			var about = doc.DocumentNode.SelectSingleNode("//meta[contains(@itemprop, 'description')]").GetAttributeValue("content", "");
			var _calories = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'calories')]");
			var calories = _calories == null ? "" : _calories.InnerText;
			var _protein = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'proteinContent')]");
			var protein = _protein == null ? "" : _protein.InnerText;
			var _fat = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'fatContent')]");
			var fat = _fat == null ? "" : _fat.InnerText;
			var _carbohydrate = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'carbohydrateContent')]");
			var carbohydrate = _carbohydrate == null ? "" : _carbohydrate.InnerText;
			var time = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'cookTime')]").InnerHtml;
			var region = doc.DocumentNode.SelectSingleNode("//meta[contains(@itemprop, 'recipeCuisine')]").GetAttributeValue("content", "");
			var portions = doc.DocumentNode.SelectSingleNode("//span[contains(@itemprop, 'recipeYield')]")
				.InnerHtml.Replace("<span>", "").Replace("</span>", "");
			var headings = doc.DocumentNode.SelectNodes("//ul[contains(@class, 'emotion-1kcflwj')]")
				.First()
				.SelectNodes("//span[contains(@class, 'emotion-1h6i17m')]")
				.Select(v => v.InnerHtml)
				.ToList();
			var category = doc.DocumentNode.SelectSingleNode("//meta[contains(@itemprop, 'recipeCategory')]").GetAttributeValue("content", "");
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
			var _image = doc.DocumentNode.SelectSingleNode("//img[contains(@alt, 'Превью фото')]");
			var image = _image == null ? "" : _image.Attributes["src"].Value.Replace("c88x88", "-x900");
			LinkNum++;
			Console.WriteLine($"{LinkNum}\t{link}");
			ParsedPages.Add(new()
			{
				Id = id,
				Title = title,
				About = about == null ? "" : about,
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
				Headings = new()
				{
					category,
					headings.Last()
				},
				Region = region,
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
					ScrapPage(href.Value);
				}
			}
		}
	}
}