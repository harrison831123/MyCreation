using Microsoft.AspNetCore.Mvc;
using NetCoreMvc.Models;
using OpenAI;
using OpenAI.Edits;
using OpenAI.Models;
using System.Diagnostics;
using System.Reflection;


namespace NetCoreMvc.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<List<OpenAIModel>> GetTask(string Chat)
		{
			var api = new OpenAIClient(new OpenAIAuthentication("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"));
			List<OpenAIModel> openAIModels = new List<OpenAIModel>();
			OpenAIModel temp = new OpenAIModel();

			#region 語法糖
			//文字修正
			//var request = new EditRequest("跟我說中文", "Fix the spelling mistakes"); 
			//var result = await api.EditsEndpoint.CreateEditAsync(request);
			//var models = await api.ModelsEndpoint.GetModelsAsync();
			//var resul = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("跟我說中文");
			//openAIModel.Chat = result.Choices[0].Text;
			#endregion

			#region 模式設定
			//問答
			await api.CompletionsEndpoint.StreamCompletionAsync(result =>
			{
				foreach (var choice in result.Completions)
				{
					temp.Chat += choice.Text;
				}
			}, Chat, max_tokens: 500, temperature: 0,top_p : 1, presencePenalty: 0.0, frequencyPenalty: 0.0, model: "text-davinci-003");

			//關鍵字
			//await api.CompletionsEndpoint.StreamCompletionAsync(result =>
			//{
			//	foreach (var choice in result.Completions)
			//	{
			//		temp.Chat += choice.Text;
			//	}
			//}, Chat, max_tokens: 500, temperature: 0.5, presencePenalty: 0.8, frequencyPenalty: 0.0, model: "text-davinci-003");

			//聊天模式
			//await api.CompletionsEndpoint.StreamCompletionAsync(result =>
			//{
			//	foreach (var choice in result.Completions)
			//	{
			//		temp.Chat += choice.Text;
			//	}
			//}, Chat, model: "text-davinci-003",temperature: 0.5,max_tokens: 500,top_p: 1.0,frequencyPenalty: 0.5,presencePenalty: 0.0);

			temp.Chat = DateTime.Now.ToString() + " AI: " + temp.Chat;
			openAIModels.Add(temp);
			#endregion

			return openAIModels;
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}