using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCreation.Helper
{
	public class FrenchFriesFactory : IFactory
	{
		//返回一般的薯條
		//@Override
		public IProduct getProduct()
		{
			return new FrenchFries();
		}

		//返回我們想要的狀態的薯條..
		public IProduct getProduct(string state)
		{
			return new FrenchFries(state);
		}
	}
}
