using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deepcode.CommandLine.Binding;
using Deepcode.CommandLine.Documentation;

namespace Deepcode.CommandLine.Tests.Documentation
{
	public class CommandLineDocumentGeneratorFixture
	{
		private readonly CommandLineDocumentGenerator _doc;

		public class TestCommandLineBase
		{
			[ParameterAlias("port")]
			[ParameterAlias("p")]
			[Description("Specify the port number to serve content from - defaults to 8080")]
			public int Port { get; set; }

			[ParameterAlias("host")]
			[ParameterAlias("h")]
			[Description("Specify the host name to listen on - defaults to *")]
			public string HostName { get; set; }
		}

		public CommandLineDocumentGeneratorFixture()
		{
			_doc = new CommandLineDocumentGenerator();
		}

		public void Given_Command_Documents_Parameters()
		{
			// Arrange
			// Act
			_doc.GenerateHelpFor(typeof (TestCommandLineBase));

			// Assert
		}




	}
}
