using EmailUploadedDocument.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace EmailUploadedDocument.Controllers
{
    public class UserController : Controller
    {
        private readonly DataEntities _db;

        public UserController(DataEntities db)
        {
            _db = db;
        }

        public UserController()
        {
            _db = new DataEntities();
        }

        // GET: User

        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(User user, HttpPostedFileBase uploaded)
        {
            try
            {
                var transn = GenerateTransNumber();
                List<Attachment> attachments = new List<Attachment>();
                while (_db.Documents.Any(d => d.TransactionNumber == transn))
                {
                    transn = GenerateTransNumber();
                }
                if (uploaded.ContentLength > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string path = Server.MapPath("~") + "\\Content\\" + file.FileName;
                        file.SaveAs(path);
                        Document document = new Document
                        {
                            DocumentName = file.FileName,
                            Doc = GetBytes(path),
                            Extension = Path.GetExtension(path).Substring(1),
                            TransactionNumber = transn
                        };
                        _db.Documents.Add(document);
                        MemoryStream memoryStream = new MemoryStream(GetBytes(path));
                        attachments.Add(new Attachment(memoryStream, path, "image/jpeg"));
                    }
                }
                //SendMail("noresponse@smail.com", user.Email, "sending files", "am sending you some files", attachments);
                SendMail2("noresponse@smail.com", user.Email, "sending files", "am sending you some files", attachments, user.UserName);
                _db.Users.Add(user);
                _db.SaveChanges();
                return View(); //Redirect("list");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(user);
        }

        private long GenerateTransNumber()
        {
            Random rnd = new Random();
            long rndnum = rnd.Next(10000000, 99999999);
            return rndnum;
        }

        private void SendMail(string from, string to, string title, string body, List<Attachment> atachtments)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential("noreplykaysteph", "@jose6000");
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.Body = body;
            message.Subject = title;
            message.From = new MailAddress(from);
            message.IsBodyHtml = true;

            foreach (var atachment in atachtments)
            {
                message.Attachments.Add(atachment);
            }

            client.EnableSsl = true;
            client.Send(message);

        }

        public static void SendMail2(string from, string to, string title, string body, List<Attachment> atachtments, string name)
        {
            try
            {
                //Send to user

                SmtpClient smtpClient1 = new SmtpClient();
                MailMessage message1 = new MailMessage();

                message1.IsBodyHtml = true;
                MailAddress fromAddress1 = new MailAddress("noreply@steve.org", title);
                MailAddress toAddress1 = new MailAddress(to, title);
                message1.From = fromAddress1;
                message1.To.Add(toAddress1);
                message1.Subject = title;
                message1.Body = "<html><head><title>" + title + "</title></head>" +
                    "<body style='font-family: calibri, \"Century Gothic\";'>" +
                    "<div><h2>Sending Email With Attachment</h2></div><br/>" +
                               "<div><p>" + "Dear " + name + "<br/><br/>"
                               + "Please find attached file for your perusal" 

                               + "</div>" +
                    "</body></html>";
                foreach (var atachment in atachtments)
                {
                    message1.Attachments.Add(atachment);
                }

                smtpClient1.Host = "smtp.elasticemail.com";
                smtpClient1.Port = 2525;
                smtpClient1.EnableSsl = true;
                smtpClient1.Credentials = new System.Net.NetworkCredential("softwaredev@cyberspace.net.ng", "5b8a6fd9-8e07-4748-affa-0dcc39bf6754");//not neccessary
                smtpClient1.Send(message1);

                //smtpClient1.Host = "smtp-pulse.com";
                //smtpClient1.Port = 2525;
                //smtpClient1.EnableSsl = false;
                //smtpClient1.Credentials = new System.Net.NetworkCredential("noreplykaysteph@gmail.com", "KDeisS3tJLB");//not neccessary
                //smtpClient1.Send(message1);
            }
            catch (Exception ex)
            {
                ex.GetBaseException();

            }
        }


        private byte[] GetBytes(string path)
        {
            try
            {
                byte[] data = null;
                //open file stream to read the file
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                //use binary write to read byte into byte array
                using (BinaryReader br = new BinaryReader(stream))
                {
                    data = br.ReadBytes((int)stream.Length);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ActionResult ViewDocument(int id = 0)
        {
            Document document = _db.Documents.FirstOrDefault(d => d.DocumentId == id);
            if (document != null)
            {
                document.DocumentString = "data:image/png;base64," + Convert.ToBase64String(document.Doc);
                return View(document);
            }
            return View();
        }
        public ActionResult List(int? userId, string email, string transNum)
        {
            List<Document> documents = new List<Document>();

            try
            {
                if (TempData["userId"] == null && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(transNum))
                    documents = _db.Documents.ToList();
                else if (!(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(transNum)))
                {
                    User user = _db.Users.FirstOrDefault(u => u.Email == email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email does not exit in system");
                        return View(documents);
                    }
                    long transNo = long.Parse(transNum);
                    documents = _db.Documents.Where(d => d.UserId == user.UserId && d.TransactionNumber == transNo).ToList();
                }
                else
                {
                    userId = (int)TempData["userId"];
                    documents = _db.Documents.Where(d => d.UserId == userId).ToList();
                }


                return View(documents);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

    }
}