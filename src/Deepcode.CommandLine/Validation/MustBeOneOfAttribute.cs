using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Deepcode.CommandLine.Validation
{
	public class MustBeOneOfAttribute : ValidationAttribute
	{
		public object[] Options { get; set; }

		public MustBeOneOfAttribute(params object [] options)
		{
			Options = options;
		}

		public override bool IsValid(object value)
		{
			return Options.Any(o => o.Equals(value));
		}
	}
}