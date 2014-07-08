using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class ArrayBindingFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		public class TargetBindingSimple
		{
			[ParameterVerb]
			public string [] Action { get; set; }

			[ParameterAlias("name")]
			[ParameterAlias("n")]
			public string [] Name { get; set; }

			[ParameterAlias("age")]
			public int [] Age { get; set; }

			[ParameterAlias("vip")]
			public bool [] IsVip { get; set; }
		}

		[Fact]
		public void Can_Bind_Multiple_Verb_To_Existing_Instance()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("", "action1").AddValue("", "action2");
			var target = new TargetBindingSimple();

			// Act
			_binder.BindTo(args, target);

			// Assert
			target.Action.Length.ShouldEqual(2);
			target.Action.ShouldContain("action1");
			target.Action.ShouldContain("action2");
		}
		
		[Fact]
		public void Can_Bind_Multiple_Verb_To_New_Instance()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("", "action1").AddValue("", "action2");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Action.Length.ShouldEqual(2);
			result.Action.ShouldContain("action1");
			result.Action.ShouldContain("action2");
		}

		[Fact]
		public void Can_Bind_Multiple_Parameters_By_Alias()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("name", "Mr Test").AddValue("name", "Mrs Jones");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);
			
			// Assert
			result.Name.Length.ShouldEqual(2);
			result.Name.ShouldContain("Mr Test");
			result.Name.ShouldContain("Mrs Jones");
		}

		[Fact]
		public void Can_Bind_Multiple_Integer_Values()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("age", "41").AddValue("age", "50");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Age.Length.ShouldEqual(2);
			result.Age.ShouldContain(41);
			result.Age.ShouldContain(50);
		}

		[Fact]
		public void Can_Bind_Multiple_Invalid_Integer_Values_Sets_Default()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("age", "yes").AddValue("age", "no");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Age.Length.ShouldEqual(2);
			result.Age[0].ShouldEqual(0);
			result.Age[1].ShouldEqual(0);
		}

		[Fact]
		public void Can_Bind_Multiple_Boolean_Values()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("vip", null).AddValue("vip", "true").AddValue("vip", "false");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.IsVip.Length.ShouldEqual(2);
			result.IsVip[0].ShouldBeTrue();
			result.IsVip[1].ShouldBeFalse();
		}
	}
}
