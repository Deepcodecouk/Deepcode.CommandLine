using Should;
using Xunit;

namespace Deepcode.CommandLine.Tests.Binding
{
	public class UnboundArgumentsFixture
	{
		[Fact]
		public void Can_Discover_Unbound_Verbs()
		{
			true.ShouldBeFalse("TODO");
			// verb1 verb2 verb3
			// 1 + 3 bound
			// 2 isn't - should be able to find it.
		}

		[Fact]
		public void Can_Discover_Unbound_Parameters()
		{
			true.ShouldBeFalse("TODO");
			// -c is unknown
			// Should be able to find it
		}
	}
}
