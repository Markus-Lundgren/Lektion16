using System.Diagnostics.Contracts;
using System.Text;

class Program
{
	static void Main()
	{
		IEmailService emailService = new RealEmailService();
		ILogger logger = new FileLogger();
		IContactRepository repository = new FileContactRepository();
		var addressBook = new AddressBook(emailService, repository, logger);
		addressBook.Add(new Contact("Kalle", "Anka", "kalle@anka.se", new DateTime(1980, DateTime.Now.Month, DateTime.Now.Day)));
		addressBook.Add(new Contact("Kajsa", "Anka", "kajsa@anka.se", new DateTime(1985, 2, 2)));

		foreach (var c in repository.GetAll())
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
	private readonly IEmailService _emailService;
	private readonly ILogger _logger;
	private readonly IContactRepository _repository;
	public AddressBook(IEmailService emailService, IContactRepository repository, ILogger logger)
	=> (_emailService, _repository, _logger) = (emailService, repository, logger);
	public void Add(Contact contact)
	{
		_repository.Add(contact);
		_logger.Log($"{DateTime.Now}: Lade till kontakt {contact.FirstName} {contact.LastName}");
	}

	public Contact? FindByEmail(string email)
	{
		foreach (var c in _repository.GetAll())
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
		foreach (Contact c in _repository.GetAll())
		{
			if (c.BirthDate.Month == DateTime.Now.Month && c.BirthDate.Day == DateTime.Now.Day)
			{
				_emailService.SendEmail(c.Email, "Grattis på födelsedagen!", $"Grattis {c.FirstName}! Du har blivit 1 år äldre");
			}
		}
	}
}
class ConsoleLogger : ILogger
{
	public void Log(string message)
	{
		Console.WriteLine(message);
	}
}
class FileLogger : ILogger
{
	public void Log(string message)
	{
		using (StreamWriter sw = File.AppendText("log.txt"))
		{
			sw.WriteLine(message);
		}
	}
}

class TestEmailService : IEmailService
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

class RealEmailService : IEmailService
{
	public void SendEmail(string to, string subject, string body)
	{
		Console.ForegroundColor = ConsoleColor.DarkGreen;
		Console.WriteLine("Mail sent");
		Console.ResetColor();
	}
}

class InMemoryContactRepository : IContactRepository
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
}

class FileContactRepository : IContactRepository
{
	private readonly List<Contact> _contacts = new();
	public void Add(Contact contact)
	{
		if (string.IsNullOrWhiteSpace(contact.Email) || !contact.Email.Contains("@"))
		{
			return;
		}

		if (File.Exists("contacts.csv"))
		{
			bool exists = GetAll().Any(c => c.FirstName == contact.FirstName &&
			c.LastName == contact.LastName && c.Email == contact.Email);

			if (exists) return;
		}
		using (StreamWriter sw = File.AppendText("contacts.csv"))
		{
			sw.WriteLine($"{contact.FirstName},{contact.LastName},{contact.Email},{contact.BirthDate}");
		}
	}

	public IEnumerable<Contact> GetAll()
	{
		List<Contact> _contacts = new();

		string[] contacts = File.ReadAllLines("contacts.csv");
		foreach (string line in contacts)
		{
			var values = line.Split(',');
			Contact c = new(values[0], values[1], values[2], DateTime.Parse(values[3]));
			_contacts.Add(c);
		}

		return _contacts;
	}
}
