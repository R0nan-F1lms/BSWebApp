using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Drawing;

namespace BSWebApp.Services
{
    public class EmailOuterService
    {
        public EmailInnerService Inner { get; set; }

        // Constructor to establish link between
        // instance of Outer_class to its
        // instance of the Inner_class
        public EmailOuterService()
        {
            this.Inner = new EmailInnerService(this);
        }



        public class EmailInnerService
        {

            private EmailOuterService obj;
            public EmailInnerService()
            {


            }
            public EmailInnerService(EmailOuterService outer)
            {

                this.obj = outer;
            }

            public bool SendEmailConfirmation(string emailTo, string Lname, string url)
            {

                var email = new MimeMessage();
                // Using Gmail
                email.From.Add(MailboxAddress.Parse("rosella.terry@ethereal.email"));

                // Use string interpolation to replace placeholders in the HTML body
                var emailBody = $@"<!DOCTYPE html>
    <html>
    <head>
        <style>
            /* Add your CSS styles here for a professional look */
            body {{
                font-family: Arial, sans-serif;
                background-image: url(./wwwroot/images/bsW.jpg);
                background-size: cover;
                margin: 0;
                padding: 0;
            }}
            .container {{
                max-width: 600px;
                margin: 0 auto;
                
                padding: 20px;
            }}
            .content {{
                background - color: rgba(255, 255, 255, 0.5); /* White color with 70% opacity */
                padding: 20px; /* Add padding to the content for spacing */
                border-radius: 10px; /* Rounded corners for the box */
                margin: 20px;
                backdrop-filter: blur(20px)/* Adjust the margin for spacing */
            }}
            .header {{
                background: #2F6672;
                color: #fff;
                text-align: center;
                padding: 10px;
            }}
            .button {{
                display: inline-block;
                padding: 10px 20px;
                background: #4AA1B5;
                color: #fff;
                text-decoration: none;
            }}
        </style>
    </head>
    <body>
        <div class=""container"">
            <div class=""header"">
                <img src=""./images/beanScene_LightLogo.png"" alt=""BeanScene Logo"" />
               
            </div class=""content"">
            <p>Dear {Lname},</p>
            <p>You have successfully signed up as a member to BeanScene.</p>
            <p>Kind Regards,</p>
            <p>BeanScene Service Team</p>
            <p>
                <a class=""button"" href=""{url}"">Click here to verify your account</a>
            </p>
        </div>
    </body>
    </html>";
                //Using Etheral
                // email.From.Add(MailboxAddress.Parse("arlie.kreiger@ethereal.email"));
                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = "Registration Confirmation";
                email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

                // send email
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                ////Using Etheral
                // smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
                //smtp.Authenticate("rosella.terry@ethereal.email", "9t8kYzD9sCBQyPxsap");

                // Using Gmail
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("ronan.films04@gmail.com", "bxaeetcqzslkurhe");


                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }


        }

    }

}

