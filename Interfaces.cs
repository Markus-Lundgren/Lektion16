interface IEmailService
{
	void SendEmail(string to, string subject, string body);
}

interface ILogger
{
	void Log(string message);
}

interface IContactRepository
{
	void Add(Contact contact);
	IEnumerable<Contact> GetAll();
}