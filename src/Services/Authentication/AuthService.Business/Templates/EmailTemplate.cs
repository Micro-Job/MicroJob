using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Templates
{
    public class EmailTemplate(IConfiguration _configuration)
    {
        public string ResetPassword(string token, string username)
        {
            string ip = _configuration["ResetPasswordUrl"]!;

            return $@"
<!DOCTYPE html>
<html lang='en-US'>

<head>
    <meta content='text/html; charset=utf-8' http-equiv='Content-Type' />
    <title></title>
    <meta name='description' content='Reset Password Email Template.'>
    <style type='text/css'>
        @import url(https://fonts.googleapis.com/css?family=Rubik:300,400,500,700|Open+Sans:300,400,600,700); 
        *{{font-family:Open Sans, sans-serif;}}
        a:hover {{ text-decoration: underline !important; }}
    </style>
</head>

<body marginheight='0' topmargin='0' marginwidth='0' style='margin: 0px; background-color: #f2f3f8;' leftmargin='0'>
    <!--100% body table-->
    <table cellspacing='0' border='0' cellpadding='0' width='100%' bgcolor='#f2f3f8'
        style=''>
        <tr>
            <td>
                <table style='background-color: #f2f3f8; max-width:670px; margin:0 auto;' width='100%' border='0'
                    align='center' cellpadding='0' cellspacing='0'>
                    <tr>
                        <td style='height:80px;'>&nbsp;</td>
                    </tr>
                    <tr>
                        <td style='text-align:center;'>
                            <a href='{ip}' title='logo' target='_blank'>
                                <img width='150' src='{ip}/icons/maillogo.png' title='logo' alt='logo'>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td style='height:20px;'>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <table width='95%' border='0' align='center' cellpadding='0' cellspacing='0'
                                style='max-width:670px;background:#fff; border-radius:3px; text-align:center;-webkit-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);-moz-box-shadow:0 6px 18px 0 rgba(0,0,0,.06);box-shadow:0 6px 18px 0 rgba(0,0,0,.06);'>
                                <tr>
                                    <td style='height:40px;'>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style='padding:0 35px;'>
                                        <h1 style='color:#1e1e2d; font-weight:500; margin:0;font-size:32px; font-family: Montserrat;'>
                                            ""HIRI""
                                        </h1>
                                        <h4 style='font-family: Montserrat;'>İstifadəçinin tam adı : <span style='color: #6fccff;'>{username}</span></h4>
                                        <p style='color:#455056; font-size:15px;line-height:24px; margin:0; font-family: Montserrat;'>
                                            Şifrəni yenilə düyməsinə sıxmaqla özünüzə yeni şifrə təyin edin və unutmayın.
                                        </p>
                                        <a href='{ip}/reset-password/{token}'
                                            style='background:#6fccff;text-decoration:none !important; font-weight:500; margin-top:35px; color:#fff;text-transform:uppercase; font-family: Montserrat; font-size:14px;padding:10px 24px;display:inline-block;border-radius:50px;'>ŞİFRƏNİ YENİLƏ</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='height:40px;'>&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='height:20px;'>&nbsp;</td>
                    </tr>
                    <tr>
                        <td style='height:80px;'>&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>

</html>";
        }
    }
}
