using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Parser;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class SimpleBindingFixture
	{
		private readonly CommandLineBinder _binder = new CommandLineBinder();

		public class TargetBindingSimple
		{
			[ParameterVerb]
			public string Action { get; set; }

			[ParameterAlias("name")]
			[ParameterAlias("n")]
			public string Name { get; set; }

			[ParameterAlias("age")]
			public int Age { get; set; }

			[ParameterAlias("vip")]
			public bool IsVip { get; set; }
		}

		[Fact]
		public void Can_Create_Through_Binder()
		{
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(new CommandLineArguments());
			result.ShouldNotBeNull();
			result.ShouldBeType(typeof (TargetBindingSimple));
		}

		[Fact]
		public void Can_Bind_Single_Verb_To_Existing_Instance()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("", "action1");
			var target = new TargetBindingSimple {Action = "I should be replaced"};

			// Act
			_binder.BindTo(args, target);

			// Assert
			target.Action.ShouldEqual("action1");
		}
		
		[Fact]
		public void Can_Bind_Single_Verb_To_New_Instance()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("", "action1");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Action.ShouldEqual("action1");
		}

		[Fact]
		public void Can_Bind_Multiple_Verbs_To_Single_Target_Last_Wins()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("", "action1").AddValue("", "action2");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Action.ShouldEqual("action2");
		}

		[Fact]
		public void Can_Bind_Parameters_By_Alias()
		{
			// Arrange
			var args1 = new CommandLineArguments().AddValue("name", "Mr Test");
			var args2 = new CommandLineArguments().AddValue("n", "Mr Test 2");

			// Act
			var result1 = _binder.CreateAndBindTo<TargetBindingSimple>(args1);
			var result2 = _binder.CreateAndBindTo<TargetBindingSimple>(args2);
			
			// Assert
			result1.Name.ShouldEqual("Mr Test");
			result2.Name.ShouldEqual("Mr Test 2");
		}

		[Fact]
		public void Can_Bind_Multiple_Parameters_To_Single_Target_Last_Wins()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("name", "Mr Test").AddValue("n", "Middle man").AddValue("n", "Mr Test 2");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Name.ShouldEqual("Mr Test 2");
		}

		[Fact]
		public void Can_Bind_Integer_Values()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("age", "41");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Age.ShouldEqual(41);
		}

		[Fact]
		public void Can_Bind_Invalid_Integer_Values_Sets_Default()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("age", "yes");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.Age.ShouldEqual(0);
		}

		[Fact]
		public void Can_Bind_Boolean_Values_When_Specified()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("vip", "true");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.IsVip.ShouldBeTrue();
		}

		[Fact]
		public void Can_Bind_Boolean_Values_When_Only_Flagged()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("vip", null);

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.IsVip.ShouldBeTrue();
		}

		[Fact]
		public void Can_Bind_Invalid_Boolean_Values_Sets_Default()
		{
			// Arrange
			var args = new CommandLineArguments().AddValue("vip", "invalid_value");

			// Act
			var result = _binder.CreateAndBindTo<TargetBindingSimple>(args);

			// Assert
			result.IsVip.ShouldBeTrue();
		}
	}
}
