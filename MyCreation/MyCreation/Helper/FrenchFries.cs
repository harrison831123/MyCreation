using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCreation.Helper
{
	public class FrenchFries : IProduct
	{
		//預設有鹽巴的
		string state = "有鹽巴";
		//預設的建構
		public FrenchFries() { }
		//帶入狀態的建構
		public FrenchFries(string state)
		{
			this.state = state;
		}

		public void describe()
		{
			Console.WriteLine("我是" + state + "薯條");
		}
	}
}
