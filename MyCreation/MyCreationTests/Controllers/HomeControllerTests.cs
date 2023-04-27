using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCreation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCreation.Controllers.Tests
{
	[TestClass()]
	public class HomeControllerTests
	{
		[TestMethod()]
		public void FactoryTest()
		{
			var controller = new HomeController();
			controller.Factory();
			//Assert.Fail();
		}
	}
}