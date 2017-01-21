using EmailUploadedDocument.Models;
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
        public ActionResult CreateUser(User user, HttpPostedFileBase upload)
        {
            try
            {
                var transn = GenerateTransNumber();
                List<Attachment> attachments = new List<Attachment>();
                while (_db.Documents.Where(d => d.TransactionNumber == transn).Any())
                {
                    transn = GenerateTransNumber();
                }
                if (upload.ContentLength > 0)
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
                SendMail("noresponse@smail.com", user.Email, "sending files", "am sending you some files", attachments);
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
            Document document = _db.Documents.Where(d => d.DocumentId == id).FirstOrDefault();
            document.DocumentString = "data:image/png;base64," + Convert.ToBase64String(document.Doc);
            return View(document);
        }
        public ActionResult List(int? userId, string Email, string TransNum)
        {
            List<Document> documents = new List<Document>();

            try
            {
                if (TempData["userId"] == null && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(TransNum))
                    documents = _db.Documents.ToList();
                else if (!(string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(TransNum)))
                {
                    User user = _db.Users.Where(u => u.Email == Email).FirstOrDefault();
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email does not exit in system");
                        return View(documents);
                    }
                    long TransNo = long.Parse(TransNum);
                    documents = _db.Documents.Where(d => d.UserId == user.UserId && d.TransactionNumber == TransNo).ToList();
                    if (documents == null)
                    {
                        ModelState.AddModelError("", "Transaction Number does not exit in system");
                        return View(documents);
                    }
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