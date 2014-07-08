using System;

namespace Deepcode.CommandLine.Binding
{
	/// <summary>
	/// Marks a property on a target binding class as receiveing the verb/verbs
	/// from the command line arguments. Verbs being any arguments not prefixed
	/// with a switch...
	/// 
	/// The binding can be scoped to positional verbs - give it a start and
	/// end position to pull only the verbs in those locations.
	/// 
	/// If you are binding to a single value and specify a range, the first item
	/// found will win. This also applies if you specify a start position and
	/// no end position - the first found will win.
	/// 
	/// If you are binding to an array and don't specify an end position, 
	/// then the binding will be greedy with the verbs.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ParameterVerbAttribute : Attribute
	{
		private int _startPosition = 1;
		private int _endPosition = Int32.MaxValue;
		private bool _greedy = false;

		/// <summary>
		/// The start position for the verb to bind. If set
		/// and endposition is max and greedy not set, this
		/// defaults the end position to an effective length
		/// of one verb. 
		/// </summary>
		public int StartPosition 
		{
			get
			{
				return _startPosition;
			}
			set
			{
				_startPosition = value;
				if (_endPosition == Int32.MaxValue && !_greedy) _endPosition = _startPosition;
			}
		}

		/// <summary>
		/// Sets the end position for the final verb to bind. If
		/// set, unmarks greedy indicator.
		/// </summary>
		public int EndPosition
		{
			get
			{
				return _endPosition;
			}
			set
			{
				_endPosition = value;
				_greedy = false;
			}
		}

		/// <summary>
		/// Setting greedy true will send end position to max.
		/// </summary>
		public bool Greedy
		{
			get
			{
				return _greedy;
			}
			set
			{
				_greedy = value;
				if( _greedy )
					_endPosition = Int32.MaxValue;
			}
		}
	}
}