using HtmlAgilityPack;
using System.Net;

public class Program
{
	static List<string> links = new();
	static List<Thread> threads = new();
	private static void Main(string[] args)
	{
		for (int x = 0; x < 1; x++)
		{
			Console.WriteLine($"from {x * 50 + 1} to {x * 50 + 50}");
			//for (int i = x * 50 + 1; i < x * 50 + 50; i++)
			//{
			//	Thread thread = new Thread(() => ScrapLinks(i));
			//	thread.Start();
			//	threads.Add(thread);
			//}

			Thread thread = new Thread(() => ScrapLinks(12));
			thread.Start();
			threads.Add(thread);

			foreach (Thread t in threads)
			{
				t.Join();
			}
			Console.WriteLine("wait");
			Thread.Sleep(2000);

		}

		File.AppendAllLines("out.txt", links);
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