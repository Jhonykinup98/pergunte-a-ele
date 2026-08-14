namespace PergunteAele.Api.Services;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string confirmationToken);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, HttpClient httpClient, ILogger<EmailService> logger)
    {
        _config = config;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationToken)
    {
        var frontendUrl = _config["Frontend:Url"];
        var confirmationLink = $"{frontendUrl}/confirmar-email?token={confirmationToken}";
        var apiKey = _config["Email:ApiKey"];
        var from = _config["Email:From"];

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.StartsWith("SUA_"))
        {
            _logger.LogWarning("==== LINK DE CONFIRMAÇÃO para {Email}: {Link} ====", toEmail, confirmationLink);
            return;
        }

        try
        {
            var payload = new
            {
                from,
                to = new[] { toEmail },
                subject = "Confirme seu e-mail - PergunteAele",
                html = $"<p>Bem-vindo! Clique para confirmar: <a href=\"{confirmationLink}\">{confirmationLink}</a></p>"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8, "application/json");

            await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao enviar e-mail para {Email}. Link: {Link}", toEmail, confirmationLink);
        }
    }
}