using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace _2280602494_DuongCongPhuoc_Mobile.Services
{
    /// <summary>
    /// Service gửi SMS OTP sử dụng eSMS.vn theo tài liệu:
    /// https://developers.esms.vn/ (SendMultipleMessage_V4)
    /// </summary>
    public class SpeedSMSService
    {
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly string _brandName;
        //Đường dẫn gốc của API eSMS (phiên bản JSON).
        private const string RootURL = "https://rest.esms.vn/MainService.svc/json";

        public SpeedSMSService(IConfiguration configuration)
        {
            _apiKey = configuration["ESMS_API_KEY"]
                ?? throw new Exception("ESMS_API_KEY không được tìm thấy trong environment variables");

            _secretKey = configuration["ESMS_SECRET_KEY"]
                ?? throw new Exception("ESMS_SECRET_KEY không được tìm thấy trong environment variables");

            // Brandname CSKH free mà eSMS trả về trong GetBrandname: Baotrixemay (Type = 2)
            _brandName = configuration["ESMS_BRANDNAME"] ?? "Baotrixemay";
        }

        public async Task<string> SendSMSOTP(string phone, string otp)
        {
            try
            {
                // API eSMS: SendMultipleMessage_V4 (GET)
                // Nội dung phải khớp đúng template CSKH đã được eSMS duyệt:
                // Template: "{P1,20} la ma xac minh dang ky Baotrixemay cua ban"
                var content = Uri.EscapeDataString($"{otp} la ma xac minh dang ky Baotrixemay cua ban");

                string url =
                    $"{RootURL}/SendMultipleMessage_V4_get" +
                    $"?ApiKey={_apiKey}" +
                    $"&SecretKey={_secretKey}" +
                    $"&Phone={phone}" +
                    $"&Content={content}" +
                    $"&SmsType=2" +                 // 2 = tin CSKH theo docs eSMS
                    $"&IsUnicode=0" +               // không dấu để tránh lỗi mã hóa
                    $"&Brandname={_brandName}";     // sử dụng brandname Baotrixemay (Type = 2)

                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gửi SMS qua eSMS: {ex.Message}");
            }
        }
    }
}
