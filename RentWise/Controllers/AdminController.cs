using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentWise.DataAccess.Repository.IRepository;
using RentWise.Models;
using RentWise.Models.Identity;
using RentWise.Utility;
using Microsoft.AspNetCore.SignalR;
using System.Web;
using Microsoft.CodeAnalysis;
using System.Drawing;

namespace RentWise.Controllers
{
    //[Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> _hubContext;


        public AdminController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, Microsoft.AspNetCore.SignalR.IHubContext<SignalRHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            IEnumerable<UsersDetailsModel> usersDetails = _unitOfWork.UsersDetails.GetAll(u => !String.IsNullOrEmpty(u.Id));
            return View(usersDetails);
        }

        public IActionResult LocationManager()
        {
            IEnumerable<State> states = _unitOfWork.State.GetAll(u=>u.StateId != null,"Cities");
            ViewBag.States = states.Count() > 0 ? states : null;
            return View();
        }

        public async Task<IActionResult> AddState(State state)
        {
            if(ModelState.IsValid)
            {
                state.Name = state.Name.Trim();
                state.Name = state.Name.ToLower();
                _unitOfWork.State.Add(state);
                _unitOfWork.Save();
                return RedirectToAction(nameof(LocationManager),  new {message = "States added successfully"});
            }
            return RedirectToAction(nameof(LocationManager), new {error = "State name is compulsory"});
        }

        public async Task<IActionResult> AddCity(City city)
        {
            if (!String.IsNullOrEmpty(city.Name) && city.StateId != null) { 
                city.Name = city.Name.Trim();
                city.Name = city.Name.ToLower();
                _unitOfWork.City.Add(city);
            _unitOfWork.Save();
            return RedirectToAction(nameof(LocationManager), new {message = "City added sccuessfully"});
        }
            return RedirectToAction(nameof(LocationManager), new { error = "City name is compulsory" });
      
        }

        public async Task<IActionResult> DeleteCity(City city)
        {
            if (city.CityId != null)
            {
               city = _unitOfWork.City.Get(u => u.CityId == city.CityId);
                _unitOfWork.City.Remove(city);
                _unitOfWork.Save();
                return RedirectToAction(nameof(LocationManager), new {message="City Removed successfully"});
            }
            return RedirectToAction(nameof(LocationManager), new {error = "Something went wrong"});
        }
        public async Task<IActionResult> DeleteState(State state)
        {
            if (state.StateId != null)
            {
               state = _unitOfWork.State.Get(u => u.StateId == state.StateId);
            City city = _unitOfWork.City.Get(u => u.StateId == state.StateId);
                if(city != null)
                {
                    _unitOfWork.City.Remove(city);
                }
                _unitOfWork.State.Remove(state);
                _unitOfWork.Save();
                return RedirectToAction(nameof(LocationManager), new {message = "State and all cities under it deleted"});
            }
            return RedirectToAction(nameof(LocationManager),new {error= "Something went wrong"});
        }
        public async Task<IActionResult> DeactivateAll(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.LockoutEnabled = true;
                await _userManager.UpdateAsync(user);
            }
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id);
            usersDetails.Enabled = false;
            usersDetails.UpdatedAt = DateTime.Now;
            _unitOfWork.UsersDetails.Update(usersDetails);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ActivateAll(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                user.LockoutEnabled = false;
                await _userManager.UpdateAsync(user);
            }
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id);
            usersDetails.Enabled = true;
            usersDetails.UpdatedAt = DateTime.Now;
            _unitOfWork.UsersDetails.Update(usersDetails);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeactivateAgent(string Id)
        {
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == Id);
            agent.Enabled = false;
            agent.UpdatedAt = DateTime.Now;
            _unitOfWork.AgentRegistration.Update(agent);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ActivateAgent(string Id)
        {
            AgentRegistrationModel agent = _unitOfWork.AgentRegistration.Get(u => u.Id == Id);
            agent.Enabled = true;
            agent.UpdatedAt = DateTime.Now;
            _unitOfWork.AgentRegistration.Update(agent);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MakeAdmin(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, Lookup.Roles[1]);

                await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[3]);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveAdmin(string Id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, Lookup.Roles[3]);

                await _userManager.RemoveFromRoleAsync(user, Lookup.Roles[1]);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Contact()
        {
            IEnumerable<ContactAdminModel> contactAdmins = _unitOfWork.ContactAdmin.GetAll();
            return View(contactAdmins);
        }

        public async Task<IActionResult> Preview(string Id)
        {
            UsersDetailsModel usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id, "Agent");
            if(usersDetails == null)
            {
                usersDetails = _unitOfWork.UsersDetails.Get(u => u.Id == Id);
            }
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            ViewBag.User = user;
            string profilePicture = $"images/{usersDetails.Id}";
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string finalPath = Path.Combine(wwwRootPath, profilePicture);
            if (Directory.Exists(finalPath))
            {
                profilePicture = $"~/images/{usersDetails.Id}/{String.Join("", Lookup.Upload[5].Split(" "))}.png";
            }
            else
            {
                profilePicture = "~/img/profile.png";
            }
            ViewBag.ProfilePicture = profilePicture;
            IEnumerable<ProductModel> products = _unitOfWork.Product.GetAll(u => u.AgentId == Id);
            ViewBag.Products = products;
            return View(usersDetails);
        }

        public async Task<ActionResult> SendNotificationToAll(string message)
        {
            _ = _hubContext.Clients.All.SendAsync("ReceiveMessage", "all", message);
            _ = SharedFunctions.SendPushNotification("All", "Message from rentwise", message);
            return RedirectToAction(nameof(Contact));
        }
        public async Task<IActionResult> ReplyEmail(string name, string email, string header, string body,int Id)
        {
            string emailContent = SharedFunctions.EmailContentReply(name, header, body);
            ContactAdminModel contact = _unitOfWork.ContactAdmin.Get(u => u.Id == Id);
            contact.Enabled = false;
            _unitOfWork.ContactAdmin.Update(contact);
            _unitOfWork.Save();
            if (email == "All")
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var user in users)
                {
                    SharedFunctions.SendEmail(user.Email, "  Message from rentwise", emailContent);
                }

            }
            else
            {
                SharedFunctions.SendEmail(email, "  Message from rentwise", emailContent);
            }
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Display()
        {

            string[] imagePaths = Directory.GetFiles("wwwroot/images/hero");
            string[] serviceImgPath = Directory.GetFiles("wwwroot/images/services");
            List<ImageModel> images = imagePaths.Select(path => new ImageModel
            {
                FileName = Path.GetFileName(path),
                FilePath = $"~/images/hero/{Path.GetFileName(path)}",
            }).ToList();

            List<ImageModel> serviceImages = serviceImgPath.Select(path => new ImageModel
            {
                FileName = Path.GetFileName(path),
                FilePath = $"~/images/services/{Path.GetFileName(path)}",
            }).ToList();

            ViewBag.Images = images;
            ViewBag.ServiceImages = serviceImages;
            return View();
        }

        public ActionResult UploadImage(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file provided.";
                return RedirectToAction("Display", "Admin");
            }

            if (file.Length > 2097152) // 2 MB in bytes
            {
                TempData["ErrorMessage"] = "File size must not exceed 2 MB.";
                return RedirectToAction("Display", "Admin");
            }
            const int requiredWidth = 1920; // Required width in pixels
            const int requiredHeight = 1080; // Required height in pixels

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin); // Reset the position of MemoryStream to the beginning after copying

                using (Image img = Image.FromStream(memoryStream))
                {
                    if (img.Width != requiredWidth || img.Height != requiredHeight)
                    {
                        TempData["ErrorMessage"] = $"Image dimensions are incorrect. Required dimensions: {requiredWidth}x{requiredHeight} pixels.";
                        return RedirectToAction("Display", "Admin");
                    }
                }
            }

            // Continue processing if dimensions are correct
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\hero\";
            string finalPath = Path.Combine(wwwRootPath, filePath);
            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string fullPath = Path.Combine(finalPath, fileName);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return RedirectToAction("Display", "Admin");
        }

        public ActionResult UploadServiceImage(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file provided.";
                return RedirectToAction("Display", "Admin");
            }

            // Check if file size exceeds 500 KB
            if (file.Length > 512000) // 500 KB in bytes
            {
                TempData["ErrorMessage"] = "File size must not exceed 500 KB.";
                return RedirectToAction("Display", "Admin");
            }
            const int minimumDimension = 200; // Required width in pixels

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin); // Reset the position of MemoryStream to the beginning after copying

                using (Image img = Image.FromStream(memoryStream))
                {
                    if (img.Width <= minimumDimension || img.Height <= minimumDimension)
                    {
                        TempData["ErrorMessage"] = $"Image dimensions are incorrect. Minimum Required dimensions: {minimumDimension}x{minimumDimension} pixels.";
                        return RedirectToAction("Display", "Admin");
                    }
                }
            }

            // Continue processing if dimensions are correct
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = @"images\services\";
            string finalPath = Path.Combine(wwwRootPath, filePath);
            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string fullPath = Path.Combine(finalPath, fileName);

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return RedirectToAction("Display", "Admin");
        }

        public ActionResult DeleteImage(string fileName)
        {
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "hero");
            string[] imagePaths = Directory.GetFiles(directoryPath);

            if (imagePaths.Length <= 1) // If only one or no files exist, redirect to admin display
            {
                return RedirectToAction("Display", "Admin");
            }

            string targetFilePath = Path.Combine(directoryPath, fileName);

            if (System.IO.File.Exists(targetFilePath))
            {
                System.IO.File.Delete(targetFilePath);

                // After deletion, check if the deleted file was the 'hero'
                if (fileName.ToLower() == "hero")
                {
                    // Select a new hero image from the remaining files
                    foreach (var filePath in imagePaths)
                    {
                        if (filePath != targetFilePath) // Ensure the file is not the one just deleted
                        {
                            string newHeroFilePath = Path.Combine(directoryPath, "hero" + Path.GetExtension(filePath));
                            System.IO.File.Move(filePath, newHeroFilePath); // Rename the first available file to 'hero'
                            break; // Exit after setting new hero file
                        }
                    }
                }
            }

            return RedirectToAction("Display", "Admin");
        }

        public ActionResult DeleteServiceImage(string fileName)
        {
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "services");
            string[] imagePaths = Directory.GetFiles(directoryPath);

            if (imagePaths.Length <= 1) // If only one or no files exist, redirect to admin display
            {
                return RedirectToAction("Display", "Admin");
            }

            string targetFilePath = Path.Combine(directoryPath, fileName);

            if (System.IO.File.Exists(targetFilePath))
            {
                System.IO.File.Delete(targetFilePath);

                // After deletion, check if the deleted file was the 'hero'
                if (fileName.ToLower() == "hero")
                {
                    // Select a new hero image from the remaining files
                    foreach (var filePath in imagePaths)
                    {
                        if (filePath != targetFilePath) // Ensure the file is not the one just deleted
                        {
                            string newHeroFilePath = Path.Combine(directoryPath, "hero" + Path.GetExtension(filePath));
                            System.IO.File.Move(filePath, newHeroFilePath); // Rename the first available file to 'hero'
                            break; // Exit after setting new hero file
                        }
                    }
                }
            }

            return RedirectToAction("Display", "Admin");
        }


        public ActionResult SetActive(string fileName)
        {
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "hero");
            string[] imagePaths = Directory.GetFiles(directoryPath);

            if (imagePaths.Length <= 1)
            {
                return RedirectToAction("Display", "Admin");
            }

            string targetFilePath = Path.Combine(directoryPath, fileName);
            string heroFilePath = null;

            // Find any existing file that has 'hero' in its name
            foreach (var path in imagePaths)
            {
                if (Path.GetFileName(path).Split('.')[0] == "hero")
                {
                    heroFilePath = path;
                    break;
                }
            }

            // Check if 'hero' file exists and rename it to a random name with its original extension
            if (heroFilePath != null)
            {
                string oldHeroExtension = Path.GetExtension(heroFilePath); // Get the file extension of the old hero
                string newHeroName;
                do
                {
                    newHeroName = Path.Combine(directoryPath, GetRandomFileName() + oldHeroExtension); // Append the original extension
                }
                while (System.IO.File.Exists(newHeroName));

                System.IO.File.Move(heroFilePath, newHeroName); // Rename old 'hero' to a random name with extension
            }

            // Rename the target file to 'hero', now that the old 'hero' is renamed
            if (System.IO.File.Exists(targetFilePath))
            {
                string targetFileExtension = Path.GetExtension(targetFilePath); // Get the file extension of the target file
                string newHeroFilePath = Path.Combine(directoryPath, "hero" + ".png"); // Append the extension to 'hero'

                // Ensure 'newHeroFilePath' does not already exist
                if (System.IO.File.Exists(newHeroFilePath))
                {
                    // Handle if 'newHeroFilePath' already exists (possibly delete it or handle as needed)
                    System.IO.File.Delete(newHeroFilePath);
                }

                System.IO.File.Move(targetFilePath, newHeroFilePath); // Rename target file to 'hero' with extension
            }

            return RedirectToAction("Display", "Admin");
        }

        public ActionResult SetOrder(string fileName,string order)
        {
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "services");
            string[] imagePaths = Directory.GetFiles(directoryPath);

            if (imagePaths.Length <= 1)
            {
                return RedirectToAction("Display", "Admin");
            }

            string targetFilePath = Path.Combine(directoryPath, fileName);
            string heroFilePath = null;

            // Find any existing file that has 'hero' in its name
            foreach (var path in imagePaths)
            {
                if (Path.GetFileName(path).Split('.')[0] == order)
                {
                    heroFilePath = path;
                    break;
                }
            }

            // Check if 'hero' file exists and rename it to a random name with its original extension
            if (heroFilePath != null)
            {
                string oldHeroExtension = Path.GetExtension(heroFilePath); // Get the file extension of the old hero
                string newHeroName;
                do
                {
                    newHeroName = Path.Combine(directoryPath, GetRandomFileName() + oldHeroExtension); // Append the original extension
                }
                while (System.IO.File.Exists(newHeroName));

                System.IO.File.Move(heroFilePath, newHeroName); // Rename old 'hero' to a random name with extension
            }

            // Rename the target file to 'hero', now that the old 'hero' is renamed
            if (System.IO.File.Exists(targetFilePath))
            {
                string targetFileExtension = Path.GetExtension(targetFilePath); // Get the file extension of the target file
                string newHeroFilePath = Path.Combine(directoryPath, order + ".png"); // Append the extension to 'hero'

                // Ensure 'newHeroFilePath' does not already exist
                if (System.IO.File.Exists(newHeroFilePath))
                {
                    // Handle if 'newHeroFilePath' already exists (possibly delete it or handle as needed)
                    System.IO.File.Delete(newHeroFilePath);
                }

                System.IO.File.Move(targetFilePath, newHeroFilePath); // Rename target file to 'hero' with extension
            }

            return RedirectToAction("Display", "Admin");
        }



        private string GetRandomFileName()
        {
            return Path.GetRandomFileName().Replace(".", "");  // Generates a random string suitable for a file name
        }


    }

}
