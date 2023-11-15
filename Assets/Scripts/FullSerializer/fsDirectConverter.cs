using System;

namespace FullSerializer
{
	public abstract class fsDirectConverter : fsBaseConverter
	{
		public abstract Type ModelType { get; }
	}
}
