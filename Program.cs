class Program
{
	static void Main()
	{
		var addressBook = new AddressBook();
		addressBook.Add(new Contact("Kalle", "Anka", "kalle@anka.se", new DateTime(1980, 1, 1)));
		addressBook.Add(new Contact("Kajsa", "Anka", "kajsa@anka.se", new DateTime(1985, 2, 2)));

		foreach (var c in addressBook.GetAll())
		{
			Console.WriteLine(c);
		}
	}
}

class Contact(string firstName, string lastName, string email, DateTime birthDate)
{
	public string FirstName { get; } = firstName;
	public string LastName { get; } = lastName;
	public string Email { get; } = email;
	public DateTime BirthDate { get; } = birthDate;

}

class AddressBook
{
	private readonly List<Contact> _contacts = new();

	public void Add(Contact contact)
	{
		if (string.IsNullOrWhiteSpace(contact.Email) || !contact.Email.Contains("@"))
		{
			return;
		}

		_contacts.Add(contact);
	}

	public IEnumerable<Contact> GetAll()
	{
		return _contacts;
	}

	public Contact? FindByEmail(string email)
	{
		foreach (var c in _contacts)
		{
			if (c.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
			{
				return c;
			}
		}
		return null;
	}
}

class TestEmailService
{
	public void SendEmail(string to, string subject, string body)
	{

	}
}
