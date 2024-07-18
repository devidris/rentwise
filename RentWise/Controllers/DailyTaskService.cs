using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RentWise.DataAccess.Repository;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DailyTaskService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly ILogger<DailyTaskService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    // Constructor with dependency injection
    public DailyTaskService(ILogger<DailyTaskService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
}

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromDays(1)); // Run daily

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        // Place your logic here.
        _logger.LogInformation("Running daily task at: {Time}", DateTime.Now);
        CheckAndUpdatePremiumStatus();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public void CheckAndUpdatePremiumStatus()
    {
        IEnumerable<ProductModel> products = _unitOfWork.Product.GetAll(u => u.Premium == true, "Agent");

        foreach (ProductModel product in products)
        {
            if (product.PremiumExpiry <= DateTime.UtcNow)
            {
                // Premium has expired
                product.Premium = false;
                _unitOfWork.Product.Update(product);
            }
            else
            {
                // Calculate the days until the premium expires
                int daysUntilExpiry = (product.PremiumExpiry - DateTime.UtcNow).Days;

                // Check if the premium is expiring in 7 days or 3 days
                if (daysUntilExpiry == 7 || daysUntilExpiry == 3)
                {
                    // Prepare the email content
                    string subject = $"Your product's premium status will expire in {daysUntilExpiry} days";
                    string body = $"Hello, <br/><br/> Your product '{product.Name}' will lose its premium status in {daysUntilExpiry} days.<br/> Please take necessary action if needed.<br/><br/>Regards,<br/>Rentwise";
                    AgentRegistrationModel agentRegistration = _unitOfWork.AgentRegistration.Get(u=>u.Id == product.AgentId,"User");
                    // Send email notification
                    SharedFunctions.SendEmail(agentRegistration.User.NormalizedUserName, subject, body);
                }
            }
        }

        // Save changes to the database after updating the products
        _unitOfWork.Save();
    }
}
