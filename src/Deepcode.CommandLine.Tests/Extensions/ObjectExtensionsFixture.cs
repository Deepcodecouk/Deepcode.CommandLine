using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deepcode.CommandLine.Extensions;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Extensions
{
	public class ObjectExtensionsFixture
	{
		public class TestDefaultsBase
		{
			[DefaultValue("Mr Test")]
			public string Name { get; set; }

			[DefaultValue("Base Description")]
			public virtual string Description { get; set; }
		}

		public class TestDefaultsDerived : TestDefaultsBase
		{
			[DefaultValue(42)]
			public int Age { get; set; }

			[DefaultValue("Derived Description")]
			public override string Description { get; set; }
		}

		[Fact]
		public void Given_Simple_Class_Instance_Should_Set_Default_Values()
		{
			// Arrange
			var instance = new TestDefaultsBase();

			// Act
			instance.ApplyDefaultValues();

			// Assert
			instance.Name.ShouldEqual("Mr Test");
			instance.Description.ShouldEqual("Base Description");
		}

		[Fact]
		public void Given_Derived_Class_Instance_Should_Set_Default_Values_Also_In_Base()
		{
			// Arrange
			var instance = new TestDefaultsDerived();

			// Act
			instance.ApplyDefaultValues();

			// Assert
			instance.Name.ShouldEqual("Mr Test");
			instance.Description.ShouldEqual("Derived Description");
			instance.Age.ShouldEqual(42);
		}
	}
}
