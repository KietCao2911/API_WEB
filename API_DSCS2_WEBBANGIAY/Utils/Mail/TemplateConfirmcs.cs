//using API_DSCS2_WEBBANGIAY.Models;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace API_DSCS2_WEBBANGIAY.Utils.Mail
//{
//    public static class FillData
//    {
//        public static string Teamplate(PhieuNhapXuat dh ,List<ChiTietNhapXuat> cthds)
//        {
//            string FilePath = Directory.GetCurrentDirectory() + "\\Utils\\Mail\\index.html";
//            StreamReader str = new StreamReader(FilePath);
//            string MailText = str.ReadToEnd();
//            str.Close();
//            MailText = MailText.Replace("[Name]", "Kiệt Trươngg")
//                .Replace("[WardName]", dh.DiaChiNavigation.WardName)
//                .Replace("[DistrictName]", dh.DiaChiNavigation.DistrictName)
//                .Replace("[ProvinceName]", dh.DiaChiNavigation.ProvinceName)
//                .Replace("[PRODUCTDETAILS]", loadProdctDetails(cthds))
//                .Replace("[MethodPayment]", dh.PhuongThucThanhToan);
//            return MailText;
//        }
//        public static string loadProdctDetails(List<ChiTietNhapXuat> cthd)
//        {
//            string result = "";
//            foreach(var item in cthd)
//            {
//                var temp1= "https:\\localhost:44328\\wwwroot\\res\\SanPhamRes\\Imgs\\"+item.MaSanPham+"\\"+item.SanPhamNavigation.IDColor.Trim()+"\\";
//                var img ="";
//                string temp = $@"<div class='u - row - container' style='padding: 0px; background - color: transparent'>
//           <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>
//      <div style = 'border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>
  


//  <div class='u-col u-col-23p14' style='max-width: 320px;min-width: 115.7px;display: table-cell;vertical-align: top;'>
//    <div style = 'height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>
    
//  <table style = 'font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
//    <tbody>
//      <tr>
//        <td style = 'overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:arial,helvetica,sans-serif;' align='left'>
          
//  <table width = '100%' cellpadding='0' cellspacing='0' border='0'>
//    <tbody><tr>
//      <td style = 'padding-right: 0px;padding-left: 0px;' align='center'>
        
//        <img align = 'center' border='0' src='{img}' alt='Jewelry Bracelet ' title='Jewelry Bracelet ' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 85%;max-width: 98.34px;' width='98.34'>
        
//      </td>
//    </tr>
//  </tbody></table>
  
//        </td>
//      </tr>
//    </tbody>
//  </table>
  
//    </div>
//  </div>
//  <div class='u-col u-col-54p33' style='max-width: 320px;min-width: 271.65px;display: table-cell;vertical-align: top;'>
//    <div style = 'background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>
    
//  <table style = 'font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
//    <tbody>
//      <tr>
//        <td style = 'overflow-wrap:break-word;word-break:break-word;padding:30px 10px 67px 20px;font-family:arial,helvetica,sans-serif;' align='left'>
          
//    <div style = 'color: #000000; line-height: 140%; text-align: left; word-wrap: break-word;'>
//      <p style='font-size: 14px; line-height: 140%;'><span style = 'font-family: Montserrat, sans-serif; font-size: 14px; line-height: 19.6px;'><span style='color: #000000; font-size: 14px; line-height: 19.6px;'><strong><span style = 'font-size: 14px; line-height: 19.6px;'> {item?.MasanPhamNavigation?.TenSanPham.Trim()}</span></strong></span><span style = 'color: #666666; font-size: 14px; line-height: 19.6px;'> x1 </ span></ span></ p>
  

//            <p style='font-size: 14px; line-height: 140%;'><span style = 'font-family: Montserrat, sans-serif; font-size: 14px; line-height: 19.6px;'>{item?.MausacPhamNavigation?.TenMau}</span></p>
//  <p style = 'font-size: 14px; line-height: 140%;'><span style= 'font-family: Montserrat, sans-serif; font-size: 14px; line-height: 19.6px;'> </span></p>
//    </div>
  
//        </td>
//      </tr>
//    </tbody>
//  </table>
  
//    </div>
//  </div>
//  <div class='u-col u-col-22p53' style='max-width: 320px;min-width: 112.65px;display: table-cell;vertical-align: top;'>
//    <div style = 'background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>
    
//  <table style = 'font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
//    <tbody>
//      <tr>
//        <td style = 'overflow-wrap:break-word;word-break:break-word;padding:30px 10px 47px 20px;font-family:arial,helvetica,sans-serif;' align='left'>
          
//    <div style = 'line-height: 140%; text-align: left; word-wrap: break-word;'>
//{(item?.MasanPhamNavigation?.GiamGia > 0 ? $@" <p style='font-size: 14px; line-height: 140%;'>

//< strong >< span style = 'text-decoration: line-through; font-size: 16px; line-height: 22.4px; font-family: Montserrat,
//sans-serif;'>{FormatCurrency.Vnd(item?.giaBan/(100-item?.MasanPhamNavigation?.GiamGia)/100)}</span></strong>
//</p>": $"<p style = 'font-size: 14px; line-height: 140%;'>{FormatCurrency.Vnd(item?.giaBan)}</p>")
//}
      
//    </div>
  
//        </td>
//      </tr>
//    </tbody>
//  </table>
  

//    </div>
//  </div>
//      </div>
//    </div>
//  </div>
//  <div class='u-row-container' style='padding: 0px;background-color: transparent'>
//    <div class='u-row' style='Margin: 0 auto;min-width: 320px;max-width: 500px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;'>
//      <div style = 'border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;'>
  


//  <div class='u-col u-col-100' style='max-width: 320px;min-width: 500px;display: table-cell;vertical-align: top;'>
//    <div style = 'background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;'>
    
//  <table style = 'font-family:arial,helvetica,sans-serif;' role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
//    <tbody>
//      <tr>
//        <td style = 'overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:arial,helvetica,sans-serif;' align='left'>
          
//    <table height = '0px' align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 2px solid #e7e7e7;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
//      <tbody>
//        <tr style = 'vertical-align: top'>
//          <td style='word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%'>
//            <span>&nbsp;</span>
//          </td>
//        </tr>
//      </tbody>
//    </table>
  
//        </td>
//      </tr>
//    </tbody>
//  </table>
  
//    </div>
//  </div>
//      </div>
//    </div>
//  </div>";
//                result += temp;
//            }
//                return result;
            
//        }
//    }
//}
