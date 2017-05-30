namespace Event.Infrastructure
{
    using System;

    public class DomainError : Exception
	{
		public static DomainError Named(string name, string format, params object[] args)
		{
			return new DomainError("[" + name + "] " + string.Format(format, args))
			{
				Name = name
			};
		}


		public DomainError() { }
		public DomainError(string message) : base(message) { }
		public DomainError(string format, params object[] args) : base(string.Format(format, args)) { }
		public DomainError(string message, Exception inner) : base(message, inner) { }


		public string Name { get; private set; }
	}
}
