using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Deepcode.CommandLine.Validation
{
	public class ObjectValidatorResult
	{
		public bool IsValid { get; set; }
		public ICollection<ValidationResult> Results { get; set; }

		public ObjectValidatorResult(ICollection<ValidationResult> results)
		{
			Results = results;
			IsValid = results.Count == 0;
		}
	}

	public class ObjectValidator
	{	
		public ObjectValidatorResult Validate(object objectToValidate)
		{
			var results = new List<ValidationResult>();
			var context = new ValidationContext(objectToValidate);

			Validator.TryValidateObject(objectToValidate, context, results, true);
			
			return new ObjectValidatorResult(results);
		}
	}
}
