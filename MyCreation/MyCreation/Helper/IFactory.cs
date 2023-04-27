using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCreation.Helper
{
	public interface IFactory
	{
		//工廠返回商品
		IProduct getProduct();
	}
}
