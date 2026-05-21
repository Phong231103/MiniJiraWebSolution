namespace Web.Infrastructure.TemplateMail
{
    public static class EmailTemplates
    {
        public static string GetOtpEmailTemplate(string otp)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                .otp-code {{ font-size: 32px; font-weight: bold; color: #2c3e50; letter-spacing: 5px; text-align: center; margin: 30px 0; }}
                .footer {{ font-size: 12px; color: #888; text-align: center; margin-top: 20px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h2>Xác thực tài khoản</h2>
                <p>Xin chào,</p>
                <p>Bạn đang thực hiện đăng ký tài khoản. Vui lòng sử dụng mã OTP bên dưới để xác thực:</p>
                <div class='otp-code'>{otp}</div>
                <p>Mã OTP có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
                <div class='footer'>Nếu bạn không thực hiện yêu cầu này, hãy bỏ qua email này.</div>
            </div>
        </body>
        </html>";
        }
    }
}
