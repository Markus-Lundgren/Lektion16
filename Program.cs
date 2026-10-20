using System.Text;

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
		addressBook.SendBirthdayEmails();
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

	public IEnumerable<Contact> GetAll() => _contacts;

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

	public void SendBirthdayEmails()
	{
		foreach (Contact c in _contacts)
		{
			if (c.BirthDate.Month == DateTime.Now.Month && c.BirthDate.Day == DateTime.Now.Day)
			{
				TestEmailService t = new();
				t.SendEmail(c.Email, "Grattis på födelsedagen!", $"Grattis {c.FirstName}! Du har blivit 1 år äldre");
			}
		}
	}
}

class TestEmailService
{
	public void SendEmail(string to, string subject, string body)
	{
		StringBuilder sb = new();
		sb.AppendLine($"To: {to}");
		sb.AppendLine($"Subject: {subject}");
		sb.AppendLine("------------------------");
		sb.AppendLine(body);

		Console.WriteLine(sb);

		Console.WriteLine("Mail sent");
	}
}
