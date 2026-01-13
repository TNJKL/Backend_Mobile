using System.Collections.Concurrent;

namespace _2280602494_DuongCongPhuoc_Mobile.Services
{
    public class OTPService
    {
        // Store OTPs temporarily (in production, use Redis or database)
        private readonly ConcurrentDictionary<string, OTPData> _otpStore = new();
        private readonly Random _random = new();
        private readonly int _otpExpiryMinutes = 5;

        public string GenerateOTP()
        {
            // Generate 6-digit OTP
            return _random.Next(100000, 999999).ToString();
        }

        public void StoreOTP(string phone, string otp)
        {
            _otpStore[phone] = new OTPData
            {
                OTP = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes)
            };
        }
         
        //kiem tra OTP có đúng và chưa hết hạn không
        public bool VerifyOTP(string phone, string otp)
        {
            if (_otpStore.TryGetValue(phone, out var otpData))
            {
                if (DateTime.UtcNow > otpData.ExpiryTime)
                {
                    _otpStore.TryRemove(phone, out _);
                    return false; // OTP expired
                }

                if (otpData.OTP == otp)
                {
                    return true;
                }
            }

            return false;
        }

        public void ClearOTP(string phone)
        {
            _otpStore.TryRemove(phone, out _);
        }

        private class OTPData
        {
            public string OTP { get; set; } = string.Empty;
            public DateTime ExpiryTime { get; set; }
        }
    }
}
