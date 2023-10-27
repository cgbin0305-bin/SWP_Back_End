
using System.Collections.Generic;
namespace API.Helper
{
    public class SendAndRetrieveRandomOtpCode
    {
        public Dictionary<int, string> otpCodeDictionary = new Dictionary<int, string>();

        public string GenerateRandomOtpCode()
        {
            // Generate a random 6-digit OTP code
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        public void StoreOtpCodeForUser(int userId, string otpCode)
        {
            // Store the OTP code in a dictionary with the user's ID as the key
            otpCodeDictionary[userId] = otpCode;
        }

        public string GetStoredOtpCodeForUser(int userId)
        {
            // Retrieve the OTP code from the dictionary based on the user's ID
            if (otpCodeDictionary.ContainsKey(userId))
            {
                return otpCodeDictionary[userId];
            }

            // Return null or an empty string if the user's OTP code is not found
            return null;
        }
    }
}