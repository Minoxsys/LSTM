﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests.Unit.Controllers.Areas.LocationManagement.OutpostControllerTests
{
	[TestFixture]
	public class GetWarehousesMethod
	{
		public readonly ObjectMother _ = new ObjectMother();
		[SetUp]
		public void BeforeEach()
		{
			_.Init();
		}

		[Test]
		public void Gets_All_Outposts_That_Ane_Not_DrugShops()
		{
			_.ExpectWarehousesToBeQueried();

			var jsonResult = _.controller.GetWarehouses();

			_.VerifyThatWarehousesHaveBeenQueried();
		}

	}
}
