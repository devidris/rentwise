using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;

namespace RentWise.Utility
{
    public class SharedFunctions
    {
        public static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
        public static string CapitalizeAllWords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }

            return string.Join(" ", words);
        }
        public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in kilometers

            var dLat = (lat2 - lat1) * (Math.PI / 180);
            var dLon = (lon2 - lon1) * (Math.PI / 180);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * (Math.PI / 180)) * Math.Cos(lat2 * (Math.PI / 180)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = R * c; // Distance in kilometers

            return distance;
        }
        public static double GetDoubleValue(string stringValue)
        {
            if (double.TryParse(stringValue, out double result))
            {
                return result;
            }
            else
            {
                // Handle the case where parsing fails, you might want to log an error or return a default value.
                return 0.0; // Or any other appropriate default value
            }
        }
        public static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalMinutes < 1)
            {
                return $"{(int)duration.TotalSeconds}s";
            }
            else if (duration.TotalHours < 1)
            {
                return $"{(int)duration.TotalMinutes}m";
            }
            else if (duration.TotalDays < 1)
            {
                return $"{(int)duration.TotalHours}h";
            }
            else if (duration.TotalDays < 7)
            {
                return $"{(int)duration.TotalDays}d";
            }
            else if (duration.TotalDays < 365)
            {
                return $"{(int)(duration.TotalDays / 7)}w";
            }
            else
            {
                return $"{(int)(duration.TotalDays / 365)}y";
            }
        }
        public static List<T> ShuffleList<T>(List<T> list)
        {
            Random random = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
        public static bool SendEmail(string toEmail, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                // Set up your email credentials and server details
                string smtpServer = "smtpout.secureserver.net";
                int smtpPort = 587;
                string smtpUsername = "sethoo";
                string smtpPassword = "Cowboy@123456";

                // Sender and recipient email addresses
                string fromEmail = "support@rentwisegh.com";

                // Create a MimeMessage
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("", fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                // Create the body part
                TextPart bodyPart = new TextPart(isBodyHtml ? "html" : "plain");
                bodyPart.Text = body;

                // Attach the body part to the message
                Multipart multipart = new Multipart("mixed");
                multipart.Add(bodyPart);
                message.Body = multipart;

                // Create an instance of SmtpClient and configure it
                using (MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtpClient.Connect(smtpServer, smtpPort, false);
                    smtpClient.Authenticate(fromEmail, smtpPassword);

                    // Send the email
                    smtpClient.Send(message);
                    smtpClient.Disconnect(true);
                }

                return true; // Assuming success (you may want to handle exceptions appropriately)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Log the exception or take appropriate action for debugging
                return false;
            }
        }
        public static string EmailContent(string Name,int Lkptype,string ProductName,int ProductQuantity, double TotalAmount)
        {
            string header = string.Empty;
            string body = string.Empty;
            string body2 = string.Empty;
            string buttonText = string.Empty;
            string buttonLink = string.Empty;
            if (Lkptype == 1)
            {
                header = $"Thank you for choosing RentWise!";
                body = "We appreciate your trust in our service. Your reservation details are as follows:";
                body += $"\n\nProduct: {ProductName}"; 
                body += $"\nQuantity: {ProductQuantity}";
                body += $"\nTotal Amount: ₵{TotalAmount}"; 
                body += "\n\nYour reservation is currently being processed, and we will notify you once it is ready for review.";
                body2 = "If you have any questions or if there's anything else you'd like to discuss, feel free to reach out. We are committed to making your rental experience exceptional!";
                buttonLink = "https://rentwisegh.com/Store/Orders";
                buttonText = "View reservation Details";
            }
            else if (Lkptype == 2)
            {
                header = $"New reservation Notification - {ProductName}";
                body = $"Dear Agent,\n\nA new reservation has been placed by {Name}. Please review the details below:";
                body += $"\n\nProduct: {ProductName}";
                body += $"\nQuantity: {ProductQuantity}";
                body += $"\nTotal Amount: ₵{TotalAmount}";
                body += "\n\nYou can manage this reservation through your agent dashboard. Thank you for your prompt attention!";
                buttonLink = "https://agent.rentwisegh.com/Dashboard/Index?active=4";
                buttonText = "Agent Dashboard";
            }
            else if (Lkptype == 3)
            {
                header = $"Order Accepted - {ProductName}";
                body = $"Dear {Name},\n\nGood news! The agent has accepted your order for {ProductName}.";
                body += "\n\nYou can now make payment to recieve your reservation. Thank you for choosing RentWise!";
                buttonLink = "https://rentwisegh.com/Store/Orders";
                buttonText = "View Order Details";
            }
            else if (Lkptype == 4)
            {
                header = $"Order Rejected - {ProductName}";
                body = $"Dear {Name},\n\nWe regret to inform you that the agent has rejected your order for {ProductName}.";
                body += "\n\nIf you have any concerns or would like to discuss this further, please reach out to our customer support. We apologize for any inconvenience.";
                body2 = "Thank you for your understanding.";
                buttonLink = "https://rentwisegh.com/Page/Contact";
                buttonText = "Contact Customer Support";
            }
            else if (Lkptype == 5)
            {
                header = $"Payment Received - {ProductName}";
                body = $"Dear Agent,\n\nGood news! The client has successfully paid for the order {ProductName}.";
                body += "\n\nYou can review the payment details on your agent dashboard. Thank you for your continued partnership!";
                buttonLink = "https://agent.rentwisegh.com/Dashboard/Index?active=4";
                buttonText = "Agent Dashboard";
            }
            else if (Lkptype == 6)
            {
                header = $"Payment with Cash - {ProductName}";
                body = $"Dear Agent,\n\nThe client has indicated that they want to pay with cash for the order {ProductName}.";
                body += "\n\nPlease coordinate with the client to arrange for the cash payment. Thank you for your attention!";
                buttonLink = "https://agent.rentwisegh.com/Dashboard/Index?active=4";
                buttonText = "Agent Dashboard";
            }
            else if (Lkptype == 7)
            {
                header = $"Payment Approval - {ProductName}";
                body = $"Dear {Name},\n\nGood news! The agent has approved your payment for the order {ProductName}.";
                body += "\n\nYour order is now marked as paid, and you can view the details on your account. Thank you for choosing RentWise!";
                buttonLink = "https://rentwisegh.com/Store/Orders";
                buttonText = "View Order Details";
            }
            string content = @"
<!DOCTYPE html>

<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]--><!--[if !mso]><!-->
<link href=""https://fonts.googleapis.com/css?family=Roboto"" rel=""stylesheet"" type=""text/css""/><!--<![endif]-->
<style>
		* {
			box-sizing: border-box;
		}

		body {
			margin: 0;
			padding: 0;
		}

		a[x-apple-data-detectors] {
			color: inherit !important;
			text-decoration: inherit !important;
		}

		#MessageViewBody a {
			color: inherit;
			text-decoration: none;
		}

		p {
			line-height: inherit
		}

		.desktop_hide,
		.desktop_hide table {
			mso-hide: all;
			display: none;
			max-height: 0px;
			overflow: hidden;
		}

		.image_block img+div {
			display: none;
		}

		@media (max-width:620px) {

			.desktop_hide table.icons-inner,
			.social_block.desktop_hide .social-table {
				display: inline-block !important;
			}

			.icons-inner {
				text-align: center;
			}

			.icons-inner td {
				margin: 0 auto;
			}

			.mobile_hide {
				display: none;
			}

			.row-content {
				width: 100% !important;
			}

			.stack .column {
				width: 100%;
				display: block;
			}

			.mobile_hide {
				min-height: 0;
				max-height: 0;
				max-width: 0;
				overflow: hidden;
				font-size: 0px;
			}

			.desktop_hide,
			.desktop_hide table {
				display: table !important;
				max-height: none !important;
			}
		}
	</style>
