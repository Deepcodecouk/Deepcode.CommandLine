using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deepcode.CommandLine.Tests.CommandDictionaryTests.TestCommands;
using Xunit;

namespace Deepcode.CommandLine.Tests.CommandDictionaryTests
{
	public class ManualCommandManagementFixture
	{
		[Fact]
		public void Can_add_command_manually()
		{
			var commands = new CommandDictionary();
			commands.AddCommand(
				new CommandDictionaryEntry(typeof (SimpleCommand))
					.WithAlias("command")
					.WithAlias("command2"));

			// Should have one entry
		}

		public void Can_add_multiple_aliases()
		{
		}

		public void Cannot_add_two_commands_same_alias()
		{
		}
	}
}
