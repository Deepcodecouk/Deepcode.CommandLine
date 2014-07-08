using System.Text.RegularExpressions;
using Deepcode.CommandLine.Parser;
using Deepcode.CommandLine.Tests.Helpers;
using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Parser
{
	public class CommandLineParserFixture
	{
		private CommandLineArguments ParseCommandLine(string commandLine)
		{
			var args = commandLine.AsCommandLineArgs();
			var parser = new CommandLineParser();

			return parser.Parse(args);
		}

		[Fact]
		public void Empty_CommandLine_Parses_To_Empty()
		{
			// Arrange / Act
			var result = ParseCommandLine("");

			// Assert
			result.Switches.Length.ShouldEqual(0);
			result.Verbs.Length.ShouldEqual(0);
		}

		[Fact]
		public void Parsing_Can_Handle_Different_SwitchTypes()
		{
			// Arrange / Act
			var result = ParseCommandLine("-a /b --c ////d");

			// Assert
			result.Switches.Length.ShouldEqual(4);
			result.HasSwitch("a").ShouldBeTrue();
			result.HasSwitch("b").ShouldBeTrue();
			result.HasSwitch("c").ShouldBeTrue();
			result.HasSwitch("d").ShouldBeTrue();
		}

		[Fact]
		public void Parsing_Switches_Can_Contain_SwitchPrefix()
		{
			// Arrange / Act
			var result = ParseCommandLine("-a-to-b -b--to-c /c/d");

			// Assert
			result.Switches.Length.ShouldEqual(3);
			result.HasSwitch("a-to-b").ShouldBeTrue();
			result.HasSwitch("b--to-c").ShouldBeTrue();
			result.HasSwitch("c/d").ShouldBeTrue();
		}

		[Fact]
		public void Parsing_Switches_Into_Separate_Collections()
		{
			// Arrange / Act
			var result = ParseCommandLine("verb -a a-one a-two -b b-one b-two -c c-one");

			// Assert
			result.Switches.Length.ShouldEqual(4);

			result.Verbs.Length.ShouldEqual(1);
			result.Verbs[0].ShouldEqual("verb");	

			result.HasSwitch("a").ShouldBeTrue();
			result.Switch("a").Length.ShouldEqual(2);
			result.Switch("a").ShouldContain("a-one");
			result.Switch("a").ShouldContain("a-two");

			result.HasSwitch("b").ShouldBeTrue();
			result.Switch("b").Length.ShouldEqual(2);
			result.Switch("b").ShouldContain("b-one");
			result.Switch("b").ShouldContain("b-two");

			result.HasSwitch("c").ShouldBeTrue();
			result.Switch("c").Length.ShouldEqual(1);
			result.Switch("c").ShouldContain("c-one");
		}

		[Fact]
		public void Parsing_Duplicate_Switches_Merges_Values()
		{
			// Arrange / Act
			var result = ParseCommandLine("-a one -a two -b three -a four");

			// Assert
			result.Switches.Length.ShouldEqual(2);

			result.HasSwitch("a").ShouldBeTrue();
			result.Switch("a").Length.ShouldEqual(3);
			result.Switch("a")[0].ShouldEqual("one");
			result.Switch("a")[1].ShouldEqual("two");
			result.Switch("a")[2].ShouldEqual("four");

			result.HasSwitch("b").ShouldBeTrue();
			result.Switch("b").Length.ShouldEqual(1);
			result.Switch("b")[0].ShouldEqual("three");
		}

		[Fact]
		public void Parsing_Switches_With_Colon_Adds_Switch_And_Value()
		{
			// Arrange/Act
			var result = ParseCommandLine("-options:one,two,three -moreoptions:four,five");

			// Assert
			result.Switches.Length.ShouldEqual(2);

			result.HasSwitch("options").ShouldBeTrue();
			result.Switch("options").Length.ShouldEqual(1);
			result.Switch("options")[0].ShouldEqual("one,two,three");

			result.HasSwitch("moreoptions").ShouldBeTrue();
			result.Switch("moreoptions").Length.ShouldEqual(1);
			result.Switch("moreoptions")[0].ShouldEqual("four,five");
		}

		[Fact]
		public void Parsing_Switches_With_Colon_Adds_Switch_Up_To_First_Colon_Rest_As_Value()
		{
			// Arrange/Act
			var result = ParseCommandLine(@"-file:d:\somepath\file.txt -drives:d:,c:,e:");

			// Assert
			result.Switches.Length.ShouldEqual(2);

			result.HasSwitch("file").ShouldBeTrue();
			result.Switch("file").Length.ShouldEqual(1);
			result.Switch("file")[0].ShouldEqual(@"d:\somepath\file.txt");

			result.HasSwitch("drives").ShouldBeTrue();
			result.Switch("drives").Length.ShouldEqual(1);
			result.Switch("drives")[0].ShouldEqual("d:,c:,e:");
		}

		[Fact]
		public void Parsing_Switches_Are_Case_Sensitive()
		{
			var result = ParseCommandLine("-c -C -a -A");

			// Assert
			result.Switches.Length.ShouldEqual(4);
		}

		[Fact]
		public void Complex_CommandLine_ParsesCorrectly()
		{
			// Arrange/Act
			var result = ParseCommandLine("commit -m \"Some message\" -m2:\"Some commit message goes here\" -r:1 -r 2 :token1 :token2 -can-do true");

			// Assert
			result.Switches.Length.ShouldEqual(5);
			
			result.Verbs.Length.ShouldEqual(1);
			result.Verbs[0].ShouldEqual("commit");

			result.HasSwitch("m").ShouldBeTrue();
			result.Switch("m").Length.ShouldEqual(1);
			result.Switch("m")[0].ShouldEqual("Some message");

			result.HasSwitch("m2").ShouldBeTrue();
			result.Switch("m2").Length.ShouldEqual(1);
			result.Switch("m2")[0].ShouldEqual("Some commit message goes here");

			result.HasSwitch("r").ShouldBeTrue();
			result.Switch("r").Length.ShouldEqual(4);
			result.Switch("r")[0].ShouldEqual("1");
			result.Switch("r")[1].ShouldEqual("2");
			result.Switch("r")[2].ShouldEqual(":token1");
			result.Switch("r")[3].ShouldEqual(":token2");

			result.HasSwitch("can-do").ShouldBeTrue();
			result.Switch("can-do").Length.ShouldEqual(1);
			result.Switch("can-do")[0].ShouldEqual("true");
		}

		[Fact]
		public void Parser_Can_Flatten_Multi_Valued_Switch()
		{
			// Arrange/Act
			var result = ParseCommandLine("-fullname John Dorian Doe");

			// Assert
			result.Switches.Length.ShouldEqual(1);
			result.HasSwitch("fullname").ShouldBeTrue();
			result.Switch("fullname").Length.ShouldEqual(3);
			result.SwitchMerged("fullname").ShouldEqual("John Dorian Doe");
		}
	}
}