</head>
<body style=""background-color: #FFFFFF; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #FFFFFF;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""image_block block-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""width:100%;"">
<div align=""center"" class=""alignment"" style=""line-height:10px"">
<div style=""max-width: 488px;""><img alt=""Rentwise Logo"" src=""https://rentwisegh.com/img/logo.png"" style=""display: block; height: auto; border: 0; width: 100%;"" title=""Rentwise Logo"" width=""488""/></div>
</div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f8f8f8; color: #000000; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 20px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<div class=""spacer_block block-1"" style=""height:45px;line-height:45px;font-size:1px;""> </div>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""heading_block block-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-left:20px;padding-right:20px;text-align:center;width:100%;"">
<h1 style=""margin: 0; color: #2a3940; direction: ltr; font-family: Roboto, Tahoma, Verdana, Segoe, sans-serif; font-size: 23px; font-weight: normal; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0; mso-line-height-alt: 27.599999999999998px;""><span class=""tinyMce-placeholder"">Hey " +Name + @",</span></h1>
</td>}
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""paragraph_block block-3"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:15px;"">
<div style=""color:#2a3940;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:18px;line-height:120%;text-align:center;mso-line-height-alt:21.599999999999998px;"">
<p style=""margin: 0; word-break: break-word;""><span>" +header + @"</span></p>
<p style=""margin: 0; word-break: break-word;""> </p>
<p style=""margin: 0; word-break: break-word;""><span>" + body + @"<br/>Every word counts!</span></p>
<p style=""margin: 0; word-break: break-word;""><span>" + body2 + @"<br/>Every word counts!</span></p>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""button_block block-4"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:30px;text-align:center;"">
<div align=""center"" class=""alignment""><!--[if mso]>
<v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""www.example.com"" style=""height:46px;width:213px;v-text-anchor:middle;"" arcsize=""3%"" stroke=""false"" fillcolor=""#f3aa0c"">
<w:anchorlock/>
<v:textbox inset=""0px,0px,0px,0px"">
<center style=""color:#ffffff; font-family:Tahoma, Verdana, sans-serif; font-size:18px"">
<![endif]--><a href="""+buttonLink+@""" style=""text-decoration:none;display:inline-block;color:#ffffff;background-color:#f3aa0c;border-radius:1px;width:auto;border-top:0px solid #8a3b8f;font-weight:undefined;border-right:0px solid #8a3b8f;border-bottom:0px solid #8a3b8f;border-left:0px solid #8a3b8f;padding-top:5px;padding-bottom:5px;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:18px;text-align:center;mso-border-alt:none;word-break:keep-all;"" target=""_blank""><span style=""padding-left:40px;padding-right:40px;font-size:18px;display:inline-block;letter-spacing:normal;""><span style=""word-break:break-word;""><span data-mce-style="""" style=""line-height: 36px;"">" +buttonText + @"</span></span></span></a><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""text_block block-5"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"">
<div style=""font-family: sans-serif"">
<div class="""" style=""font-size: 14px; font-family: Roboto, Tahoma, Verdana, Segoe, sans-serif; mso-line-height-alt: 16.8px; color: #2a3940; line-height: 1.2;"">
</div>
</div>
</td>
</tr>
</table>
<div class=""spacer_block block-6"" style=""height:60px;line-height:60px;font-size:1px;""> </div>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""divider_block block-7"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:30px;padding-left:10px;padding-right:10px;padding-top:10px;"">
<div align=""center"" class=""alignment"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""95%"">
<tr>
<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 2px solid #E8E8E8;""><span> </span></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""social_block block-8"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"">
<div align=""center"" class=""alignment"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""social-table"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block;"" width=""46px"">
<tr>
<td style=""padding:0 7px 0 7px;""><a href=""https://www.instagram.com/rentwise_gh"" target=""_blank""><img alt=""Instagram"" height=""32"" src=""https://rentwisegh.com/img/instagram.png"" style=""display: block; height: auto; border: 0;"" title=""instagram"" width=""32""/></a></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""paragraph_block block-9"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:30px;padding-right:30px;padding-top:10px;"">
<div style=""color:#66787f;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;"">
<p style=""margin: 0; word-break: break-word;"">At Rentwise, we are committed to making high-quality services and items available for people to rent and make money in the most affordable and convenient way possible. </p>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""paragraph_block block-10"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"">
<div style=""color:#66787f;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;"">
<p style=""margin: 0; word-break: break-word;""><a href=""https://rentwisegh.com/Page/Contact"" rel=""noopener"" style=""text-decoration: underline; color: #0068a5;"" target=""_blank"">Contact Us</a></p>
</div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-3"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; background-color: #ffffff; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table><!-- End -->
</body>
</html>
";


            return content;
        }
    public static string EmailContentReply(string Name, string header, string body)
    {
        string body2 = string.Empty;
        string buttonText = "Contact Us";
        string buttonLink = "https://rentwisegh.com/Page/Contact";
        
        string content = @"
<!DOCTYPE html>

<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]--><!--[if !mso]><!-->
<link href=""https://fonts.googleapis.com/css?family=Roboto"" rel=""stylesheet"" type=""text/css""/><!--<![endif]-->
<style>
		* {
			box-sizing: border-box;
		}

		body {
			margin: 0;
			padding: 0;
		}

		a[x-apple-data-detectors] {
			color: inherit !important;
			text-decoration: inherit !important;
		}

		#MessageViewBody a {
			color: inherit;
			text-decoration: none;
		}

		p {
			line-height: inherit
		}

		.desktop_hide,
		.desktop_hide table {
			mso-hide: all;
			display: none;
			max-height: 0px;
			overflow: hidden;
		}

		.image_block img+div {
			display: none;
		}

		@media (max-width:620px) {

			.desktop_hide table.icons-inner,
			.social_block.desktop_hide .social-table {
				display: inline-block !important;
			}

			.icons-inner {
				text-align: center;
			}

			.icons-inner td {
				margin: 0 auto;
			}

			.mobile_hide {
				display: none;
			}

			.row-content {
				width: 100% !important;
			}

			.stack .column {
				width: 100%;
				display: block;
			}

			.mobile_hide {
				min-height: 0;
				max-height: 0;
				max-width: 0;
				overflow: hidden;
				font-size: 0px;
			}

			.desktop_hide,
			.desktop_hide table {
				display: table !important;
				max-height: none !important;
			}
		}
	</style>
</head>
<body style=""background-color: #FFFFFF; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #FFFFFF;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""image_block block-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""width:100%;"">
<div align=""center"" class=""alignment"" style=""line-height:10px"">
<div style=""max-width: 488px;""><img alt=""Rentwise Logo"" src=""https://rentwisegh.com/img/logo.png"" style=""display: block; height: auto; border: 0; width: 100%;"" title=""Rentwise Logo"" width=""488""/></div>
</div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f8f8f8; color: #000000; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 20px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
<div class=""spacer_block block-1"" style=""height:45px;line-height:45px;font-size:1px;""> </div>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""heading_block block-2"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-left:20px;padding-right:20px;text-align:center;width:100%;"">
<h1 style=""margin: 0; color: #2a3940; direction: ltr; font-family: Roboto, Tahoma, Verdana, Segoe, sans-serif; font-size: 23px; font-weight: normal; letter-spacing: normal; line-height: 120%; text-align: center; margin-top: 0; margin-bottom: 0; mso-line-height-alt: 27.599999999999998px;""><span class=""tinyMce-placeholder"">Hey " + Name + @",</span></h1>
</td>}
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""paragraph_block block-3"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:20px;padding-right:20px;padding-top:15px;"">
<div style=""color:#2a3940;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:18px;line-height:120%;text-align:center;mso-line-height-alt:21.599999999999998px;"">
<p style=""margin: 0; word-break: break-word;""><span>" + header + @"</span></p>
<p style=""margin: 0; word-break: break-word;""> </p>
<p style=""margin: 0; word-break: break-word;""><span>" + body + @"<br/>Every word counts!</span></p>
<p style=""margin: 0; word-break: break-word;""><span>" + body2 + @"<br/>Every word counts!</span></p>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""button_block block-4"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:10px;padding-right:10px;padding-top:30px;text-align:center;"">
<div align=""center"" class=""alignment""><!--[if mso]>
<v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""www.example.com"" style=""height:46px;width:213px;v-text-anchor:middle;"" arcsize=""3%"" stroke=""false"" fillcolor=""#f3aa0c"">
<w:anchorlock/>
<v:textbox inset=""0px,0px,0px,0px"">
<center style=""color:#ffffff; font-family:Tahoma, Verdana, sans-serif; font-size:18px"">
<![endif]--><a href=""" + buttonLink + @""" style=""text-decoration:none;display:inline-block;color:#ffffff;background-color:#f3aa0c;border-radius:1px;width:auto;border-top:0px solid #8a3b8f;font-weight:undefined;border-right:0px solid #8a3b8f;border-bottom:0px solid #8a3b8f;border-left:0px solid #8a3b8f;padding-top:5px;padding-bottom:5px;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:18px;text-align:center;mso-border-alt:none;word-break:keep-all;"" target=""_blank""><span style=""padding-left:40px;padding-right:40px;font-size:18px;display:inline-block;letter-spacing:normal;""><span style=""word-break:break-word;""><span data-mce-style="""" style=""line-height: 36px;"">" + buttonText + @"</span></span></span></a><!--[if mso]></center></v:textbox></v:roundrect><![endif]--></div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""text_block block-5"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"">
<div style=""font-family: sans-serif"">
<div class="""" style=""font-size: 14px; font-family: Roboto, Tahoma, Verdana, Segoe, sans-serif; mso-line-height-alt: 16.8px; color: #2a3940; line-height: 1.2;"">
</div>
</div>
</td>
</tr>
</table>
<div class=""spacer_block block-6"" style=""height:60px;line-height:60px;font-size:1px;""> </div>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""divider_block block-7"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:30px;padding-left:10px;padding-right:10px;padding-top:10px;"">
<div align=""center"" class=""alignment"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""95%"">
<tr>
<td class=""divider_inner"" style=""font-size: 1px; line-height: 1px; border-top: 2px solid #E8E8E8;""><span> </span></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""social_block block-8"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
<tr>
<td class=""pad"">
<div align=""center"" class=""alignment"">
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""social-table"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block;"" width=""46px"">
<tr>
<td style=""padding:0 7px 0 7px;""><a href=""https://www.instagram.com/rentwise_gh"" target=""_blank""><img alt=""Instagram"" height=""32"" src=""https://rentwisegh.com/img/instagram.png"" style=""display: block; height: auto; border: 0;"" title=""instagram"" width=""32""/></a></td>
</tr>
</table>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""paragraph_block block-9"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"" style=""padding-bottom:10px;padding-left:30px;padding-right:30px;padding-top:10px;"">
<div style=""color:#66787f;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;"">
<p style=""margin: 0; word-break: break-word;"">At Rentwise, we are committed to making high-quality services and items available for people to rent and make money in the most affordable and convenient way possible. </p>
</div>
</td>
</tr>
</table>
<table border=""0"" cellpadding=""10"" cellspacing=""0"" class=""paragraph_block block-10"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;"" width=""100%"">
<tr>
<td class=""pad"">
<div style=""color:#66787f;font-family:Roboto, Tahoma, Verdana, Segoe, sans-serif;font-size:14px;line-height:120%;text-align:center;mso-line-height-alt:16.8px;"">
<p style=""margin: 0; word-break: break-word;""><a href=""https://rentwisegh.com/Page/Contact"" rel=""noopener"" style=""text-decoration: underline; color: #0068a5;"" target=""_blank"">Contact Us</a></p>
</div>
</td>
</tr>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-3"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #ffffff;"" width=""100%"">
<tbody>
<tr>
<td>
<table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; background-color: #ffffff; width: 600px; margin: 0 auto;"" width=""600"">
<tbody>
<tr>
<td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table>
</td>
</tr>
</tbody>
</table><!-- End -->
</body>
</html>
";


        return content;
    }
        public static string GenerateOTP(int length = 6)
        {
            const string characters = "0123456789";
            Random random = new Random();

            char[] otpArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                otpArray[i] = characters[random.Next(characters.Length)];
            }

            return new string(otpArray);
        }

        public static async Task SendPushNotification( string userId, string header, string message,string redirectUrl = "https://rentwisegh.com/Page/Chat")
        {
            string restApiKey = "OGM0MDgxM2UtN2I4Yy00ODQyLWI2NDEtZTJiODhmYjJhMDBl";
            string appId = "b88de5c6-032a-4026-a52f-e61732fc390b";

            if(userId == "All")
            {
               var notificationData = new
                {
                    app_id = appId,
                    contents = new { en = message },
                    included_segments = new[] { "All" },
                    headings = new { en = header },
                    url = redirectUrl
                };
                var options = new RestClientOptions("https://onesignal.com/api/v1/notifications");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic " + restApiKey);
                request.AddJsonBody(notificationData);

                var response = await client.PostAsync(request);
            } else
            {

                var notificationData = new
                {
                    app_id = appId,
                    contents = new { en = message },
                    include_external_user_ids = new[] { userId },
                    headings = new { en = header },
                    url = redirectUrl
                };

                var options = new RestClientOptions("https://onesignal.com/api/v1/notifications");
                var client = new RestClient(options);
                var request = new RestRequest("");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic " + restApiKey);
                request.AddJsonBody(notificationData);

                var response = await client.PostAsync(request);
            }

        }
        public static string EmailContent(string Name, string type)
        {
            string header = string.Empty;
            string body = string.Empty;
            string buttonText = string.Empty;
            string buttonLink = string.Empty;

            if (type == "ENABLE")
            {
                header = "Account Status Notification";
                if (Name != null)
                {
                    body = $"Dear {Name},\n\nYour account has been enabled.";
                }
                else
                {
                    body = "Dear Agent,\n\nYour account has been enabled.";
                }
                buttonLink = "https://agent.rentwisegh.com";
                buttonText = "Login to Your Account";
            }
            else if (type == "DISABLED")
            {
                header = "Account Status Notification";
                if (Name != null)
                {
                    body = $"Dear {Name},\n\nYour account has been disabled.";
                }
                else
                {
                    body = "Dear Agent,\n\nYour account has been disabled.";
                }
                body += "\n\nIf you believe this is an error or have any questions, please contact customer support.";
                buttonLink = "https://rentwisegh.com/Page/Contact";
                buttonText = "Contact Customer Support";
            }

            string content = @"
<!DOCTYPE html>
<html lang=""en"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:v=""urn:schemas-microsoft-com:vml"">
<head>
<title></title>
<meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type""/>
<meta content=""width=device-width, initial-scale=1.0"" name=""viewport""/>
<link href=""https://fonts.googleapis.com/css?family=Roboto"" rel=""stylesheet"" type=""text/css""/>
<style>
    /* CSS styles */
    * {
        box-sizing: border-box;
    }
    body {
        margin: 0;
        padding: 0;
        background-color: #FFFFFF;
        -webkit-text-size-adjust: none;
        text-size-adjust: none;
    }
</style>
</head>
<body>
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""nl-container"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #FFFFFF;"" width=""100%"">
        <tbody>
            <tr>
                <td>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row row-1"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt;"" width=""100%"">
                        <tbody>
                            <tr>
                                <td>
                                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""row-content stack"" role=""presentation"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 600px; margin: 0 auto;"" width=""600"">
                                        <tbody>
                                            <tr>
                                                <td class=""column column-1"" style=""mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; padding-bottom: 5px; padding-top: 5px; vertical-align: top; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;"" width=""100%"">
                                                    <h2 style=""color: #333333; font-family: 'Roboto', sans-serif; font-size: 24px; line-height: 30px; font-weight: bold;"">Header: " + header + @"</h2>
                                                    <p style=""color: #666666; font-family: 'Roboto', sans-serif; font-size: 16px; line-height: 24px;"">Body: " + body + @"</p>
                                                    <a href=""" + buttonLink + @""" style=""background-color: #007BFF; border: none; color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin-top: 20px;"">" + buttonText + @"</a>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</body>
</html>
";


            return content;
        }

    }

}

